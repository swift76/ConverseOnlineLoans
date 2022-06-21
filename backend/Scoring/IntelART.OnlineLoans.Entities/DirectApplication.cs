using System;

namespace IntelART.OnlineLoans.Entities
{
    public class DirectApplication : Application
    {
        public string FIRST_NAME_AM { get; set; }
        public string LAST_NAME_AM { get; set; }
        public string PATRONYMIC_NAME_AM { get; set; }
        public DateTime? BIRTH_DATE { get; set; }
        public string SOCIAL_CARD_NUMBER { get; set; }
        public string DOCUMENT_TYPE_CODE { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public string REGISTRATION_MOBILE_PHONE { get; set; }
        public string REGISTRATION_EMAIL { get; set; }
    }
}
