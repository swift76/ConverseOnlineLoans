using System;
using System.Collections.Generic;
using System.Transactions;
using System.Xml;

namespace IntelART.CLRServices
{
    public class NORQQuery
    {
        public void GetResponse(DataHelper dataAccess, NORQRequest entity, string url, string sessionID)
        {
            dataAccess.SaveNORQTryCount(entity.ID);

            string responseText = dataAccess.GetCachedNORQResponse(entity.SocialCardNumber);
            if (string.IsNullOrEmpty(responseText))
            {
                Dictionary<string, string> parametersQuery = new Dictionary<string, string>();
                parametersQuery.Add("argGuid", sessionID);
                parametersQuery.Add("argSocCard", entity.SocialCardNumber);
                parametersQuery.Add("argIsWorkData", "1");
                responseText = ServiceHelper.GetNORQResponseText(url, "GetUserData_v2018", parametersQuery);
            }
            XmlDocument document = ServiceHelper.CheckNORQResponse(responseText);

            NORQResponse response = new NORQResponse();
            response.FirstName = FormatName(ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Firstname"));
            response.LastName = FormatName(ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Lastname"));

            string entityFirstName = FormatName(entity.FirstName);
            string entityLastName = FormatName(entity.LastName);

            DateTime dateCurrent = dataAccess.GetServerDate();

            response.BirthDate = ServiceHelper.GetNORQDateValue(ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Birthdate"), dateCurrent);
            if (response.FirstName == string.Empty || response.LastName == string.Empty)
            {
                dataAccess.AutomaticallyRefuseApplication(entity.ID, "Սխալ փաստաթղթի տվյալներ");
            }
            else if ((entity.CustomerUserID != 0 && (entityFirstName != response.FirstName || entityLastName != response.LastName)) || (dataAccess.GetSettingValue("NORQ_CHECK_BIRTH_DATE") == "1" && entity.BirthDate != response.BirthDate))
            {
                dataAccess.AutomaticallyRefuseApplication(entity.ID, "Տվյալների անհամապատասխանություն");
            }
            else
            {
                response.MiddleName = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Middlename").ToUpper();
                response.Gender = (ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Gender") == "2");
                response.District = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Region").ToUpper();
                response.Community = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Community").ToUpper();
                response.Street = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Street").ToUpper();
                response.Building = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Building").ToUpper();
                response.Apartment = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Apartment").ToUpper();

                response.NonBiometricPassport = GetDocumentData(document, "Passport", dateCurrent);
                response.BiometricPassport = GetDocumentData(document, "Biometric", dateCurrent);
                response.IDCard = GetDocumentData(document, "IdCard", dateCurrent);

                if (!response.NonBiometricPassport.IsDocumentValid()
                    && !response.BiometricPassport.IsDocumentValid()
                    && !response.IDCard.IsDocumentValid())
                {
                    dataAccess.AutomaticallyRefuseApplication(entity.ID, "Անվավեր փաստաթուղթ");
                }
                else if (response.NonBiometricPassport.Number != entity.DocumentNumber
                    && response.BiometricPassport.Number != entity.DocumentNumber
                    && response.IDCard.Number != entity.DocumentNumber)
                {
                    dataAccess.AutomaticallyRefuseApplication(entity.ID, "Սխալ փաստաթղթի տվյալներ");
                }
                else
                {
                    response.SocialCardNumber = ServiceHelper.GetNORQNodeValue(document, "GetUserData", "argPrivateData/Soccard");

                    XmlNodeList listWork = document.SelectNodes("/Envelope/Body/GetUserDataResponse/argWorkData/WorkData");
                    int months = int.Parse(dataAccess.GetSettingValue("NORQ_WORK_MONTH"));
                    response.Salary = GetFee(listWork, dateCurrent.Date, months);
                    if (response.Salary == 0 && dataAccess.GetSettingValue("TAKE_NORQ_PREVIOUS_YEAR") == "1")
                        response.Salary = GetFee(listWork, new DateTime(dateCurrent.Year - 1, 12, 30), months);

                    using (TransactionScope transScope = new TransactionScope())
                    {
                        if (dataAccess.LockApplicationByID(entity.ID, 1))
                            dataAccess.SaveNORQQueryResult(entity.ID, response, responseText);
                        transScope.Complete();
                    }
                }
            }
        }

        private decimal GetFee(XmlNodeList listWork, DateTime dateCurrent, int months)
        {
            decimal fee = 0;
            DateTime dateInitial = dateCurrent.AddMonths(-months);
            foreach (XmlNode node in listWork)
                if (ServiceHelper.GetNORQDateValue(node.SelectSingleNode("ExpiryDate").InnerXml, dateCurrent) >= dateCurrent
                    && ServiceHelper.GetNORQDateValue(node.SelectSingleNode("Pajman_data").InnerXml, dateInitial) <= dateInitial)
                    fee += decimal.Parse(node.SelectSingleNode("Salary").InnerXml);
            return fee;
        }

        private DocumentData GetDocumentData(XmlDocument document, string documentName, DateTime dateCurrent)
        {
            DocumentData result = new DocumentData();
            result.Number = ServiceHelper.GetNORQNodeValue(document, "GetUserData", string.Format("argPrivateData/{0}", documentName));
            result.IssueDate = ServiceHelper.GetNORQDateValue(ServiceHelper.GetNORQNodeValue(document, "GetUserData", string.Format("argPrivateData/{0}Date", documentName)), dateCurrent);
            result.ExpiryDate = ServiceHelper.GetNORQDateValue(ServiceHelper.GetNORQNodeValue(document, "GetUserData", string.Format("argPrivateData/{0}Vdate", documentName)), dateCurrent);
            result.IssuedBy = ServiceHelper.GetNORQNodeValue(document, "GetUserData", string.Format("argPrivateData/{0}Where", documentName));
            return result;
        }

        private string FormatName(string source)
        {
            string result = source.Trim().ToUpper();
            result = result.Replace("և", "ԵՎ");
            result = result.Replace("ԵՒ", "ԵՎ");
            return result;
        }
    }
}
