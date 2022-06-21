using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for getting the  
    /// loan options available as a result of the successful scoring
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Authorize]
    [Route("/Applications/{id}")]
    public class LoanApplicationScoringResultsController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanApplicationScoringResultsController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements GET /Applications/{id}/InstallationScoringResults?shopCode={shopCode}&productCategoryCode={productCategoryCode}
        /// Returns scoring results for the main application with the given id
        /// </summary>
        [HttpGet("InstallationScoringResults")]
        public async Task<IEnumerable<ScoringResults>> Get(Guid id, [FromQuery]string shopCode, [FromQuery]string productCategoryCode)
        {
            if (shopCode == null && this.IsShopUser)
            {
                shopCode = Repository.GetHeadShopCode(this.CurrentUserID);
            }

            IEnumerable<ScoringResults> results = await Repository.GetInstallationApplicationScoringResult(id, shopCode, productCategoryCode);
            return results;
        }

        /// <summary>
        /// Implements GET /Applications/{id}/GeneralScoringResults
        /// Returns scoring results for the main application with the given id
        /// </summary>
        [HttpGet("GeneralScoringResults")]
        public async Task<IEnumerable<ScoringResults>> Get(Guid id)
        {
            IEnumerable<ScoringResults> results = await Repository.GetGeneralApplicationScoringResult(id);
            return results;
        }

        /// <summary>
        /// Implements GET /Applications/{id}/IsConverseCustomer
        /// Returns scoring results for the main application with the given id
        /// </summary>
        [HttpGet("IsConverseCustomer")]
        public async Task<bool> IsConverseCustomer(Guid id)
        {
            int? customerStatusID = await Repository.GetCustomerStatusID(id);
            return customerStatusID.HasValue && customerStatusID.Value == 3;
        }
    }
}
