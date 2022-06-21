using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IdentityModel.Client;
using Newtonsoft.Json;
using IntelART.OnlineLoans.Entities;
using IntelART.Utilities;
using IntelART.OnlineLoans.Repositories;

namespace IntelART.OnlineLoans.CustomerModuleWebApp.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private string identityServerUrl;

        private string CurrentUsername
        {
            get
            {
                return HttpContext.User.Identity.Name;
            }
        }

        public AccountController(IConfigurationRoot configuration)
        {
            this.identityServerUrl = configuration.GetSection("Authentication")["Authority"];
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery]string returnUrl)
        {
            IActionResult result;
            if (HttpContext.User != null
                && HttpContext.User.Identity != null
                && HttpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    result = RedirectToAction("", "");
                }
                else
                {
                    result = Redirect(returnUrl);
                }
            }
            else
            {
                AuthenticationViewModel model = new AuthenticationViewModel();
                model.LoginModel.ReturnUrl = returnUrl;
                result = View(model);
            }

            return result;
        }

        [HttpGet]
        public async Task<IActionResult> RequestPasswordReset()
        {
            RequestPasswordResetModel model = new RequestPasswordResetModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset([FromForm]RequestPasswordResetModel model)
        {
            IActionResult result = null;
            bool hasError = !ModelState.IsValid;
            try
            {
                if (!hasError)
                {
                    PasswordResetModel newModel = new PasswordResetModel();
                    newModel.RegistrationProcessId = Guid.NewGuid();
                    newModel.Phone = model.Phone;

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host));
                    HttpResponseMessage response = await client.PutAsync(string.Format("/api/customer/Account/{0}/PasswordManagerProcess/{1}", model.Phone, newModel.RegistrationProcessId), null);
                    if (!response.IsSuccessStatusCode)
                    {
                        hasError = true;
                        bool isUnknownError = true;
                        if (response.Content != null)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            ErrorInfo exception = JsonConvert.DeserializeObject<ErrorInfo>(content);
                            if (exception != null)
                            {
                                isUnknownError = false;
                                ModelState.AddModelError("", exception.Message);
                            }
                        }
                        if (isUnknownError)
                        {
                            ModelState.AddModelError("", "Համակարգային սխալ գաղտնաբառի փոփոխման ժամանակ");
                        }
                    }

                    result = View("PasswordReset", newModel);
                }
            }
            catch (Exception e)
            {
                hasError = true;
                ModelState.AddModelError("", e.Message);
            }
            if(hasError)
            {
                result = View(model);
            }
            
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> PasswordReset([FromForm]PasswordResetModel model)
        {
            IActionResult result = null;
            bool hasError = !ModelState.IsValid;
            try
            {
                if (!hasError)
                {
                    CustomerUserPasswordResetData data = new CustomerUserPasswordResetData();
                    data.SmsCode = model.VerificationCode;
                    data.NewPassword = model.Password;
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host));
                    HttpResponseMessage response = await client.PostAsync(string.Format("/api/customer/Account/{0}/PasswordManagerProcess/{1}", model.Phone, model.RegistrationProcessId), new StringContent(JsonConvert.SerializeObject(data, Formatting.None), System.Text.Encoding.UTF8));
                    if (!response.IsSuccessStatusCode)
                    {
                        hasError = true;
                        bool isUnknownError = true;
                        if (response.Content != null)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            ErrorInfo exception = JsonConvert.DeserializeObject<ErrorInfo>(content);
                            if (exception != null)
                            {
                                isUnknownError = false;
                                ModelState.AddModelError("", exception.Message);
                            }
                        }
                        if (isUnknownError)
                        {
                            ModelState.AddModelError("", "Համակարգային սխալ գաղտնաբառի փոփոխման ժամանակ");
                        }
                    }

                    result = Redirect("Login");
                }
            }
            catch (Exception e)
            {
                hasError = true;
                ModelState.AddModelError("", e.Message);
            }
            if (hasError)
            {
                result = View(model);
            }

            return result;
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm]LoginModel model)
        {
            IActionResult result;

            var discoveryClient = new DiscoveryClient(this.identityServerUrl);
            discoveryClient.Policy.RequireHttps = false;
            var disco = await discoveryClient.GetAsync();
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var client = new TokenClient(
                disco.TokenEndpoint,
                "customerApplication",
                "secret");

            string username = null;
            string password = null;

            username = model.Username;
            password = model.Password;

            string ipAddress;
            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"];
            else if (HttpContext.Request.Headers.ContainsKey("X-OL-IP"))
                ipAddress = HttpContext.Request.Headers["X-OL-IP"];
            else
                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            object extra = new System.Collections.Generic.Dictionary<string, string> { { "X-OL-IP", ipAddress } }; ;
            TokenResponse tokenResponse = await client.RequestResourceOwnerPasswordAsync(username, password, "openid profile customerApi loanApplicationApi offline_access", extra);

            if (!tokenResponse.IsError
                && tokenResponse.AccessToken != null)
            {
                JwtSecurityToken token = new JwtSecurityToken(tokenResponse.AccessToken);
                AuthenticationProperties props = new AuthenticationProperties();
                props.Items[".Token.access_token"] = tokenResponse.AccessToken;
                await HttpContext.Authentication.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme)),
                    props);

                if (string.IsNullOrEmpty(model.ReturnUrl))
                {
                    result = RedirectToAction("", "");
                }
                else
                {
                    result = Redirect(model.ReturnUrl);
                }
            }
            else
            {
                AuthenticationViewModel viewModel = new AuthenticationViewModel();
                viewModel.LoginModel.Username = model.Username;
                viewModel.LoginModel.ReturnUrl = model.ReturnUrl;
                if (!string.IsNullOrEmpty(tokenResponse.ErrorDescription))
                {
                    ModelState.AddModelError("", tokenResponse.ErrorDescription);
                }
                result = View(viewModel);
            }
            return result;
        }

        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            AuthenticationViewModel viewModel = new AuthenticationViewModel();
            Guid processId = Guid.NewGuid();
            string ssn = null;
            bool hasError = !ModelState.IsValid;
            try
            {
                if (!hasError)
                {
                    ssn = model.SSN.Replace(" ", "");

                    if (!model.AcceptedTermsAndConditions)
                    {
                        throw new ApplicationException("ERR-0034", "Պայմաններին և կանոններին Ձեր համաձայնությունը պարտադիր է։");
                    }
                    else
                    {
                        // password validation
                        ValidationManager.ValidateCustomerPasswordCreation(model.SSN,
                                                                           model.Password,
                                                                           model.ConfirmPassword);

                        CustomerUserRegistrationPreVerification user = new CustomerUserRegistrationPreVerification();
                        user.FIRST_NAME = model.Name;
                        user.LAST_NAME = model.Lastname;
                        user.MOBILE_PHONE = model.Phone;
                        user.EMAIL = model.Email;
                        user.HASH = Crypto.HashString(model.Password);
                        user.SOCIAL_CARD_NUMBER = ssn;
                        user.PROCESS_ID = processId;
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host));
                        HttpResponseMessage response = await client.PostAsync("/api/customer/Account", new StringContent(JsonConvert.SerializeObject(user, Formatting.None), System.Text.Encoding.UTF8));
                        if (!response.IsSuccessStatusCode)
                        {
                            hasError = true;
                            bool isUnknownError = true;
                            if (response.Content != null)
                            {
                                string content = await response.Content.ReadAsStringAsync();
                                ErrorInfo exception = JsonConvert.DeserializeObject<ErrorInfo>(content);
                                if (exception != null)
                                {
                                    isUnknownError = false;
                                    ModelState.AddModelError("", exception.Message);
                                }
                            }
                            if (isUnknownError)
                            {
                                ModelState.AddModelError("", "Համակարգային սխալ գրանցման ժամանակ");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                hasError = true;
                ModelState.AddModelError("", e.Message);
            }

            if (hasError)
            {
                viewModel.RegisterModel.Name = model.Name;
                viewModel.RegisterModel.Lastname = model.Lastname;
                viewModel.RegisterModel.SSN = model.SSN;
                viewModel.RegisterModel.Phone = model.Phone;
                viewModel.RegisterModel.Email = model.Email;
                viewModel.RegisterModel.AcceptedTermsAndConditions = model.AcceptedTermsAndConditions;
                viewModel.RegisterModel.IsActive = true;
                viewModel.LoginModel.IsActive = false;
            }
            else
            {
                ModelState.Clear();
                viewModel.VerificationModel.Phone = "+374 " + model.Phone;
                viewModel.VerificationModel.RegistrationProcessId = processId;
                viewModel.VerificationModel.IsActive = true;
                viewModel.LoginModel.IsActive = false;
            }

            return View("Login", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhone([FromForm]VerificationModel model)
        {
            AuthenticationViewModel viewModel = new AuthenticationViewModel();
            bool hasError = !ModelState.IsValid;
            try
            {
                if (!hasError)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host));
                    HttpResponseMessage response = await client.PutAsync(string.Format("/api/customer/Account/{0}/Verification/{1}", model.RegistrationProcessId, model.VerificationCode), null);
                    if (!response.IsSuccessStatusCode)
                    {
                        hasError = true;
                        bool isUnknownError = true;
                        if (response.Content != null)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            ErrorInfo exception = JsonConvert.DeserializeObject<ErrorInfo>(content);
                            if (exception != null)
                            {
                                isUnknownError = false;
                                ModelState.AddModelError("", exception.Message);
                            }
                        }
                        if (isUnknownError)
                        {
                            ModelState.AddModelError("", "Համակարգային սխալ գրանցման ժամանակ");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                hasError = true;
                ModelState.AddModelError("", e.Message);
            }

            if (hasError)
            {
                viewModel.VerificationModel.Phone = model.Phone;
                viewModel.VerificationModel.RegistrationProcessId = model.RegistrationProcessId;
                viewModel.VerificationModel.VerificationCode = model.VerificationCode;
                viewModel.VerificationModel.IsActive = true;
                viewModel.RegisterModel.IsActive = false;
                viewModel.LoginModel.IsActive = false;
                return View("Login", viewModel);

            }
            else
            {
                ModelState.Clear();
                return View("SuccessRegisteredMessage");

            }

        }

        [HttpPost]
        public async Task<IActionResult> ResendVerificationCode([FromQuery]Guid processID)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(string.Format("{0}://{1}", this.Request.Scheme, this.Request.Host));
            HttpResponseMessage response = await client.PostAsync(string.Format("/api/customer/Account/{0}/", processID), null);

            if (!response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ErrorInfo exception = JsonConvert.DeserializeObject<ErrorInfo>(content);
                    throw new Exception(exception.Message);
                }
                throw new Exception();
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("", "");
        }
    }
}
