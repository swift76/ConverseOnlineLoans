using System;

namespace IntelART.OnlineLoans.Entities
{
    public class MobilePhoneAuthorization
    {
        public string SMS_HASH { get; set; }
        public DateTime SMS_SENT_DATE { get; set; }
        public int SMS_COUNT { get; set; }
    }
}
