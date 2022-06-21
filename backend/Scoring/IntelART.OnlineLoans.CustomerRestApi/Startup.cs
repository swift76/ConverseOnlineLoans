using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using IntelART.Utilities;
using System.Collections.Generic;
using IntelART.Communication;
using IntelART.ConverseBank.Communication;

namespace IntelART.OnlineLoans.CustomerRestApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection corsConfig = Configuration.GetSection("Cors");

            if (corsConfig != null)
            {
                IEnumerable<string> origins = corsConfig.GetSection("Origins").AsEnumerable().Where(t => t.Value != null).Select(t => t.Value);
                services.AddCors(options =>
                {
                    options.AddPolicy("default", policy =>
                    {
                        policy.WithOrigins(origins.ToArray())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            }

            // Add framework services.
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddSingleton(Configuration);

            ISmsSender smsSender = new ConverseBankDbSmsSender(Configuration.GetSection("ConnectionStrings")["ScoringDB"]);

            services.AddTransient((sp) => smsSender);

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("default");

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        ErrorInfo errorInfo = ErrorInfo.For(ex.Error);
                        string error = Newtonsoft.Json.JsonConvert.SerializeObject(errorInfo);
                        await context.Response.WriteAsync(error).ConfigureAwait(false);
                    }
                });
            });

            string authServiceUrl = Configuration.GetSection("AutenticationService")["Url"];
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = authServiceUrl,
                AllowedScopes = { "customerApi" },
                RequireHttpsMetadata = false,
            });

            app.UseMvc();
        }
    }
}
