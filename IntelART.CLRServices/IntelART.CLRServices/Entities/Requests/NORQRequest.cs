using System;

namespace IntelART.CLRServices
{
    public class NORQRequest
    {
        public Guid ID { get; set; }
        public string SocialCardNumber { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int? CustomerUserID { get; set; }
    }
}
