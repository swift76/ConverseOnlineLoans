using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using IntelART.WebApiRequestProxy;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace IntelART.OnlineLoans.CustomerModuleWebApp
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddSingleton(Configuration);

            services.AddMvc()
                .AddJsonOptions(options=>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            services.Configure<ForwardedHeadersOptions>(option => { option.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForward‌​edFor; });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookies",
                ExpireTimeSpan = TimeSpan.FromMinutes(90),
                AutomaticChallenge = true,
                SlidingExpiration = true,
                Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments(new PathString("/api")))
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult<object>(null);
                    }
                }
            });

            ////JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            ////string authority = Configuration.GetSection("Authentication")["Authority"];

            ////app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            ////{
            ////    AuthenticationScheme = "oidc",
            ////    SignInScheme = "Cookies",
            ////    Authority = authority,
            ////    ClientId = "customerApplication2",
            ////    ClientSecret = "secret",
            ////    ResponseType = "code id_token",
            ////    RequireHttpsMetadata = false, // TODO: Change to 'true', leave as 'false' only in development mode
            ////    GetClaimsFromUserInfoEndpoint = true,
            ////    SaveTokens = true,
            ////    Scope = {
            ////        "customerApi",
            ////        "loanApplicationApi",
            ////        "offline_access",
            ////    },
            ////    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            ////    {
            ////        NameClaimType = "name",
            ////        RoleClaimType = "role",
            ////    },
            ////    AutomaticAuthenticate = false,
            ////    AutomaticChallenge = true,
            ////});

            IConfigurationSection requestProxyPolicies = Configuration.GetSection("RequestProxy").GetSection("Policies");
            IConfigurationSection requestProxyPolicy1 = requestProxyPolicies.GetSection("0");
            IConfigurationSection requestProxyPolicy2 = requestProxyPolicies.GetSection("1");

            PathString customerApiPathPrefix = new PathString(requestProxyPolicy1["LocalPath"]);
            string customerApiForwardUrlBase = requestProxyPolicy1["RemoteUrlBase"];

            PathString loanApiPathPrefix = new PathString(requestProxyPolicy2["LocalPath"]);
            string loanApiForwardUrlBase = requestProxyPolicy2["RemoteUrlBase"];

            app.UseWebApiRequestProxy()
                .AddRule(
                    new SimleWebApiRequestProxyRule(
                        customerApiPathPrefix,
                        customerApiForwardUrlBase,
                        async (r, c) =>
                        {
                            AuthenticateInfo info = await r.HttpContext.Authentication.GetAuthenticateInfoAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            string accessToken;
                            if (info != null
                                && info.Properties!=null
                                && info.Properties.Items!=null
                                && info.Properties.Items.TryGetValue(".Token.access_token", out accessToken))
                            {
                                c.SetBearerToken(accessToken);
                            }
                        })
                )
                .AddRule(
                    new SimleWebApiRequestProxyRule(
                        loanApiPathPrefix,
                        loanApiForwardUrlBase,
                        async (r, c) =>
                        {
                            AuthenticateInfo info = await r.HttpContext.Authentication.GetAuthenticateInfoAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            string accessToken;
                            if (info != null
                                && info.Properties != null
                                && info.Properties.Items != null
                                && info.Properties.Items.TryGetValue(".Token.access_token", out accessToken))
                            {
                                c.SetBearerToken(accessToken);
                            }
                        })
                );

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
