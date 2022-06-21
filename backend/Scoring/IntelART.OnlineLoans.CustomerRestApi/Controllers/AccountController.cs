using System;
using System.Linq;
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
    [Route("[controller]")]
    public class AccountController : Controller
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

        public AccountController(IConfigurationRoot Configuration, ISmsSender smsSender)
        {
            this.smsSender = smsSender;
            string connectionString = Configuration.GetSection("ConnectionStrings")["ScoringDB"];
            this.repository = new CustomerUserRepository(connectionString);
        }

        /// <summary>
        /// Creates new customer user
        /// </summary>
        [HttpPost]
        public async Task Post([FromBody]CustomerUserRegistrationPreVerification customerUser)
        {
            if (customerUser != null && !customerUser.ID.HasValue) // create customer user
            {
                if (repository.GetSetting("CLIENT_ONLY") == "1" 
                    && repository.GetClientByDocument(string.Empty, string.Empty, customerUser.SOCIAL_CARD_NUMBER).ToList().Count == 0)
                {
                    throw new ApplicationException("E-5012", "Նշված տվյալներով բանկի հաճախորդ գոյություն չունի");
                }

                string smsCode = (new Random()).Next(0, 9999).ToString().Trim().PadLeft(4, '0');
                customerUser.VERIFICATION_CODE = smsCode;
                this.repository.StartRegistration(customerUser);
                await smsSender.SendAsync(string.Format("374{0}", customerUser.MOBILE_PHONE.Trim()), string.Format("Mekangamya ogtagorcman kod - {0}", smsCode));
            }
        }

        /// <summary>
        /// Resends SMS code to a newly created customer user
        /// /Account/{registrationProcessId}
        /// </summary>
        [HttpPost("{registrationProcessId}")]
        public async Task Post(Guid registrationProcessId)
        {
            string smsCode = (new Random()).Next(0, 9999).ToString().Trim().PadLeft(4, '0');
            CustomerUserRegistrationPreVerification registrationProcess = this.repository.UpdateRegistration(registrationProcessId, smsCode);
         
            if (registrationProcess == null)
            {
                throw new ApplicationException("E-5002", "Անհայտ գրանցում");
            }
            else
            {
                await smsSender.SendAsync(string.Format("374{0}", registrationProcess.MOBILE_PHONE.Trim()), string.Format("Mekangamya ogtagorcman kod - {0}", smsCode));
            }
        }

        /// <summary>
        /// Set the SMS verification code for the given registration process ID.
        /// /Account/{registrationProcessId}/Verification/{verificationCode}
        /// </summary>
        [HttpPut("{registrationProcessId}/Verification/{verificationCode}")]
        public void Put(Guid registrationProcessId, string verificationCode)
        {
            CustomerUserRegistrationPreVerification registrationProcess = this.repository.GetRegistration(registrationProcessId);

            if (registrationProcess == null)
            {
                throw new ApplicationException("E-5002", "Անհայտ գրանցում");
            }

            if (registrationProcess.PROCESS_ID != registrationProcessId
                || registrationProcess.VERIFICATION_CODE != verificationCode)
            {
                throw new ApplicationException("E-5003", "Մուտքագրված կոդը սխալ է։ Կարող եք ստանալ նոր կոդ");
            }

            CustomerUser customerUser = new CustomerUser();
            customerUser.FIRST_NAME = registrationProcess.FIRST_NAME;
            customerUser.LAST_NAME = registrationProcess.LAST_NAME;
            customerUser.SOCIAL_CARD_NUMBER = registrationProcess.SOCIAL_CARD_NUMBER;
            customerUser.MOBILE_PHONE = registrationProcess.MOBILE_PHONE;
            customerUser.EMAIL = registrationProcess.EMAIL;
            customerUser.HASH = registrationProcess.HASH;

            if (customerUser != null && !customerUser.ID.HasValue) // create customer user
            {
                this.repository.CreateCustomerUser(customerUser);
            }
        }
    }
}
