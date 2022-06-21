using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Services;
using IntelART.IdentityManagement;
using IntelART.OnlineLoans.SqlMembershipProvider;
using IntelART.Communication;
using IntelART.ConverseBank.Communication;

namespace IntelART.IdentityServer
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer(Config.SetupIdentityServer)
                .AddTemporarySigningCredential()  // Replace with .AddSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(Configuration.GetSection("ClientApplications")))
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                ;

            IMembershipProvider membershipProvider = new SqlMembershipProvider(Configuration.GetSection("ConnectionStrings")["MembershipDB"]);

            services.AddTransient<IUserStore>((sp) => membershipProvider);
            services.AddTransient<IMembershipProvider>((sp) => membershipProvider);
            ////services.AddTransient<IUserStore>((sp) => new TestUserStore(new[] { new UserInfo() { Id = 1, Username = "administrator" } }));

            services.AddTransient<IProfileService, ProfileService>();

            ////IEmailSender emailSender = new SmtpEmailSender("in-v3.mailjet.com", 465, "0fdd489790973f190ccfff73f1425b64", "9ac8f567083d3e1fa22932c8dec555b9", "support@inchvorban.com", true);
            ////IEmailSender emailSender = new SmtpEmailSender("email-smtp.us-east-1.amazonaws.com", 465, "AKIAJVZDYMEVBDV75LSA", "Ag2+Vnn+QmB5L8jor8cyB9dNR4E8LSzfids6pYcosj8D", "tigran.grigoryan@gmail.com", true);
            IEmailSender emailSender;
            if (Configuration["EmailService"] == "ConverseBank")
            {
                emailSender = new ConverseBankDbEmailSender(Configuration.GetSection("ConnectionStrings")["MembershipDB"]);
            }
            else
            {
                emailSender = new SmtpEmailSender("smtp.sendgrid.net", 465, "apikey", "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", "support@inchvorban.com", true);
            }

            services.AddTransient<IEmailSender>((sp) => emailSender);

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseSwagger();
            app.UseSwaggerUi3();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
