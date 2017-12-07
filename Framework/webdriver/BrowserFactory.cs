using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using WebdriverFramework.Framework.Util;
using WebdriverFramework.Framework.WebDriver.BrowserBuilder;

namespace WebdriverFramework.Framework.WebDriver
{
    public class BrowserFactory
    {
        private static readonly Logger Log = Logger.Instance;
        private static bool IsRemote { get; set; }
        private static Uri RemoteUri { get; set; }
        private static string DownloadFolder { get; set; }
        private static Proxy SeleniumProxy { get; set; }

        public static string GetDownloadFolder()
        {
            return DownloadFolder;
        }

        public enum BrowserType
        {
            Chrome, Firefox, Iexplore, Html, ChromeMobileEmulator, Edge
        }

        public static Dictionary<BrowserType, IWebDriverBuilder> BrowserDictionary = new Dictionary<BrowserType, IWebDriverBuilder>()
        {
          {BrowserType.Chrome, new ChromeDriverBuilder() },
          {BrowserType.Firefox, new FirefoxDriverBuilder() },
          {BrowserType.Iexplore, new IExploreDriverBuilder() },
          {BrowserType.Html, new HtmlDriverBuilder() },
          {BrowserType.ChromeMobileEmulator, new ChromeMobileEmulatorDriverBuilder() },
          {BrowserType.Edge, new EdgeDriverBuilder() }
        };

        public static RemoteWebDriver GetDriver(BrowserType type)
        {
            var pathString = CreateDownloadDir();
            var driver = BrowserDictionary[type].GetDriver(IsRemote, RemoteUri, SeleniumProxy, pathString);
            return driver;
        }

        public static RemoteWebDriver SetUp(string type, bool isRemote, Uri remoteUri, out BrowserType browserType)
        {
            IsRemote = isRemote;
            RemoteUri = remoteUri;
            if (Enum.TryParse(type, true, out browserType))
            {
                return GetDriver(browserType);
            }

            Log.Fail($"couldn't parse browser type {type}");
            return null;
        }

        private static string CreateDownloadDir()
        {
            var downloadFolderPath = DirectoryExtension.CreateTempDirectory();
            DownloadFolder = downloadFolderPath;
            Directory.CreateDirectory(downloadFolderPath);
            return downloadFolderPath;
        }
    }
}
