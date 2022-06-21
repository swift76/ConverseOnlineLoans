using System;

namespace IntelART.OnlineLoans.Entities
{
    public class FinalApplication
    {
        public string LOAN_TYPE_ID { get; set; }
        public decimal INITIAL_AMOUNT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string FIRST_NAME_AM { get; set; }
        public string LAST_NAME_AM { get; set; }
        public string PATRONYMIC_NAME_AM { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string SOCIAL_CARD_NUMBER { get; set; }
        public byte? REPAY_DAY { get; set; }
        public decimal FINAL_AMOUNT { get; set; }
        public decimal INTEREST { get; set; }
        public string FIRST_NAME_EN { get; set; }
        public string LAST_NAME_EN { get; set; }
        public string MOBILE_PHONE_1 { get; set; }
        public string FIXED_PHONE { get; set; }
        public string EMAIL { get; set; }
        public string PERIOD_TYPE_CODE { get; set; }
        public string BIRTH_PLACE_CODE { get; set; }
        public string CITIZENSHIP_CODE { get; set; }
        public string REGISTRATION_COUNTRY_CODE { get; set; }
        public string REGISTRATION_STATE_CODE { get; set; }
        public string REGISTRATION_CITY_CODE { get; set; }
        public string REGISTRATION_STREET { get; set; }
        public string REGISTRATION_BUILDNUM { get; set; }
        public string REGISTRATION_APARTMENT { get; set; }
        public string CURRENT_COUNTRY_CODE { get; set; }
        public string CURRENT_STATE_CODE { get; set; }
        public string CURRENT_CITY_CODE { get; set; }
        public string CURRENT_STREET { get; set; }
        public string CURRENT_BUILDNUM { get; set; }
        public string CURRENT_APARTMENT { get; set; }
        public string COMMUNICATION_TYPE_CODE { get; set; }
        public string LOAN_GETTING_OPTION_CODE { get; set; }
        public bool GENDER { get; set; }
        public string DISTRICT { get; set; }
        public string COMMUNITY { get; set; }
        public string STREET { get; set; }
        public string BUILDING { get; set; }
        public string APARTMENT { get; set; }
        public DocumentData NonBiometricPassport { get; set; }
        public DocumentData BiometricPassport { get; set; }
        public DocumentData IDCard { get; set; }
        public decimal SALARY { get; set; }
        public string AGREEMENT_NUMBER { get; set; }
        public Pledge PLEDGE { get; set; }
    }

    public class DocumentData
    {
        public string NUMBER { get; set; }
        public DateTime ISSUE_DATE { get; set; }
        public DateTime EXPIRY_DATE { get; set; }
        public string ISSUED_BY { get; set; }
    }

    public class Pledge
    {
        public decimal AMOUNT { get; set; }
        public string CURRENCY { get; set; }
        public decimal RATIO { get; set; }
        public string NAME { get; set; }
        public decimal PRICE { get; set; }
    }
}
