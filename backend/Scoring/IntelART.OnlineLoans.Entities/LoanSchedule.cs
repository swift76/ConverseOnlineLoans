using System;

namespace IntelART.OnlineLoans.Entities
{
    public class LoanSchedule
    {
        public DateTime Date { get; set; }
        public decimal MainAmount { get; set; }
        public decimal InterestAmount { get; set; }
    }
}
