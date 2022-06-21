using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntelART.Utilities;
using IntelART.OnlineLoans.Repositories;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;

namespace IntelART.OnlineLoans.CustomerRestApi.Controllers
{
    /// <summary>
    /// A controller class that exposes Customers entities.
    /// </summary>
    [Route("[controller]")]
    [Authorize]
    public class ProfileController : Controller
    {
        private string ConnectionString;
        private CustomerUserRepository repository;
        
        private string CurrentUsername
        {
            get
            {
                return HttpContext.User.Identity.Name;
            }
        }

        private int CurrentUserID
        {
            get
            {
                return int.Parse(HttpContext.User.FindFirst("sub").Value);
            }
        }

        public ProfileController(IConfigurationRoot Configuration)
        {
            this.ConnectionString = Configuration.GetSection("ConnectionStrings")["ScoringDB"];
            this.repository = new CustomerUserRepository(this.ConnectionString);
        }

        /// <summary>
        /// Get the current user ID and lookup the
        /// CustiomerUser object with that ID
        /// </summary>
        [HttpGet]
        public CustomerUser Get()
        {
            CustomerUser customerUser = this.repository.GetCustomerUser(this.CurrentUserID);
            return customerUser;
        }

        /// <summary>
        /// Modify customer user
        /// </summary>
        [HttpPost]
        public void Post([FromBody]CustomerUser customerUser)
        {
            if (customerUser != null)
            {
                customerUser.HASH = "";
                if (!string.IsNullOrEmpty(customerUser.PASSWORD)) // password is modified
                {
                    ValidationManager.ValidatePasswordChange(customerUser.LOGIN, string.Empty, customerUser.PASSWORD, customerUser.PASSWORD);
                }
                this.repository.ModifyCustomerUser(customerUser, this.CurrentUserID);
            }
        }

        /// <summary>
        /// Changes Customer user password
        /// </summary>
        [HttpPut("login")]
        public void ChangeCustomerUserPassword([FromBody]ChangePasswordRequest changePasswordRequest)
        {
            string login = HttpContext.User.Identity.Name;
            ValidationManager.ValidatePasswordChange(login, changePasswordRequest.oldPassword, changePasswordRequest.newPassword, changePasswordRequest.newPasswordRepeat);

            if (repository.AuthenticateCustomerUser(login, Crypto.HashString(changePasswordRequest.oldPassword)) == null)
            {
                throw new ApplicationException("ERR-0029", "Հին գաղտնաբառը սխալ է");
            }

            repository.ChangeCustomerUserPassword(login, Crypto.HashString(changePasswordRequest.newPassword));
        }
    }

    public class ChangePasswordRequest
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string newPasswordRepeat { get; set; }
    }
}
