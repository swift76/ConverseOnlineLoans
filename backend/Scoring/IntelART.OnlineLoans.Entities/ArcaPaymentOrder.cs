using System;
using System.Collections.Generic;
using System.Text;

namespace IntelART.OnlineLoans.Entities
{
    public class ArcaPaymentOrder
    {
        public long ID { get; set; }
        public string APPLICATION_PAYLOAD { get; set; }
        public string ARCA_ORDER_ID { get; set; }
        public DateTime PAY_DATE { get; set; }
        public string CARD_HOLDER_NAME { get; set; }
        public string STATUS { get; set; }
    }
}
