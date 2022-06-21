using System;

namespace IntelART.OnlineLoans.Entities
{
    /// <summary>
    /// Basic information about the loan application
    /// </summary>
    public class Application
    {
        public Guid? ID               { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public byte   STATUS_ID       { get; set; }
        public string STATUS_AM       { get; set; }
        public string STATUS_EN       { get; set; }
        public string STATUS_STATE    { get; set; }
        public decimal AMOUNT         { get; set; }
        public string LOAN_TYPE_ID    { get; set; }
        public string LOAN_TYPE_AM    { get; set; }
        public string LOAN_TYPE_EN    { get; set; }
        public string LOAN_TYPE_STATE { get; set; }
        public string DISPLAY_AMOUNT  { get; set; }
        public bool PRINT_EXISTS { get; set; }
    }
}
