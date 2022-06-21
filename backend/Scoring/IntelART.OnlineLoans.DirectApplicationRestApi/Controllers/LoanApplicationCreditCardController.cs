using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using IntelART.Communication;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required
    /// for customers' credit card authorization
    /// </summary>
    [Route("/Applications/{id}")]
    public class LoanApplicationCreditCardController : RepositoryControllerBase<ApplicationRepository>
    {
        private ISmsSender smsSender;

        public LoanApplicationCreditCardController(IConfigurationRoot configuration, ISmsSender smsSender)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
            this.smsSender = smsSender;
        }

        /// <summary>
        /// Implements POST /Applications/{id}/CreditCardAuthorization
        /// Generates SMS code and sends it to the user for the given application ID
        /// </summary>
        [HttpPost("CreditCardAuthorization")]
        public async Task SendSMSCode(Guid id)
        {
            ClientDataForCardValidation clientData = await Repository.GetClientDataForCardValidation(id);
            Client card = await Repository.GetClientData(clientData.CLIENT_CODE);
            string phone = card.MobilePhone;
            string code = await Repository.GenerateSMSCode(id);
            await smsSender.SendAsync(phone, code);
            await Repository.LogSMSCreditCardStep(id, null, phone);
        }

        /// <summary>
        /// Implements POST /Applications/{id}/CheckCreditCardAuthorization
        /// Checks whether the customer has entered correct SMS Code generated
        /// for the application with a given application ID
        /// </summary>
        [HttpPost("CheckCreditCardAuthorization")]
        public async Task Post(Guid id, [FromBody]string smsCode)
        {
            await Repository.CheckCreditCardAuthorization(id, smsCode);
            await Repository.SubmitAuthorizedApplication(id);
        }

        /// <summary>
        /// Implements POST /Applications/{id}/ValidateCard
        /// Validates entered card number and expiry date for the given application ID
        /// </summary>
        [HttpPost("ValidateCard")]
        public async Task<bool> ValidateCard(Guid id, [FromBody]ClientCard cardNumber)
        {
            bool isValid = false;
            ClientDataForCardValidation clientData = await Repository.GetClientDataForCardValidation(id);
            await Repository.LogSMSCreditCardStep(id, cardNumber.cardNumber, null);
            ClientCard card = await Repository.GetClientCardData(clientData.CLIENT_CODE, cardNumber.cardNumber, cardNumber.expiryDate.Value);
            if (card != null)
            {
                int status = await Repository.IsCardActive(cardNumber.cardNumber, cardNumber.expiryDate.Value);
                if (status != 1 && status != 3 && status != 6 && status != 7 && status != 8 && status != 9 && status != 10 && status != 12 && status != 19 && status != 20 && status != 21)
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                await Repository.SubmitNotAuthorizedApplication(id);
                if (card == null) // incorrect card number has been inserted
                {
                    throw new ApplicationException("ERR-0505", "Տվյալների անհամապատասխանություն. խնդրում ենք ճշգրտել։ " +
                        "Հակառակ դեպքում վարկը ստանալու համար խնդրում ենք անձը հաստատող փաստաթղթով մոտենալ \"Կոնվերս Բանկ\" ՓԲԸ-ի " +
                        "ցանկացած մասնաճյուղ կամ կապ հաստատել \"Կոնվերս Բանկ\" ՓԲԸ-ի  հետ (+37410) 511 211 հեռախոսահամարով։ " +
                        "\"Կոնվերս Բանկ\" ՓԲԸ-ի սպասարկման ցանցը և աշխատանքի ժամանակացույցը ներկայացված են " +
                        "<a href = \"https://www.conversebank.am/hy/branches/\" target=\"_blank\"><b>հետևյալ հղումով:</b></a>");
                }
                else // correct card number has been inserted but the card has been blocked
                {
                    throw new ApplicationException("ERR-0507", "Վարկը ստանալու համար անհրաժեշտ է լրացուցիչ տեղեկատվություն։ " +
                        "Խնդրում ենք անձը հաստատող փաստաթղթով մոտենալ \"Կոնվերս Բանկ\" ՓԲԸ-ի ցանկացած մասնաճյուղ կամ կապ հաստատել " +
                        "\"Կոնվերս Բանկ\" ՓԲԸ-ի  հետ (+37410) 511 211 հեռախոսահամարով։ \"Կոնվերս Բանկ\" ՓԲԸ-ի սպասարկման ցանցը և " +
                        "աշխատանքի ժամանակացույցը ներկայացված են <a href = \"https://www.conversebank.am/hy/branches/\" target=\"_blank\"><b>հետևյալ հղումով:</b></a>");
                }
            }
            else // card verified
            {
                await Repository.SubmitCardAuthorizedApplication(id);
                string phone = card.MobilePhone;
                string code = await Repository.GenerateSMSCode(id);
                await smsSender.SendAsync(phone, code);
                await Repository.LogSMSCreditCardStep(id, null, phone);
            }

            return isValid;
        }

        /// <summary>
        /// Gets the list of active cards
        /// </summary>
        [HttpGet("ActiveCards")]
        public async Task<IEnumerable<ActiveClientCard>> GetActiveCards(Guid id)
        {
            ClientDataForCardValidation clientData = await Repository.GetClientDataForCardValidation(id);
            IEnumerable<ActiveClientCard> cards = await Repository.GetActiveClientCards(clientData.CLIENT_CODE, clientData.LOAN_TYPE_ID, clientData.CURRENCY_CODE);
            return cards;
        }

        /// <summary>
        /// Gets the list of possible options to receive credit card types
        /// </summary>
        [HttpGet("CreditCardTypes")]
        public async Task<IEnumerable<DirectoryEntity>> GetCreditCardTypes(Guid id)
        {
            InitialApplication application = await Repository.GetInitialApplication(id);
            IEnumerable<DirectoryEntity> creditCardTypes = await Repository.GetCreditCardTypes(this.languageCode, application.LOAN_TYPE_ID, application.CURRENCY_CODE);
            return creditCardTypes;
        }
    }
}
