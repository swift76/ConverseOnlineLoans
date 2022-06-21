using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntelART.Utilities;
using IntelART.OnlineLoans.Repositories;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.Communication;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.CustomerRestApi.Controllers
{
    /// <summary>
    /// A controller class that exposes functionality for customer account management.
    /// </summary>
    [Route("Account/{username}/PasswordManagerProcess")]
    public class AccountPasswordController : Controller
    {
        private ISmsSender smsSender;
        private CustomerUserRepository repository;

        private string CurrentUsername
        {
            get
            {
                return HttpContext.User.Identity.Name;
            }
        }

        public AccountPasswordController(IConfigurationRoot Configuration, ISmsSender smsSender)
        {
            this.smsSender = smsSender;
            string connectionString = Configuration.GetSection("ConnectionStrings")["ScoringDB"];
            this.repository = new CustomerUserRepository(connectionString);
        }

        /// <summary>
        /// Creates new password reset process
        /// </summary>
        [HttpPut("{processId}")]
        public async Task Put(string username, Guid processId)
        {
            if (processId != null)
            {
                if (this.repository.CheckCustomerUserExistenceByParameter("MOBILE_PHONE", username, null))
                {
                    string smsCode = (new Random()).Next(0, 9999).ToString().Trim().PadLeft(4, '0');
                    this.repository.StartCustomerUserPasswordReset(username, processId, Crypto.HashString(smsCode));
                    await smsSender.SendAsync(string.Format("374{0}", username), string.Format("Mekangamya ogtagorcman kod - {0}", smsCode));
                }
                else
                {
                    throw new ApplicationException("E-5112", "Տվյալ հեռախոսահամարով գրանցված օգտագործող գոյություն չունի");
                }
            }
        }

        /// <summary>
        /// Updates the password within the context of the given password update process.
        /// /Account/{username}/PasswordManagerProcess/{processId}
        /// </summary>
        [HttpPost("{processId}")]
        public void Post(string username, Guid processId, [FromBody]CustomerUserPasswordResetData data)
        {
            if (data == null)
            {
                throw new ApplicationException("E-5111", "Գաղտնաբառի վերականգնման թերի հարցում");
            }

            try
            {
                this.repository.ResetCustomerUserPassword(username, Crypto.HashString(data.SmsCode), processId, Crypto.HashString(data.NewPassword));
            }
            catch(Exception e)
            {
                throw new ApplicationException("E-5112", e.Message);
            }
        }
    }
}
