using System;

namespace IntelART.OnlineLoans.Entities
{
    public class ClientCard
    {
        public string EmbossedName { get; set; }
        public string MobilePhone { get; set; }
        public string cardNumber { get; set; }
        public DateTime? expiryDate { get; set; }
    }
}
