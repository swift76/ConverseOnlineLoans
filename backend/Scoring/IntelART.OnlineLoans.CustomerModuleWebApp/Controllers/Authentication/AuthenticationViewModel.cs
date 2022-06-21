﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class AuthenticationViewModel
    {
        public LoginModel LoginModel { get; private set; }
        public RegisterModel RegisterModel { get; private set; }
        public VerificationModel VerificationModel { get; private set; }

        public string ActiveView { get; set; }

        public AuthenticationViewModel()
        {
            this.LoginModel = new LoginModel();
            this.RegisterModel = new RegisterModel();
            this.VerificationModel = new VerificationModel();
            this.LoginModel.IsActive = true;
        }
    }
}
