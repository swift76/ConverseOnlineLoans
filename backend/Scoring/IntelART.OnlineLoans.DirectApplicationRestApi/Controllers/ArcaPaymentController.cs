using IntelART.Communication;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for ARCA payments processing
    /// </summary>
    [Route("/ArcaPayment")]
    public class ArcaPaymentController : RepositoryControllerBase<ArcaPaymentRepository>
    {
        public ArcaPaymentController(IConfigurationRoot configuration, ISmsSender smsSender)
            : base(configuration, (connectionString) => new ArcaPaymentRepository(connectionString))
        {

        }

        /// <summary>
        /// Update ARCA payment order id status in DB and if payment was successfull with right user details, register new loan application
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<Guid> Get(string orderId)
        {
            await Repository.UpdateOrder(orderId);

            if (await Repository.IsPaymentSuccessfull(orderId))
            {
                // Don`t wait for refund operation
                Repository.RefundOrder(orderId);

                var application = await Repository.GetApplication(orderId);

                var applicationRepo = new ApplicationRepository(this.connectionString);
                ApplicationCountSetting setting = await applicationRepo.GetApplicationCountSetting(application.SOCIAL_CARD_NUMBER, application.ID);
                if (setting.APPLICATION_COUNT > setting.REPEAT_COUNT)
                {
                    throw new ApplicationException("ERR-0200", "Application count overflow");
                }
                if (await applicationRepo.DoesClientWorkAtBank(application.SOCIAL_CARD_NUMBER))
                {
                    throw new ApplicationException("ERR-0201", "Bank employees are not permitted");
                }
                var appId = await applicationRepo.RegisterAndSubmitApplication(application);

                return appId;
            }
            else {
                throw new ApplicationException("ERR-0211", "Վճարումը ավարտվեց անհաջողությամբ կամ վճարողի տվյալները տարբերվում են վարկառուի տվյալներից");
            }
        }
    }
}