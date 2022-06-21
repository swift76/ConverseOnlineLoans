using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing, creating, and 
    /// managing loan applications
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("/Applications/{id}/Full")]
    public class LoanFullApplicationController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanFullApplicationController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements POST /Applications/{id}/Full
        /// Creates, saves, or submits main application with the given id, 
        /// along with its full data
        /// </summary>
        [HttpPost]
        public async Task Post(Guid id, [FromBody]FullApplication application)
        {
            application.loanDeliveryDetails.SUBMIT = false;
            await Repository.CreateAgreedApplication(id, application.loanDeliveryDetails);
            await Repository.CreateMainApplication(id, application.mainApplication, null, false);
        }
    }
}
