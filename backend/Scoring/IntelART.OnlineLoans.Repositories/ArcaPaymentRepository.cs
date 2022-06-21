using Dapper;
using IntelART.OnlineLoans.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.Repositories
{
    public class ArcaPaymentRepository : BaseRepository
    {
        private readonly string createOrderEndpoint;
        private readonly string getOrderDetailsEndpoint;
        private readonly string refundOrderEndpoint;

        public ArcaPaymentRepository(string connectionString) : base(connectionString)
        {
            AppConfiguration config = new AppConfiguration();
            createOrderEndpoint = config.CreateOrderEndpoint;
            getOrderDetailsEndpoint = config.GetOrderDetailsEndpoint;
            refundOrderEndpoint = config.RefundOrderEndpoint;
        }

        public async Task<string> CreateOrder(DirectApplication application)
        {
            string formUrl = string.Empty;
            string orderId = string.Empty;

            // Create order in database
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("APPLICATION_PAYLOAD", JsonConvert.SerializeObject(application));
            var orderNumber = await GetSingleAsync<long>(parameters, string.Format("dbo.sp_CreateArcaOrder"));

            // Create payment order in ARCA
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(string.Format(createOrderEndpoint, orderNumber), null);

            var error = string.Empty;
            if (!response.IsSuccessStatusCode)
            {
                error = response.StatusCode.ToString();
            }
            else
            {
                var payload = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(payload);
                if (data.errorCode != "0")
                {
                    error = data.errorCode;
                }
                else
                {
                    formUrl = data.formUrl;
                    orderId = data.orderId;
                }                
            }

            DynamicParameters parametersToUpdate = new DynamicParameters();
            parametersToUpdate.Add("FORM_URL", formUrl);
            parametersToUpdate.Add("ARCA_ORDER_ID", orderId);
            parametersToUpdate.Add("ERROR_CODE", error);
            parametersToUpdate.Add("ID", orderNumber);
            await ExecuteAsync(parametersToUpdate, string.Format("dbo.sp_UpdateArcaOrderById"));

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return formUrl;
        }

        public async Task<DirectApplication> GetApplication(string arcaOrderId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ARCA_ORDER_ID", arcaOrderId);
            var order = await GetSingleAsync<ArcaPaymentOrder>(parameters, string.Format("dbo.sp_GetArcaOrderByOrderId"));
            var application = JsonConvert.DeserializeObject<DirectApplication>(order.APPLICATION_PAYLOAD);

            return application;
        }

        public async Task UpdateOrder(string arcaOrderId)
        {
            string cardHolderName = string.Empty;
            string orderStatus = string.Empty;

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(string.Format(getOrderDetailsEndpoint, arcaOrderId));
            var error = string.Empty;
            if (!response.IsSuccessStatusCode)
            {
                error = response.StatusCode.ToString();
            }
            else
            {
                var payload = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(payload);
                if (data.ErrorCode != "0")
                {
                    error = $"{data.ErrorCode} {data.ErrorMessage}";
                }
                else
                {
                    cardHolderName = data.cardholderName;
                    orderStatus = data.OrderStatus;
                }
            }

            DynamicParameters parametersToUpdate = new DynamicParameters();
            parametersToUpdate.Add("PAY_DATE", DateTime.UtcNow);
            parametersToUpdate.Add("CARD_HOLDER_NAME", cardHolderName);
            parametersToUpdate.Add("STATUS", orderStatus);
            parametersToUpdate.Add("ERROR_CODE", error);
            parametersToUpdate.Add("ARCA_ORDER_ID", arcaOrderId);
            await ExecuteAsync(parametersToUpdate, string.Format("dbo.sp_UpdateArcaOrderByOrderId"));

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
        }

        public async Task<bool> IsPaymentSuccessfull(string arcaOrderId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ARCA_ORDER_ID", arcaOrderId);
            var order = await GetSingleAsync<ArcaPaymentOrder>(parameters, string.Format("dbo.sp_GetArcaOrderByOrderId"));
            var application = JsonConvert.DeserializeObject<DirectApplication>(order.APPLICATION_PAYLOAD);

            //1 Pre-authorisation amount was held (for two-phase payment)
            //2 The amount was deposited successfully
            return new List<string> { "1", "2" }.Contains(order.STATUS)
                && !string.IsNullOrEmpty(order.CARD_HOLDER_NAME)
                && order.CARD_HOLDER_NAME.IndexOf(application.FIRST_NAME_AM) >= 0
                && order.CARD_HOLDER_NAME.IndexOf(application.LAST_NAME_AM) >= 0;
        }

        public async Task RefundOrder(string arcaOrderId)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(string.Format(refundOrderEndpoint, arcaOrderId), null);
            var error = string.Empty;
            if (!response.IsSuccessStatusCode)
            {
                error = response.StatusCode.ToString();
            }
            else
            {
                var payload = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(payload);
                if (data.errorCode != "0")
                {
                    error = $"{data.errorCode} {data.errorMessage}";
                }
                else
                {
                    DynamicParameters parametersToUpdate = new DynamicParameters();
                    parametersToUpdate.Add("IS_REFUNDED", true);
                    parametersToUpdate.Add("ARCA_ORDER_ID", arcaOrderId);
                    await ExecuteAsync(parametersToUpdate, string.Format("dbo.sp_UpdateArcaOrderByOrderId"));
                }
            }
        }
    }
}
