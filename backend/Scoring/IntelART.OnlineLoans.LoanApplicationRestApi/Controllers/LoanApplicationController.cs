using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing, creating, and 
    /// managing loan applications
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Authorize]
    [Route("/Applications")]
    public class LoanApplicationController : RepositoryControllerBase<ApplicationRepository>
    {
        private readonly string remoteIpAddress;

        public LoanApplicationController(IConfigurationRoot configuration, IHttpContextAccessor httpContextAccessor)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
            if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                this.remoteIpAddress = httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
            else if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-OL-IP"))
                this.remoteIpAddress = httpContextAccessor.HttpContext.Request.Headers["X-OL-IP"];
            else
                this.remoteIpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        /// <summary>
        /// Implements GET /Applications?fromDate={fromDate}&toDate={toDate}&name={name}
        /// Returns all applications accessible to the current user
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<Application>> Get([FromQuery]DateTime fromDate,
                                                        [FromQuery]DateTime toDate,
                                                        [FromQuery]string name)
        {
            IEnumerable<Application> applications;
            fromDate = fromDate.Date;
            toDate = toDate.Date.AddDays(1);
            if (this.IsShopPowerUser) // shop manager
            {
                applications = await Repository.GetManagerApplications(this.CurrentUserID, fromDate, toDate, name);
            }
            else if (this.IsShopUser) // shop user
            {
                applications = await Repository.GetOperatorApplications(this.CurrentUserID, fromDate, toDate, name);
            }
            else // customer user
            {
                applications = await Repository.GetApplications(this.CurrentUserID);
            }

            return applications;
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
        public async Task<Guid> Post([FromBody]InitialApplication application)
        {
            if (application.SUBMIT && !application.AGREED_WITH_TERMS)
            {
                throw new ApplicationException("ERR-0020", "User must agree with terms and conditions before submitting an application.");
            }

            if (application.SUBMIT)
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
            }

            int? currentUserID = null;
            if (string.IsNullOrEmpty(application.PARTNER_COMPANY_CODE))
                currentUserID = this.CurrentUserID;

            this.Repository.LogClientIpAddress(remoteIpAddress, "CREATE APPLICATION", currentUserID);

            return await Repository.CreateInitialApplication(application, currentUserID, this.IsShopUser);
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
    }
}
