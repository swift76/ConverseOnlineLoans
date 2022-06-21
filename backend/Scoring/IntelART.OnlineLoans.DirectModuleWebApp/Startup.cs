using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using IntelART.WebApiRequestProxy;

namespace IntelART.OnlineLoans.DirectModuleWebApp
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
                .AddJsonFormatters();

            services.AddSingleton(Configuration);

            services.AddMvc()
                .AddJsonOptions(options=>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
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
                        customerApiForwardUrlBase, async (r, c) =>
                        {
                        })
                )
                .AddRule(
                    new SimleWebApiRequestProxyRule(
                        loanApiPathPrefix,
                        loanApiForwardUrlBase, async (r, c) =>
                        {
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
