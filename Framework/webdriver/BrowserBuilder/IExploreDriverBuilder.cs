using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public class IExploreDriverBuilder : IWebDriverBuilder
    {
        private static readonly Logger Log = Logger.Instance;
        public RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            return isRemote ? GetRemoteDriver(link) : GetLocalDriver();
        }

        private static RemoteWebDriver GetRemoteDriver(Uri link)
        {
            var caps = DesiredCapabilities.InternetExplorer();
            caps.SetCapability(CapabilityType.TakesScreenshot, true);
            caps.SetCapability("ignoreProtectedModeSettings", true);
            caps.IsJavaScriptEnabled = true;
            return new ScreenShotRemoteWebDriver(link, caps);
        }
        
        private static RemoteWebDriver GetLocalDriver()
        {
            RemoteWebDriver driver = null;
            var options = new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true, EnsureCleanSession = true };
            options.AddAdditionalCapability("javascriptEnabled", true);
            options.AddAdditionalCapability("handlesAlerts", true);
            var ieDriverPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    driver = new InternetExplorerDriver(ieDriverPath, options, new TimeSpan(0, 5, 0));
                    break;
                }
                catch (Exception e)
                {
                    if (i != 2)
                    {
                        Log.Info($"Failed to create {BrowserFactory.BrowserType.Iexplore} Driver instance, attempt #{i + 1}: {e.Message}");
                    }
                    else
                    {
                        Log.Fail($"Failed to create {BrowserFactory.BrowserType.Iexplore}Driver instance on the 3rd attempt. Message: {e.Message}");
                    }
                }
            }

            return driver;
        }
    }
}
