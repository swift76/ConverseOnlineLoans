using System;
using System.Collections.Generic;
using System.Transactions;
using System.Xml;

namespace IntelART.CLRServices
{
    public class ACRAQuery
    {
        public void GetResponse(DataHelper dataAccess, ACRARequest entity, string url, string sessionID, string cookie)
        {
            ACRAResponse response = new ACRAResponse();

            dataAccess.SaveACRATryCount(entity.ID);

            string responseText = dataAccess.GetCachedACRAResponse(entity.SocialCardNumber);
            if (!string.IsNullOrEmpty(responseText))
                ParseResponseText(dataAccess, entity.ID, responseText, ref response);
            else
            {
                Dictionary<string, string> parametersCheck = new Dictionary<string, string>();
                parametersCheck.Add("AppNumber", ServiceHelper.DecorateValue(ServiceHelper.GenerateUniqueID(15)));
                parametersCheck.Add("DateTime", ServiceHelper.DecorateValue(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                parametersCheck.Add("SID", sessionID);

                Dictionary<string, string> participientParametersCheck = new Dictionary<string, string>();
                participientParametersCheck.Add("FirstName", ServiceHelper.DecorateValue(entity.FirstName));
                participientParametersCheck.Add("LastName", ServiceHelper.DecorateValue(entity.LastName));
                participientParametersCheck.Add("DateofBirth", ServiceHelper.DecorateValue(entity.BirthDate.ToString("dd-MM-yyyy")));
                participientParametersCheck.Add("PassportNumber", entity.PassportNumber);
                participientParametersCheck.Add("IdCardNumber", entity.IDCardNumber);
                participientParametersCheck.Add("SocCardNumber", entity.SocialCardNumber);

                responseText = ServiceHelper.GetACRAResponseText(url, "Bank_PersonOrg_List", ref cookie, parametersCheck, participientParametersCheck);
                XmlDocument document = ServiceHelper.CheckACRAResponse(responseText);

                if (ServiceHelper.GetNodeValue(document, "/ROWDATA[@*]/PARTICIPIENT[@*]/ThePresenceData") == "2")
                {
                    dataAccess.AutomaticallyRefuseApplication(entity.ID, "Վարկային զեկույցն արգելափակված է");
                    return;
                }

                List<string> persons = GetPersonList(document);

                if (persons.Count > 0)
                {
                    Dictionary<string, string> parametersQuery = new Dictionary<string, string>();
                    parametersQuery.Add("AppNumber", ServiceHelper.DecorateValue(ServiceHelper.GenerateUniqueID(15)));
                    parametersQuery.Add("DateTime", ServiceHelper.DecorateValue(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                    parametersQuery.Add("SID", sessionID);
                    parametersQuery.Add("ReportType", ServiceHelper.DecorateValue(dataAccess.GetSettingValue("ACRA_REPORT_TYPE")));

                    responseText = ServiceHelper.GetACRAResponseText(url, "Bank_Application_LOAN_PP", ref cookie, parametersQuery, null, persons);
                    ParseResponseText(dataAccess, entity.ID, responseText, ref response);
                }
            }
            using (TransactionScope transScope = new TransactionScope())
            {
                if (dataAccess.LockApplicationByID(entity.ID, 2))
                    dataAccess.SaveACRAQueryResult(entity.ID, response, responseText);
                transScope.Complete();
            }
        }

        private void ParseResponseText(DataHelper dataAccess, Guid id, string responseText, ref ACRAResponse response)
        {
            XmlDocument document = ServiceHelper.CheckACRAResponse(responseText);

            string presence = ServiceHelper.GetNodeValue(document, "/ROWDATA[@*]/PARTICIPIENT[@*]/ThePresenceData");
            if (presence == "2")
                dataAccess.AutomaticallyRefuseApplication(id, "Վարկային զեկույցն արգելափակված է");
            else
            {
                if (presence == "1")
                {
                    DateTime dateCurrent = dataAccess.GetServerDate();
                    int currentYear = dateCurrent.Year;
                    int currentMonth = dateCurrent.Month;
                    ParseLoanGuarantee(document, false, currentYear, currentMonth, response.Details);
                    ParseLoanGuarantee(document, true, currentYear, currentMonth, response.Details);
                }
                ParseQuery(document, response.Queries);
                response.FicoScore = ServiceHelper.GetNodeValue(document, "/ROWDATA[@*]/PARTICIPIENT[@*]/Score/FICOScore");
            }
        }

        private static List<string> GetPersonList(XmlDocument document)
        {
            List<string> result = new List<string>();
            XmlNodeList listPerson = document.SelectNodes("/ROWDATA[@*]/PARTICIPIENT[@*]/Person");
            foreach (XmlNode node in listPerson)
                result.Add(ServiceHelper.RetrieveValue(node.SelectSingleNode("Identificator").InnerXml));
            return result;
        }

        private static void ParseQuery(XmlDocument document, List<ACRAQueryResultQueries> queries)
        {
            XmlNodeList list = document.SelectNodes("/ROWDATA[@*]/PARTICIPIENT[@*]/Requests/Request");
            foreach (XmlNode node in list)
                queries.Add(new ACRAQueryResultQueries()
                {
                    DATE = ServiceHelper.GetACRADateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode("Date").InnerXml)),
                    BANK_NAME = ServiceHelper.RetrieveValue(node.SelectSingleNode("BankName").InnerXml),
                    REASON = ServiceHelper.RetrieveValue(node.SelectSingleNode("Reason").InnerXml)
                });
        }

        private static void ParseLoanGuarantee(XmlDocument document, bool isGuarantee, int currentYear, int currentMonth, List<ACRAQueryResultDetails> details)
        {
            string prefixLG = isGuarantee ? "Guarantee" : "Loan";
            XmlNodeList list = document.SelectNodes(string.Format("/ROWDATA[@*]/PARTICIPIENT[@*]/{0}s/{0}", prefixLG));
            foreach (XmlNode node in list)
            {
                DateTime dateFrom = ServiceHelper.GetACRADateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode("CreditStart").InnerXml), DateTime.MinValue);
                DateTime dateTo = ServiceHelper.GetACRADateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode("CloseDate").InnerXml), DateTime.MaxValue);

