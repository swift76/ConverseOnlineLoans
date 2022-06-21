using System;
using IntelART.Communication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing, creating, and 
    /// managing loan applications
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("/Applications")]
    public class LoanApplicationController : RepositoryControllerBase<ApplicationRepository>
    {
        private ISmsSender smsSender;
        private readonly string remoteIpAddress;

        public LoanApplicationController(IConfigurationRoot configuration, ISmsSender smsSender, IHttpContextAccessor httpContextAccessor)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
            this.smsSender = smsSender;
            if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                this.remoteIpAddress = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
            else if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-OL-IP"))
                this.remoteIpAddress = httpContextAccessor.HttpContext.Request.Headers["X-OL-IP"];
            else
                this.remoteIpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        /// <summary>
        /// Implements GET /Applications/{id}
        /// Returns initial application with the given id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<InitialApplication> Get(Guid id)
        {
            InitialApplication application = await Repository.GetInitialApplication(id);
            return application;
        }

        /// <summary>
        /// Implements POST /Applications
        /// Creates a new initial application
        /// </summary>
        [HttpPost]
        public async Task<ApplicationRegistrationResult> Post([FromBody]DirectApplication application)
        {
            ApplicationCountSetting setting = await Repository.GetApplicationCountSetting(application.SOCIAL_CARD_NUMBER, application.ID);
            if (setting.APPLICATION_COUNT > setting.REPEAT_COUNT)
            {
                throw new ApplicationException("ERR-0200", "Application count overflow");
            }
            if (await Repository.DoesClientWorkAtBank(application.SOCIAL_CARD_NUMBER))
            {
                throw new ApplicationException("ERR-0201", "Bank employees are not permitted");
            }

            var response = new ApplicationRegistrationResult();
            var customersRepo = new CustomerUserRepository(this.connectionString);
            // If Customer SSN already exists in bank DB, proceed and submit loan application
            if (customersRepo.CheckCustomerUserExistenceByParameter("SOCIAL_CARD_NUMBER", application.SOCIAL_CARD_NUMBER, null))
            {               
                response.RegisteredApplicationId = await Repository.RegisterAndSubmitApplication(application);
                this.Repository.LogClientIpAddress(remoteIpAddress, "CREATE APPLICATION");
            }
            // If Customer SSN does not exist in bank DB, create new order in ARCA and ask user to pay a small amount of money to be identified.
            else
            {
                var arcaRepo = new ArcaPaymentRepository(this.connectionString);
                try
                {
                    response.PaymentFormUrl = await arcaRepo.CreateOrder(application);
                }
                catch
                {
                    throw new ApplicationException("ERR-0210", "Error occurred when creating payment order in ARCA system");
                }
            }

            return response;
        }

        /// <summary>
        /// Implements DELETE /Applications/{id}
        /// Deletes an application with the given id
        /// </summary>
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await Repository.DeleteApplication(id);
        }

        /// <summary>
        /// Implements GET /Applications/ApplicationInformation/{id}
        /// Returns information of the application with the given id
        /// </summary>
        [HttpGet("ApplicationInformation/{id}")]
        public async Task<ApplicationInformation> ApplicationInformation(Guid id)
        {
            ApplicationInformation application = await Repository.GetApplicationInformation(id);
            return application;
        }

        /// <summary>
        /// Implements POST /Applications/SendMobilePhoneAuthorization
        /// Generates SMS code and sends it for the given mobile phone
        /// </summary>
        [HttpPost("SendMobilePhoneAuthorization")]
        public async Task SendSMSCode([FromBody]string mobilePhone)
        {
            string code = await Repository.GenerateMobilePhoneAuthorizationCode(mobilePhone);
            await smsSender.SendAsync(string.Format("374{0}", mobilePhone), code);
        }

        /// <summary>
        /// Implements POST /Applications/CheckMobilePhoneAuthorization
        /// Check entered SMS code with sent for the given mobile phone
        /// </summary>
        [HttpPost("CheckMobilePhoneAuthorization")]
        public async Task CheckSMSCode([FromBody]CheckMobilePhoneAuthorization phoneAuthorization)
        {
            if (string.IsNullOrEmpty(phoneAuthorization.MOBILE_PHONE))
            {
                throw new ApplicationException("ERR-0202", "Mobile phone authorization code is empty");
            }
            await Repository.CheckMobilePhoneAuthorization(phoneAuthorization.MOBILE_PHONE, phoneAuthorization.SMS_CODE);
        }
    }
}
