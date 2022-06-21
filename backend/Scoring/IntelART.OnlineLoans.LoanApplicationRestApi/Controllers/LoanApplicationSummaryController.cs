using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.Utilities;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing 
    /// the overall loan application summary details
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Authorize]
    [Route("/Applications/{id}/Summary")]
    public class LoanApplicationSummaryController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanApplicationSummaryController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements GET /Applications/{id}/Summary
        /// Returns agreed application with the given id
        /// </summary>
        [HttpGet]
        public async Task<ApplicationTermsSummaryDetails> Get(Guid id)
        {
            ApplicationTermsSummaryDetails application = await Repository.GetApplicationTermsSummaryDetails(id);
            return application;
        }
    }
}
