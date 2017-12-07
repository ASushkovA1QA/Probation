using System;
using System.Configuration;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// class with settings for the solution
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// get from app.config field Login
        /// </summary>
        public static string Login
        {
            get { return GetValue("Login"); }
        }

        /// <summary>
        /// get from app.config field LoginURL
        /// </summary>
        public static string LoginUrl
        {
            get { return GetValue("LoginURL"); }
        }
        
        public static string StartUrl
        {
            get { return GetEnviromentVar("StartUrl", "http://localhost:8080"); }
        }

        /// <summary>
        /// get from app.config field Password
        /// </summary>
        public static string Password
        {
            get { return GetValue("Password"); }
        }
      
        /// <summary>
        /// get from app.config field element_timeout
        /// </summary>
        public static string ElementTimeout
        {
            get { return GetValue("element_timeout"); }
        }

        /// <summary>
        /// get from app.config field email_wait_timeout
        /// defines timeout for waiting emails
        /// </summary>
        public static string EmailWaitTimeout
        {
            get { return GetValue("email_wait_timeout"); }
        }
        

        /// <summary>
        /// get from app.config field page_timeout
        /// </summary>
        public static string PageTimeout
        {
            get { return GetValue("page_timeout"); }
        }

        /// <summary>
        /// get from app.config field Delay
        /// </summary>
        public static string Delay
        {
            get { return GetValue("Delay"); }
        }

        /// <summary>
        /// get from app.config field Browser
        /// </summary>
        public static string Browser
        {
            get { return GetValue("browser"); }
        }
               
        /// <summary>
        /// get from app.config field Remote
        /// </summary>
        public static bool Remote
        {
            get { return bool.Parse(GetValue("remote")); }
        }

        /// <summary>
        /// get from app.config field Grid.hub
        /// </summary>
        public static Uri GridHub
        {
            get { return new Uri(GetValue("grid.hub")); }
        }

        /// <summary>
        /// get from app.config field platform.vesion
        /// </summary>
        public static String PlatformVersion
        {
            get { return GetValue("platform.vesion"); }
        }
        /// <summary>
        ///  get boolean value for the field MobileTesting from environment or from app.config
        /// </summary>
        public static bool MobileTesting
        {
            get { return bool.Parse(GetValue("MobileTesting")); }
        }

        public static string BrowserLang
        {
            get { return GetValue("BrowserLang"); }
        }

        public static string ImageFormat
        {
            get { return GetValue("ImageFormat"); }
        }

        public static string StartProxyServer
        {
            get { return GetValue("StartProxyServer"); }
        }
        public static string EdgeDriverPath
        {
            get { return GetValue("EdgeDriverPath"); }
        }

        /// <summary>
        /// described preferences for database
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// get from app.config field ProviderInvariantName
            /// </summary>
            public static string ProviderInvariantName
            {
                get { return GetDatabaseValue("provider_invariant_name"); }
            }

            /// <summary>
            /// get from app.config field ConnectionString
            /// </summary>
            public static string ConnectionString
            {
                get { return GetDatabaseValue("connection_string"); }
            }
        }

        private static string GetDatabaseValue(string key)
        {
            return GetValue("database." + key);
        }

        public static string DeviceName
        {
            get { return GetChromeMobileEmulator("deviceName"); }
        }

        private static string GetChromeMobileEmulator(string key)
        {
            return GetValue("chromemobileemulator." + key);
        }

        /// <summary>
        /// get from app.config field and converts into string
        /// </summary>
        protected static string GetValue(string key)
        {
            return GetEnviromentVar(key, ConfigurationManager.AppSettings.Get(key));
        }

        /// <summary>
        /// returns value of environment variable
        /// </summary>
        /// <param name="var">variable's name</param>
        /// <param name="defaultValue">default value, will be returned if env var was not setted</param>
        /// <returns>value of environment variable</returns>
        public static string GetEnviromentVar(string var, string defaultValue)
        {
            return System.Environment.GetEnvironmentVariable(var) ?? defaultValue;
        }

        /// <summary>
        /// virtual disk, which will contains sreenshots
        /// </summary>
        public static string DriveTestData
        {
            get { return GetEnviromentVar("DriveTestData", "C"); }
        }
    }
}