﻿namespace IntelART.OnlineLoans.Entities
{
    /// <summary>
    /// The agreed application for the loan
    /// It contain all the info from the applicatin metadata, thus inheriting it
    /// </summary>
    public class AgreedApplication : Application
    {
        public string LOAN_GETTING_OPTION_CODE { get; set; }
        public string COMMUNICATION_TYPE_CODE  { get; set; }
        public string EXISTING_CARD_CODE    { get; set; }
        public bool IS_NEW_CARD             { get; set; }
        public string CREDIT_CARD_TYPE_CODE { get; set; }
        public bool IS_CARD_DELIVERY        { get; set; }
        public string CARD_DELIVERY_ADDRESS { get; set; }
        public string BANK_BRANCH_CODE      { get; set; }
        public string MOBILE_PHONE_2        { get; set; }
        public string CARD_RECOVERY_CODE    { get; set; }
        public bool AGREED_WITH_TERMS       { get; set; }
        public bool IS_ARBITRAGE_CHECKED    { get; set; }
        public bool SUBMIT                  { get; set; } // Submit or Save
    }
}
