using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace helper
{
    public static class Helper
    {
        public static IConfiguration Configuration { get; set; }

        public static void ReadConfiguration()
        {
            // Reading configuration from appsettings.json
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Helper.Configuration = builder.Build();
        }

        public static DateTime MongoTimeStampToDateTime(Int64 TimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(TimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static Int64 DateTimeToMongoTimeStamp(DateTime date)
        {
            Int64 unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp * 1000;
        }

        public static void Log(String message)
        {
            Console.WriteLine(message);
        }

        public static void DEBUG(String message)
        {
            if (Configuration["DEBUG"] == "YES")
                Console.WriteLine(message);
        }
    }
}

