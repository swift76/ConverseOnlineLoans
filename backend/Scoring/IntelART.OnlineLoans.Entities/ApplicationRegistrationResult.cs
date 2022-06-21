using System;
using System.Collections.Generic;
using System.Text;

namespace IntelART.OnlineLoans.Entities
{
    /// <summary>
    /// If application is registered, then <c>RegisteredApplicationId</c> is not null and contains the id of application.
    /// If user has to make payment to be identified, then <c>PaymentFormUrl</c> contains the form url where user can make payment.
    /// </summary>
    public class ApplicationRegistrationResult
    {
        public Guid? RegisteredApplicationId { get; set; }
        public string PaymentFormUrl { get; set; }
    }
}