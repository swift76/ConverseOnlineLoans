using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for getting 
    /// the loan repayment schedule based on the loan amount, interest,
    /// and the loan duration
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("[controller]")]
    public class RepaymentScheduleController : RepositoryControllerBase<ApplicationRepository>
    {
        public RepaymentScheduleController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements GET /RepaymentSchedule?interest={interest}&amount={amount}&duration={duration}&serviceInterest={serviceInterest}serviceAmount={serviceAmount}
        /// Returns the loan repayment schedule based on the loan amount, the interest rate,
        /// and the loan duration in months
        /// </summary>
        [HttpGet]
        public RepaymentSchedule Get([FromQuery]double interest,
                                     [FromQuery]double amount,
                                     [FromQuery]byte duration,
                                     [FromQuery]double serviceInterest,
                                     [FromQuery]double serviceAmount)
        {
            RepaymentSchedule repaymentSchedule = new RepaymentSchedule
            {
                MONTHLY_PAYMENT_AMOUNT = Repository.CalculateMonthlyPaymentAmount(interest, amount, duration, serviceInterest, serviceAmount)
            };

            return repaymentSchedule;
        }
    }
}
