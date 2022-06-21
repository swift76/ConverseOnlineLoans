using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntelART.OnlineLoans.Repositories
{
    public class AppConfiguration
    {
        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            var arcaSection = root.GetSection("ArcaEndpoints");
            this.CreateOrderEndpoint = arcaSection.GetSection("CreateOrderEndpoint").Value;
            this.GetOrderDetailsEndpoint = arcaSection.GetSection("GetOrderDetailsEndpoint").Value;
            this.RefundOrderEndpoint = arcaSection.GetSection("RefundOrderEndpoint").Value;
        }

        public string CreateOrderEndpoint { get; }

        public string GetOrderDetailsEndpoint { get; }

        public string RefundOrderEndpoint { get; }
    }
}