                int dueDays1 = 0, dueDays2 = 0, dueDays3 = 0, dueDays4 = 0, dueDays5 = 0;

                DateTime? dateLastPayment = ServiceHelper.GetACRANullableDateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode(string.Format("{0}LastPaymentDate", prefixLG)).InnerXml));

                if (node.SelectSingleNode("OutstandingDaysCount") != null)
                {
                    dueDays1 += GetDueDaysByYear(node, currentYear, currentMonth, 1, dateLastPayment, dateTo);
                    dueDays2 = dueDays1 + GetDueDaysByYear(node, currentYear, currentMonth, 2, dateLastPayment, dateTo);
                    dueDays3 = dueDays2 + GetDueDaysByYear(node, currentYear, currentMonth, 3, dateLastPayment, dateTo);
                    dueDays4 = dueDays3 + GetDueDaysByYear(node, currentYear, currentMonth, 4, dateLastPayment, dateTo);
                    dueDays5 = dueDays4 + GetDueDaysByYear(node, currentYear, currentMonth, 5, dateLastPayment, dateTo);
                }

                details.Add(new ACRAQueryResultDetails()
                {
                    STATUS = ServiceHelper.RetrieveValue(node.SelectSingleNode("CreditStatus").InnerXml),
                    FROM_DATE = dateFrom,
                    TO_DATE = dateTo,
                    TYPE = ServiceHelper.RetrieveValue(node.SelectSingleNode("LiabilityKind").InnerXml),
                    CUR = ServiceHelper.RetrieveValue(node.SelectSingleNode("Currency").InnerXml),
                    CONTRACT_AMOUNT = decimal.Parse(ServiceHelper.RetrieveValue(node.SelectSingleNode("Amount").InnerXml)),
                    DEBT = decimal.Parse(ServiceHelper.RetrieveValue(node.SelectSingleNode("Balance").InnerXml)),
                    PAST_DUE_DATE = ServiceHelper.GetACRANullableDateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode("OutstandingDate").InnerXml)),
                    RISK = ServiceHelper.RetrieveValue(node.SelectSingleNode(string.Format("The{0}Class", prefixLG)).InnerXml),
                    CLASSIFICATION_DATE = ServiceHelper.GetACRANullableDateValue(ServiceHelper.RetrieveValue(node.SelectSingleNode("LastClassificationDate").InnerXml)),
                    INTEREST_RATE = decimal.Parse(ServiceHelper.RetrieveValue(node.SelectSingleNode("Interest").InnerXml)),
                    PLEDGE = ServiceHelper.RetrieveValue(node.SelectSingleNode("PledgeSubject").InnerXml),
                    PLEDGE_AMOUNT = ServiceHelper.RetrieveOptionalAmount(node, "CollateralAmount"),
                    OUTSTANDING_AMOUNT = ServiceHelper.RetrieveOptionalAmount(node, "AmountOverdue"),
                    OUTSTANDING_PERCENT = ServiceHelper.RetrieveOptionalAmount(node, "OutstandingPercent"),
                    BANK_NAME = ServiceHelper.RetrieveValue(node.SelectSingleNode("SourceName").InnerXml).ToUpper(),
                    IS_GUARANTEE = isGuarantee,
                    DUE_DAYS_1 = dueDays1,
                    DUE_DAYS_2 = dueDays2,
                    DUE_DAYS_3 = dueDays3,
                    DUE_DAYS_4 = dueDays4,
                    DUE_DAYS_5 = dueDays5,
                    LAST_REPAYMENT_DATE = dateLastPayment
                });
            }
        }

        private static int GetDueDaysByYear(XmlNode node, int currentYear, int currentMonth, int shift,DateTime? dateLastPayment, DateTime dateTo)
        {
            int result = 0;

            XmlNodeList listDueYear = node.SelectNodes("OutstandingDaysCount/Year[@*]");
            foreach (XmlNode nodeDueYear in listDueYear)
            {
                XmlAttributeCollection colYear = nodeDueYear.Attributes;
                foreach (XmlAttribute attrYear in colYear)
                {
                    if (attrYear.Name.ToLower() == "name")
                    {
                        int valueYear = int.Parse(attrYear.Value);
                        if ((valueYear == (currentYear - shift)) || (valueYear == (currentYear - shift + 1)))
                        {
                            XmlNodeList listDueMonth = nodeDueYear.SelectNodes("Month");
                            foreach (XmlNode nodeDueMonth in listDueMonth)
                            {
                                XmlAttributeCollection colMonth = nodeDueMonth.Attributes;
                                foreach (XmlAttribute attrMonth in colMonth)
                                    if (attrYear.Name.ToLower() == "name")
                                    {
                                        int valueMonth = int.Parse(attrMonth.Value);
                                        if (IsDateInYear(currentYear, currentMonth, shift, valueYear, valueMonth))
                                        {
                                            int days = 0;
                                            if (int.TryParse(ServiceHelper.RetrieveValue(nodeDueMonth.InnerXml), out days))
                                                result += days;
                                        }
                                    }
                            }
                        }
                    }
                }
            }

            if (dateLastPayment.HasValue)
            {
                int overdueDays = (dateTo - dateLastPayment.Value).Days;
                if (overdueDays < 0)
                {
                    if (IsDateInYear(currentYear, currentMonth, shift, dateLastPayment.Value.Year, dateLastPayment.Value.Month))
                        result -= overdueDays;
                }
            }

            return result;
        }

        private static bool IsDateInYear(int currentYear, int currentMonth, int shift, int valueYear, int valueMonth)
        {
            return ((valueYear == (currentYear - shift) && valueMonth > currentMonth) || valueYear == (currentYear - shift + 1) && valueMonth <= currentMonth);
        }
    }
}
