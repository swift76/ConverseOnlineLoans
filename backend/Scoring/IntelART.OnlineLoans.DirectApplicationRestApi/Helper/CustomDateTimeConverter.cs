using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Helper
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd/MM/yyyy";
        }
    }
}