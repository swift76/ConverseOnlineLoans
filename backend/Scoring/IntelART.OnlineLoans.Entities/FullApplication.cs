using System;
using System.Collections.Generic;
using System.Text;

namespace IntelART.OnlineLoans.Entities
{
    /// <summary>
    /// A wrapper including all aspects of the loan application
    /// </summary>
    public class FullApplication
    {
        public Guid? ID { get; set; }
        public InitialApplication preApprovalApplication { get; set; }
        public MainApplication mainApplication { get; set; }
        public AgreedApplication loanDeliveryDetails { get; set; }
    }
}
