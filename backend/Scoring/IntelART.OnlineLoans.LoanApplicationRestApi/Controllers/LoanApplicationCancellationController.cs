using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Repositories;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required
    /// to cancel the loan applications by the customers
    /// </summary>
    [Authorize]
    [Route("/Applications/Cancelled")]
    public class LoanApplicationCancellationController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanApplicationCancellationController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements POST /Applications/Cancelled/{id}
        /// Cancels the application with the given id by a customer
        /// </summary>
        [HttpPost("{id}")]
        public async Task Post(Guid id)
        {
            await Repository.CancelApplicationByCustomer(id);
        }
    }
}
