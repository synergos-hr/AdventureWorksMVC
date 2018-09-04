using System;
using System.Configuration;
using System.Globalization;

namespace AdventureWorks.Web.Helpers.Settings
{
    public static class AppSettings
    {
        public static string CompanyName => Setting<string>("CompanyName");
        public static string AppName => Setting<string>("AppName");

        public static string AppTitle => $"{CompanyName} - {AppName}";

        public static bool LoginAsEmail => Setting<bool>("LoginAsEmail");
        public static int LoginExpireMinutes => Setting<int>("LoginExpireMinutes");

        public static bool FakeEmailSend => Setting<bool>("FakeEmailSend");

        public static string EMailProjectAddress => Setting<string>("EMailProjectAddress");
        public static string EMailFromAddress => Setting<string>("EMailFromAddress");
        public static string EMailFromName => Setting<string>("EMailFromName");
        public static string EMailHostName => Setting<string>("EMailHostName");
        public static int EMailHostPort => Setting<int>("EMailHostPort");
        public static bool EMailHostEnableSsl => Setting<bool>("EMailHostEnableSsl");
        public static string EMailUserName => Setting<string>("EMailUserName");
        public static string EMailPassword => Setting<string>("EMailPassword");
        public static string EMailBcc => Setting<string>("EMailBcc");

        public static string DocumentsDir => Setting<string>("DocumentsDir");

        public static string AppVersion => Setting<string>("AppVersion");

        //public static bool LogSqlQueries => Setting<bool>("LogSqlQueries");

        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
                throw new Exception($"Could not find setting '{name}',");

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}