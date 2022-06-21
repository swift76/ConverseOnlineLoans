using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for creating
    /// final loan applications
    /// </summary>
    [Authorize]
    [Route("/Applications/{id}/Final")]
    public class LoanFinalApplicationController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanFinalApplicationController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements POST /Applications/{id}/Final
        /// Submits final application
        /// </summary>
        [HttpPost]
        public async Task<Guid> Post([FromBody]FinalApplication application)
        {
            return await Repository.CreateFinalApplication(application);
        }
    }
}
