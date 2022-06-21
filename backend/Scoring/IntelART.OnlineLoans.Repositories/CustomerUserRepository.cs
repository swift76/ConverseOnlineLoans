using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using IntelART.OnlineLoans.Entities;

namespace IntelART.OnlineLoans.Repositories
{
    public class CustomerUserRepository : BaseRepository
    {
        public CustomerUserRepository(string connectionString) : base(connectionString)
        {
        }

        public void StartRegistration(CustomerUserRegistrationPreVerification registration)
        {
            this.CheckCustomerUserExistence(registration.MOBILE_PHONE, registration.EMAIL, registration.SOCIAL_CARD_NUMBER);

            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();

            PrepareParameters(changes, parameters, "FIRST_NAME",         registration.FIRST_NAME,         true);
            PrepareParameters(changes, parameters, "LAST_NAME",          registration.LAST_NAME,          true);
            PrepareParameters(changes, parameters, "SOCIAL_CARD_NUMBER", registration.SOCIAL_CARD_NUMBER, true);
            PrepareParameters(changes, parameters, "MOBILE_PHONE",       registration.MOBILE_PHONE,       true);
            PrepareParameters(changes, parameters, "EMAIL",              registration.EMAIL,              false);
            PrepareParameters(changes, parameters, "PROCESS_ID",         registration.PROCESS_ID,         true);
            PrepareParameters(changes, parameters, "VERIFICATION_CODE",  registration.VERIFICATION_CODE,  true);

            parameters.Add("HASH", registration.HASH);
            Execute(parameters, "dbo.sp_StartCustomerUserRegistration");

        }

        public CustomerUserRegistrationPreVerification GetRegistration(Guid registrationId)
        {
            CustomerUserRegistrationPreVerification result = null;

            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();

            PrepareParameters(changes, parameters, "PROCESS_ID", registrationId, true);

            result = GetSingle<CustomerUserRegistrationPreVerification>(parameters, "dbo.sp_GetCustomerUserRegistration");

            return result;
        }

        public CustomerUserRegistrationPreVerification UpdateRegistration(Guid registrationId, string verificationCode)
        {
            CustomerUserRegistrationPreVerification result = null;

            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();

            PrepareParameters(changes, parameters, "PROCESS_ID", registrationId, true);
            PrepareParameters(changes, parameters, "VERIFICATION_CODE", verificationCode, true);

            result = GetSingle<CustomerUserRegistrationPreVerification>(parameters, "dbo.sp_UpdateCustomerUserRegistration");

            return result;
        }

