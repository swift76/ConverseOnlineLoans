﻿using System.ComponentModel.DataAnnotations;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class LoginModel
    {
        [Required(ErrorMessage = "ՀԾՀ / սոցիալական քարտի համարը պարտադիր է:")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Գաղտնաբառը պարտադիր է:")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
