﻿using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public class ChromeDriverBuilder : IWebDriverBuilder
    {
        private static readonly Logger Log = Logger.Instance;

        public RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            return isRemote
                ? GetRemoteDriver(link, seleniumProxy, downloadFolder)
                : GetLocalDriver(seleniumProxy, downloadFolder);
        }

        private RemoteWebDriver GetRemoteDriver(Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            var caps = DesiredCapabilities.Chrome();
            var options = GetChromeOptions(true, seleniumProxy, downloadFolder);
            options.AddAdditionalCapability(CapabilityType.Version, Configuration.PlatformVersion, true);
            return new ScreenShotRemoteWebDriver(link, options.ToCapabilities(), new TimeSpan(0, 5, 0)); 
        }

        private static ChromeOptions GetChromeOptions(bool isRemote, Proxy seleniumProxy, string downloadFolder)
        {
            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", string.Empty + downloadFolder);
            options.AddUserProfilePreference("download.prompt_for_download", isRemote);
            options.AddUserProfilePreference("download.extensions_to_open", "exe:msi:pkg");
            options.AddUserProfilePreference("unexpectedAlertBehaviour", "ignore");
            options.AddUserProfilePreference("safebrowsing.enabled", true);

            options.AddArguments("user-agent=Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.93");
            options.AddArguments("disable-application-cache");
            options.AddArguments("disable-cache");
            options.AddArguments("disable-gpu-program-cache");
            //options.AddArguments("--enable-potentially-annoying-security-features");
            //options.AddArguments("--disable-geolocation");
            //options.AddUserProfilePreference("profile.default_content_settings.popups", "0");
            //options.AddUserProfilePreference("disable-popup-blocking", "true");
            //options.AddAdditionalCapability("profile.default_content_settings.geolocation", 0);

            if (!isRemote)
            {
                options.AddArguments("start-maximized");
                options.AddArguments("test-type");
            }

            options.AddArgument("lang=" + Configuration.BrowserLang);
            if (Convert.ToBoolean(Configuration.StartProxyServer))
            {
                options.AddArgument("proxy-server=" + seleniumProxy.HttpProxy);
            }

            return options;
        }

        private static RemoteWebDriver GetLocalDriver(Proxy seleniumProxy, string downloadFolder)
        {
            RemoteWebDriver driver = null;
            var options = GetChromeOptions(false, seleniumProxy, downloadFolder);
            var chromeDriverPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    driver = new ChromeDriver(chromeDriverPath, options, new TimeSpan(0, 5, 0));
                    break;
                }
                catch (Exception e)
                {
                    if (i != 2)
                    {
                        Log.Info($"Failed to create {BrowserFactory.BrowserType.Chrome} Driver instance, attempt #{i + 1}: {e.Message}");
                    }
                    else
                    {
                        Log.Fail($"Failed to create {BrowserFactory.BrowserType.Chrome}Driver instance on the 3rd attempt. Message: {e.Message}");
                    }
                }
            }

            return driver;
        }
    }
}