        /// <summary>
        /// Creates a customer user during registration
        /// </summary>
        public void CreateCustomerUser(CustomerUser customerUser)
        {
            this.CheckCustomerUserExistence(customerUser.MOBILE_PHONE, customerUser.EMAIL, customerUser.SOCIAL_CARD_NUMBER);

            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();

            PrepareParameters(changes, parameters, "FIRST_NAME",         customerUser.FIRST_NAME,         true);
            PrepareParameters(changes, parameters, "LAST_NAME",          customerUser.LAST_NAME,          true);
            PrepareParameters(changes, parameters, "SOCIAL_CARD_NUMBER", customerUser.SOCIAL_CARD_NUMBER, true);
            PrepareParameters(changes, parameters, "MOBILE_PHONE",       customerUser.MOBILE_PHONE,       true);
            PrepareParameters(changes, parameters, "EMAIL",              customerUser.EMAIL,              false);
            
            parameters.Add("HASH", customerUser.HASH);
            parameters.Add("OPERATION_DETAILS", GenerateOperationDetailsString(changes));
            parameters.Add("APPLICATION_USER_ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
            Execute(parameters, "dbo.sp_CreateCustomerUser");
        }

        /// <summary>
        /// Modifies a customer user from personal data page
        /// </summary>
        public void ModifyCustomerUser(CustomerUser customerUser, int userID)
        {
            this.CheckCustomerUserExistence(customerUser.MOBILE_PHONE, customerUser.EMAIL, customerUser.SOCIAL_CARD_NUMBER, userID);

            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();
            PrepareParameters(changes, parameters, "APPLICATION_USER_ID",       userID, true);
            PrepareParameters(changes, parameters, "FIRST_NAME_EN",             customerUser.FIRST_NAME_EN, true);
            PrepareParameters(changes, parameters, "LAST_NAME_EN",              customerUser.LAST_NAME_EN, true);
            PrepareParameters(changes, parameters, "FIRST_NAME_AM",             customerUser.FIRST_NAME_AM, true);
            PrepareParameters(changes, parameters, "LAST_NAME_AM",              customerUser.LAST_NAME_AM, true);
            PrepareParameters(changes, parameters, "PATRONYMIC_NAME_AM",        customerUser.PATRONYMIC_NAME_AM, true);
            PrepareParameters(changes, parameters, "BIRTH_DATE",                customerUser.BIRTH_DATE);
            PrepareParameters(changes, parameters, "BIRTH_PLACE_CODE",          customerUser.BIRTH_PLACE_CODE);
            PrepareParameters(changes, parameters, "CITIZENSHIP_CODE",          customerUser.CITIZENSHIP_CODE);
            PrepareParameters(changes, parameters, "MOBILE_PHONE",              customerUser.MOBILE_PHONE, true);
            PrepareParameters(changes, parameters, "EMAIL",                     customerUser.EMAIL, true);
            PrepareParameters(changes, parameters, "SOCIAL_CARD_NUMBER",        customerUser.SOCIAL_CARD_NUMBER, true);
            PrepareParameters(changes, parameters, "DOCUMENT_TYPE_CODE",        customerUser.DOCUMENT_TYPE_CODE);
            PrepareParameters(changes, parameters, "DOCUMENT_NUMBER",           customerUser.DOCUMENT_NUMBER);
            PrepareParameters(changes, parameters, "DOCUMENT_GIVEN_DATE",       customerUser.DOCUMENT_GIVEN_DATE);
            PrepareParameters(changes, parameters, "DOCUMENT_EXPIRY_DATE",      customerUser.DOCUMENT_EXPIRY_DATE);
            PrepareParameters(changes, parameters, "DOCUMENT_GIVEN_BY",         customerUser.DOCUMENT_GIVEN_BY);
            PrepareParameters(changes, parameters, "REGISTRATION_COUNTRY_CODE", customerUser.REGISTRATION_COUNTRY_CODE);
            PrepareParameters(changes, parameters, "REGISTRATION_STATE_CODE",   customerUser.REGISTRATION_STATE_CODE);
            PrepareParameters(changes, parameters, "REGISTRATION_CITY_CODE",    customerUser.REGISTRATION_CITY_CODE);
            PrepareParameters(changes, parameters, "REGISTRATION_STREET",       customerUser.REGISTRATION_STREET);
            PrepareParameters(changes, parameters, "REGISTRATION_BUILDNUM",     customerUser.REGISTRATION_BUILDNUM);
            PrepareParameters(changes, parameters, "REGISTRATION_APARTMENT",    customerUser.REGISTRATION_APARTMENT);
            PrepareParameters(changes, parameters, "CURRENT_COUNTRY_CODE",      customerUser.CURRENT_COUNTRY_CODE);
            PrepareParameters(changes, parameters, "CURRENT_STATE_CODE",        customerUser.CURRENT_STATE_CODE);
            PrepareParameters(changes, parameters, "CURRENT_CITY_CODE",         customerUser.CURRENT_CITY_CODE);
            PrepareParameters(changes, parameters, "CURRENT_STREET",            customerUser.CURRENT_STREET);
            PrepareParameters(changes, parameters, "CURRENT_BUILDNUM",          customerUser.CURRENT_BUILDNUM);
            PrepareParameters(changes, parameters, "CURRENT_APARTMENT",         customerUser.CURRENT_APARTMENT);

            parameters.Add("OPERATION_DETAILS", GenerateOperationDetailsString(changes));
            Execute(parameters, "dbo.sp_ModifyCustomerUser");
        }

        public bool CheckCustomerUserExistenceByParameter(string parameterName, string parameterValue, int? userID)
        {
            string procedureName = string.Empty;
            switch (parameterName)
            {
                case "MOBILE_PHONE":
                    procedureName = "sp_CheckCustomerUserExistenceByMobilePhone";
                    break;
                case "SOCIAL_CARD_NUMBER":
                    procedureName = "sp_CheckCustomerUserExistenceBySocialCard";
                    break;
                case "EMAIL":
                    procedureName = "sp_CheckCustomerUserExistenceByEmail";
                    break;
            }
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(parameterName, parameterValue);
            List<int?> customerUser = GetList<int?>(parameters, string.Format("dbo.{0}", procedureName)).ToList();
            if (userID.HasValue &&
                (customerUser.Count > 1 || (customerUser.Count == 1 && !customerUser.Contains(userID)))) // modify customer user
            {
                return true;
            }
            else if (!userID.HasValue && customerUser.Count > 0) // register customer user
            {
                return true;
            }
            return false;
        }

        private void CheckCustomerUserExistence(string mobilePhone, string email, string socialCard, int? userID = null)
        {
            if (this.CheckCustomerUserExistenceByParameter("SOCIAL_CARD_NUMBER", socialCard, userID))
            {
                throw new ApplicationException("ERR-0042", "Նման ՀԾՀ / սոցիալական քարտի համարով առկա է օգտատեր։");
            }
            //else if (this.CheckCustomerUserExistenceByParameter("MOBILE_PHONE", mobilePhone, userID))
            //{
            //    throw new ApplicationException("ERR-0040", "Նման բջջային հեռախոսահամարով առկա է օգտատեր։");
            //}
            //else if (this.CheckCustomerUserExistenceByParameter("EMAIL", email, userID))
            //{
            //    throw new ApplicationException("ERR-0041", "Նման էլեկտրոնային հասցեով առկա է օգտատեր։");
            //}
        }

        public CustomerUser GetCustomerUser(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("APPLICATION_USER_ID", id);
            CustomerUser customerUser = GetSingle<CustomerUser>(parameters, "dbo.sp_GetCustomerUser");
            return customerUser;
        }

        public CustomerUser AuthenticateCustomerUser(string login, string hash)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("LOGIN", login);
            parameters.Add("HASH", hash);
            CustomerUser customerUser = GetSingle<CustomerUser>(parameters, "dbo.sp_AuthenticateApplicationUser");
            return customerUser;
        }

