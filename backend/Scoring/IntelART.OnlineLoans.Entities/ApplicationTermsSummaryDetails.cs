using System;

namespace IntelART.OnlineLoans.Entities
{
    /// <summary>
    /// The preapproval application for the loan
    /// It contain all the info from the applicatin metadata, thus inheriting it
    /// </summary>
    public class ApplicationTermsSummaryDetails
    {
        public string LOAN_TYPE_ID { get; set; }
        public string LOAN_TYPE_NAME { get; set; }
        public string STATUS_STATE { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public decimal FINAL_AMOUNT { get; set; }
        public byte DURATION { get; set; }
        public decimal INTEREST { get; set; }
        public decimal REAL_INTEREST { get; set; }
        public decimal MONTHLY_PAYMENT { get; set; }
        public decimal FIRST_PAYMENT { get; set; }
        public decimal FIRST_PRINCIPAL_PAYMENT { get; set; }
        public decimal TOTAL_PAYMENT { get; set; }
        public decimal TOTAL_INTEREST_PAYMENT { get; set; }
        public string CURRENCY_NAME { get; set; }
    }
}
