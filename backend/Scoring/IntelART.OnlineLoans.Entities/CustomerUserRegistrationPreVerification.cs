using System;

namespace IntelART.OnlineLoans.Entities
{
    public class CustomerUserRegistrationPreVerification : ApplicationUser
    {
        public Guid PROCESS_ID { get; set; }
        public string SOCIAL_CARD_NUMBER { get; set; }
        public string MOBILE_PHONE { get; set; }
        public string VERIFICATION_CODE { get; set; }
    }
}
