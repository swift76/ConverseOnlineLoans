using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.Utilities;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing, creating, and 
    /// managing loan applications
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("/Applications/{id}/Main")]
    public class LoanMainApplicationController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanMainApplicationController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements GET /Applications/{id}/Main
        /// Returns main application with the given id
        /// </summary>
        [HttpGet]
        public async Task<MainApplication> Get(Guid id)
        {
            MainApplication application = await Repository.GetMainApplication(id);
            return application;
        }

        /// <summary>
        /// Implements POST /Applications/{id}/Main
        /// Creates main application with the given id
        /// </summary>
        [HttpPost]
        public async Task Post(Guid id, [FromBody]MainApplication application)
        {
            await Repository.CreateMainApplication(id, application, null, false);
        }
    }
}
