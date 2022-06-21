﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Repositories;

namespace IntelART.OnlineLoans.LoanApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required
    /// to reject the loan applications by a shop-user
    /// </summary>
    [Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("/Applications/Rejected")]
    public class LoanApplicationRejectionController : RepositoryControllerBase<ApplicationRepository>
    {
        public LoanApplicationRejectionController(IConfigurationRoot configuration)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
        }

        /// <summary>
        /// Implements PUT /Applications/Rejected/{id}
        /// Rejects the application with the given id by a shop-user
        /// </summary>
        [HttpPut("{id}")]
        public async Task Put(Guid id)
        {
            await Repository.RejectApplicationByShopUser(id);
        }
    }
}
