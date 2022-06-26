using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IntelART.CLRServices
{
    public class DataHelper : IDisposable
    {
        public ServiceConfig GetServiceConfig(string serviceCode)
        {
            ServiceConfig result = new ServiceConfig();
            using (SqlCommand cmd = new SqlCommand(string.Format("sp_Get{0}ConfigData", serviceCode), ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result.URL = rdr.GetString(0);
                        result.UserName = rdr.GetString(1);
                        result.UserPassword = rdr.GetString(2);
                    }
                }
            }
            return result;
        }

        public string GetSettingValue(string settingCode)
        {
            string result = string.Empty;
            using (SqlCommand cmd = new SqlCommand("sp_GetSettings", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@CODE", SqlDbType.VarChar, 30)).Value = settingCode;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = rdr.GetString(1);
                    }
                }
            }
            return result;
        }

        public DateTime GetServerDate()
        {
            DateTime result = DateTime.Now;
            using (SqlCommand cmd = new SqlCommand("select convert(date,getdate())", ActiveConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = CommandTimeoutInterval;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        result = rdr.GetDateTime(0);
                }
            }
            return result.Date;
        }

        public void AutomaticallyRefuseApplication(Guid id, string reason)
        {
            using (SqlCommand cmd = new SqlCommand("sp_AutomaticallyRefuseApplication", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.Parameters.Add(new SqlParameter("@REFUSAL_REASON", SqlDbType.NVarChar, 100)).Value = reason;
                cmd.ExecuteScalar();
            }
        }

        public void LogError(string operation, string errorMessage, Guid? id = null)
        {
            using (SqlCommand cmd = new SqlCommand("sp_LogScoringError", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                if (id.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@APPLICATION_ID", SqlDbType.UniqueIdentifier)).Value = id.Value;
                cmd.Parameters.Add(new SqlParameter("@OPERATION", SqlDbType.VarChar, 200)).Value = operation;
                cmd.Parameters.Add(new SqlParameter("@ERROR_MESSAGE", SqlDbType.NVarChar, -1)).Value = errorMessage;
                cmd.ExecuteScalar();
            }
        }

        public void SaveNORQQueryResult(Guid id, NORQResponse result, string responseText)
        {
            using (SqlCommand cmd = new SqlCommand("sp_SaveNORQQueryResult", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@APPLICATION_ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.Parameters.Add(new SqlParameter("@FIRST_NAME", SqlDbType.NVarChar, 40)).Value = result.FirstName;
                cmd.Parameters.Add(new SqlParameter("@LAST_NAME", SqlDbType.NVarChar, 40)).Value = result.LastName;
                cmd.Parameters.Add(new SqlParameter("@PATRONYMIC_NAME", SqlDbType.NVarChar, 40)).Value = result.MiddleName;
                cmd.Parameters.Add(new SqlParameter("@BIRTH_DATE", SqlDbType.Date)).Value = result.BirthDate;
                cmd.Parameters.Add(new SqlParameter("@GENDER", SqlDbType.Bit)).Value = result.Gender;
                cmd.Parameters.Add(new SqlParameter("@DISTRICT", SqlDbType.NVarChar, 40)).Value = result.District;
                cmd.Parameters.Add(new SqlParameter("@COMMUNITY", SqlDbType.NVarChar, 40)).Value = result.Community;
                cmd.Parameters.Add(new SqlParameter("@STREET", SqlDbType.NVarChar, 100)).Value = result.Street;
                cmd.Parameters.Add(new SqlParameter("@BUILDING", SqlDbType.NVarChar, 40)).Value = result.Building;
                cmd.Parameters.Add(new SqlParameter("@APARTMENT", SqlDbType.NVarChar, 40)).Value = result.Apartment;
                cmd.Parameters.Add(new SqlParameter("@FEE", SqlDbType.Money)).Value = result.Salary;
                cmd.Parameters.Add(new SqlParameter("@NON_BIOMETRIC_PASSPORT_NUMBER", SqlDbType.Char, 9)).Value = result.NonBiometricPassport.Number;
                cmd.Parameters.Add(new SqlParameter("@NON_BIOMETRIC_PASSPORT_ISSUE_DATE", SqlDbType.Date)).Value = result.NonBiometricPassport.IssueDate;
                cmd.Parameters.Add(new SqlParameter("@NON_BIOMETRIC_PASSPORT_EXPIRY_DATE", SqlDbType.Date)).Value = result.NonBiometricPassport.ExpiryDate;
                cmd.Parameters.Add(new SqlParameter("@NON_BIOMETRIC_PASSPORT_ISSUED_BY", SqlDbType.Char, 3)).Value = result.NonBiometricPassport.IssuedBy;
                cmd.Parameters.Add(new SqlParameter("@BIOMETRIC_PASSPORT_NUMBER", SqlDbType.Char, 9)).Value = result.BiometricPassport.Number;
                cmd.Parameters.Add(new SqlParameter("@BIOMETRIC_PASSPORT_ISSUE_DATE", SqlDbType.Date)).Value = result.BiometricPassport.IssueDate;
                cmd.Parameters.Add(new SqlParameter("@BIOMETRIC_PASSPORT_EXPIRY_DATE", SqlDbType.Date)).Value = result.BiometricPassport.ExpiryDate;
                cmd.Parameters.Add(new SqlParameter("@BIOMETRIC_PASSPORT_ISSUED_BY", SqlDbType.Char, 3)).Value = result.BiometricPassport.IssuedBy;
                cmd.Parameters.Add(new SqlParameter("@ID_CARD_NUMBER", SqlDbType.Char, 9)).Value = result.IDCard.Number;
                cmd.Parameters.Add(new SqlParameter("@ID_CARD_ISSUE_DATE", SqlDbType.Date)).Value = result.IDCard.IssueDate;
                cmd.Parameters.Add(new SqlParameter("@ID_CARD_EXPIRY_DATE", SqlDbType.Date)).Value = result.IDCard.ExpiryDate;
                cmd.Parameters.Add(new SqlParameter("@ID_CARD_ISSUED_BY", SqlDbType.Char, 3)).Value = result.IDCard.IssuedBy;
                cmd.Parameters.Add(new SqlParameter("@SOCIAL_CARD_NUMBER", SqlDbType.Char, 10)).Value = result.SocialCardNumber;
                cmd.Parameters.Add(new SqlParameter("@RESPONSE_XML", SqlDbType.NVarChar, -1)).Value = ServiceHelper.GetFormattedXML(responseText);
                cmd.ExecuteScalar();
            }
        }

        public void SaveACRAQueryResult(Guid id, ACRAResponse result, string responseText)
        {
            using (SqlCommand cmd = new SqlCommand("sp_SaveACRAQueryResult", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@APPLICATION_ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.Parameters.Add(new SqlParameter("@FICO_SCORE", SqlDbType.Char, 3)).Value = result.FicoScore;
                cmd.Parameters.Add(new SqlParameter("@RESPONSE_XML", SqlDbType.NVarChar, -1)).Value = ServiceHelper.GetFormattedXML(responseText);

                DataTable tableDetails = new DataTable("ACRAQueryResultDetails");
                tableDetails.Columns.Add("STATUS", typeof(string));
                tableDetails.Columns.Add("FROM_DATE", typeof(DateTime));
                tableDetails.Columns.Add("TO_DATE", typeof(DateTime));
                tableDetails.Columns.Add("TYPE", typeof(string));
                tableDetails.Columns.Add("CUR", typeof(string));
                tableDetails.Columns.Add("CONTRACT_AMOUNT", typeof(decimal));
                tableDetails.Columns.Add("DEBT", typeof(decimal));
                tableDetails.Columns.Add("PAST_DUE_DATE", typeof(DateTime));
                tableDetails.Columns.Add("RISK", typeof(string));
                tableDetails.Columns.Add("CLASSIFICATION_DATE", typeof(DateTime));
                tableDetails.Columns.Add("INTEREST_RATE", typeof(decimal));
                tableDetails.Columns.Add("PLEDGE", typeof(string));
                tableDetails.Columns.Add("PLEDGE_AMOUNT", typeof(decimal));
                tableDetails.Columns.Add("OUTSTANDING_AMOUNT", typeof(decimal));
                tableDetails.Columns.Add("OUTSTANDING_PERCENT", typeof(decimal));
                tableDetails.Columns.Add("BANK_NAME", typeof(string));
                tableDetails.Columns.Add("IS_GUARANTEE", typeof(bool));
                tableDetails.Columns.Add("DUE_DAYS_1", typeof(int));
                tableDetails.Columns.Add("DUE_DAYS_2", typeof(int));
                tableDetails.Columns.Add("DUE_DAYS_3", typeof(int));
                tableDetails.Columns.Add("DUE_DAYS_4", typeof(int));
                tableDetails.Columns.Add("DUE_DAYS_5", typeof(int));
                tableDetails.Columns.Add("LAST_REPAYMENT_DATE", typeof(DateTime));
                tableDetails.Columns.Add("SUM_OVERDUE_DAYS_Y1_Y1", typeof(int));
                tableDetails.Columns.Add("MAX_OVERDUE_DAYS_Y1_Y1", typeof(int));
                tableDetails.Columns.Add("SUM_OVERDUE_DAYS_Y1_Y2", typeof(int));
                tableDetails.Columns.Add("MAX_OVERDUE_DAYS_Y1_Y2", typeof(int));
                tableDetails.Columns.Add("SUM_OVERDUE_DAYS_Y1_Y3", typeof(int));
                tableDetails.Columns.Add("MAX_OVERDUE_DAYS_Y1_Y3", typeof(int));
                tableDetails.Columns.Add("SUM_OVERDUE_DAYS_Y1_Y4", typeof(int));
                tableDetails.Columns.Add("MAX_OVERDUE_DAYS_Y1_Y4", typeof(int));
                tableDetails.Columns.Add("SUM_OVERDUE_DAYS_Y1_Y5", typeof(int));
                tableDetails.Columns.Add("MAX_OVERDUE_DAYS_Y1_Y5", typeof(int));
                for (int i = 0; i < result.Details.Count; i++)
                    if (result.Details[i].CUR.Length == 3)
                        tableDetails.Rows.Add(result.Details[i].STATUS, result.Details[i].FROM_DATE, result.Details[i].TO_DATE, result.Details[i].TYPE
                        , result.Details[i].CUR, result.Details[i].CONTRACT_AMOUNT, result.Details[i].DEBT, result.Details[i].PAST_DUE_DATE, result.Details[i].RISK, result.Details[i].CLASSIFICATION_DATE
                        , result.Details[i].INTEREST_RATE, result.Details[i].PLEDGE, result.Details[i].PLEDGE_AMOUNT, result.Details[i].OUTSTANDING_AMOUNT, result.Details[i].OUTSTANDING_PERCENT
                        , result.Details[i].BANK_NAME, result.Details[i].IS_GUARANTEE, result.Details[i].DUE_DAYS_1, result.Details[i].DUE_DAYS_2, result.Details[i].DUE_DAYS_3, result.Details[i].DUE_DAYS_4, result.Details[i].DUE_DAYS_5, result.Details[i].LAST_REPAYMENT_DATE
                        , result.Details[i].SUM_OVERDUE_DAYS_Y1_Y1, result.Details[i].MAX_OVERDUE_DAYS_Y1_Y1, result.Details[i].SUM_OVERDUE_DAYS_Y1_Y2, result.Details[i].MAX_OVERDUE_DAYS_Y1_Y2, result.Details[i].SUM_OVERDUE_DAYS_Y1_Y3, result.Details[i].MAX_OVERDUE_DAYS_Y1_Y3, result.Details[i].SUM_OVERDUE_DAYS_Y1_Y4, result.Details[i].MAX_OVERDUE_DAYS_Y1_Y4, result.Details[i].SUM_OVERDUE_DAYS_Y1_Y5, result.Details[i].MAX_OVERDUE_DAYS_Y1_Y5);
                cmd.Parameters.AddWithValue("@DETAILS", tableDetails).SqlDbType = SqlDbType.Structured;

                DataTable tableQueries = new DataTable("ACRAQueryResultQueries");
                tableQueries.Columns.Add("DATE", typeof(DateTime));
                tableQueries.Columns.Add("BANK_NAME", typeof(string));
                tableQueries.Columns.Add("REASON", typeof(string));
                for (int i = 0; i < result.Queries.Count; i++)
                    tableQueries.Rows.Add(result.Queries[i].DATE, result.Queries[i].BANK_NAME, result.Queries[i].REASON);
                cmd.Parameters.AddWithValue("@QUERIES", tableQueries).SqlDbType = SqlDbType.Structured;

                cmd.ExecuteScalar();
            }
        }

        public List<NORQRequest> GetApplicationsForNORQRequest()
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationsForNORQRequest", ActiveConnection))
                return GetNORQEntities(cmd);
        }

        public List<ACRARequest> GetApplicationsForACRARequest()
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationsForACRARequest", ActiveConnection))
                return GetACRAEntities(cmd);
        }

        public List<NORQRequest> GetApplicationForNORQRequestByID(Guid id)
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationForNORQRequestByID", ActiveConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.UniqueIdentifier)).Value = id;
                return GetNORQEntities(cmd);
            }
        }

        public List<ACRARequest> GetApplicationForACRARequestByID(Guid id)
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationForACRARequestByID", ActiveConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.UniqueIdentifier)).Value = id;
                return GetACRAEntities(cmd);
            }
        }

        public List<NORQRequest> GetApplicationForNORQRequestByISN(int isn)
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationForNORQRequestByISN", ActiveConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@ISN", SqlDbType.Int)).Value = isn;
                return GetNORQEntities(cmd);
            }
        }

        public List<ACRARequest> GetApplicationForACRARequestByISN(int isn)
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetApplicationForACRARequestByISN", ActiveConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@ISN", SqlDbType.Int)).Value = isn;
                return GetACRAEntities(cmd);
            }
        }

        public bool LockApplicationByID(Guid id, byte status)
        {
            bool result;
            using (SqlCommand cmd = new SqlCommand("sp_LockApplicationByID", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.Parameters.Add(new SqlParameter("@STATUS_ID", SqlDbType.TinyInt)).Value = status;
                using (SqlDataReader reader = cmd.ExecuteReader())
                    result = reader.Read();
            }
            return result;
        }

        private List<NORQRequest> GetNORQEntities(SqlCommand cmd)
        {
            bool checkBirthDate = (GetSettingValue("NORQ_CHECK_BIRTH_DATE") == "1");
            List<NORQRequest> result = new List<NORQRequest>();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = CommandTimeoutInterval;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    NORQRequest item = new NORQRequest();
                    item.ID = reader.GetGuid(0);
                    item.SocialCardNumber = reader.GetString(1);
                    item.DocumentTypeCode = reader.GetString(2);
                    item.DocumentNumber = reader.GetString(3);
                    item.FirstName = reader.GetString(4);
                    item.LastName = reader.GetString(5);
                    if (checkBirthDate)
                        item.BirthDate = reader.GetDateTime(6);
                    if (!reader.IsDBNull(7))
                        item.CustomerUserID = reader.GetInt32(7);
                    result.Add(item);
                }
            }
            return result;
        }

        private List<ACRARequest> GetACRAEntities(SqlCommand cmd)
        {
            List<ACRARequest> result = new List<ACRARequest>();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = CommandTimeoutInterval;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ACRARequest item = new ACRARequest();
                    item.ID = reader.GetGuid(0);
                    item.FirstName = reader.GetString(1);
                    item.LastName = reader.GetString(2);
                    item.BirthDate = reader.GetDateTime(3);
                    item.PassportNumber = reader.GetString(4).Trim();
                    item.IDCardNumber = reader.GetString(5).Trim();
                    item.SocialCardNumber = reader.GetString(6);
                    result.Add(item);
                }
            }
            return result;
        }

        public void SaveNORQTryCount(Guid id)
        {
            using (SqlCommand cmd = new SqlCommand("sp_SaveNORQTryCount", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@APPLICATION_ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.ExecuteScalar();
            }
        }

        public void SaveACRATryCount(Guid id)
        {
            using (SqlCommand cmd = new SqlCommand("sp_SaveACRATryCount", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@APPLICATION_ID", SqlDbType.UniqueIdentifier)).Value = id;
                cmd.ExecuteScalar();
            }
        }

        public string GetCachedNORQResponse(string socialCard)
        {
            string result = string.Empty;
            using (SqlCommand cmd = new SqlCommand("sp_GetCachedNORQResponse", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@SOCIAL_CARD_NUMBER", SqlDbType.VarChar, 10)).Value = socialCard;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = rdr.GetString(0);
                    }
                }
            }
            return result;
        }

        public string GetCachedACRAResponse(string socialCard)
        {
            string result = string.Empty;
            using (SqlCommand cmd = new SqlCommand("sp_GetCachedACRAResponse", ActiveConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutInterval;
                cmd.Parameters.Add(new SqlParameter("@SOCIAL_CARD_NUMBER", SqlDbType.VarChar, 10)).Value = socialCard;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result = rdr.GetString(0);
                    }
                }
            }
            return result;
        }

        public static int CommandTimeoutInterval { get; set; }

        public DataHelper()
        {
            this.ActiveConnection = new SqlConnection("context connection=true");
            this.ActiveConnection.Open();
        }

        ~DataHelper()
        {
            this.DisposeConnection();
        }

        public SqlConnection ActiveConnection { get; set; }

        public void Dispose()
        {
            this.DisposeConnection();
            GC.SuppressFinalize(this);
        }

        public void DisposeConnection()
        {
            if (this.ActiveConnection != null)
            {
                this.ActiveConnection.Close();
                this.ActiveConnection = null;
            }
        }
    }
}