        public void ChangeCustomerUserPassword(string login, string hash)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("LOGIN", login);
            parameters.Add("HASH", hash);
            parameters.Add("PASSWORD_EXPIRY_DATE", GenerateUserPasswordExpiryDate());
            Execute(parameters, "dbo.sp_ChangeApplicationUserPassword");
        }

        public void ResetCustomerUserPassword(string login, string smsCode, Guid processId, string passwordHash)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("PROCESS_ID", processId);
            parameters.Add("MOBILE_PHONE", login);
            parameters.Add("VERIFICATION_CODE_HASH", smsCode);
            parameters.Add("PASSWORD_HASH", passwordHash);
            Execute(parameters, "dbo.sp_UpdateCustomerUserPassword");
        }

        public void StartCustomerUserPasswordReset(string phone, Guid processId, string smsCode)
        {
            Dictionary<string, string> changes = new Dictionary<string, string>();
            DynamicParameters parameters = new DynamicParameters();

            PrepareParameters(changes, parameters, "PROCESS_ID", processId, true);
            PrepareParameters(changes, parameters, "LOGIN", phone, true);
            PrepareParameters(changes, parameters, "HASH", smsCode, true);

            Execute(parameters, "dbo.sp_StartCustomerUserPasswordReset");
        }

        public IEnumerable<string> GetClientByDocument(string documentCode, string documentType, string socialCardCode)
        {
            string bankDB = GetSetting("BANK_SERVER_DATABASE");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("PassportCode", documentCode);
            parameters.Add("PassportType", documentType);
            parameters.Add("SocialCardCode", socialCardCode);
            return GetList<string>(parameters, string.Format("{0}dbo.ol0sp_GetClientByDocument", bankDB));
        }
    }
}
