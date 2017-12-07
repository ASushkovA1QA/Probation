using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public class FirefoxDriverBuilder : IWebDriverBuilder
    {
        public RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            RemoteWebDriver driver;
            var profile = GetProfile(downloadFolder);
            if (isRemote)
            {
                var caps = DesiredCapabilities.Firefox();
                caps.SetCapability(CapabilityType.TakesScreenshot, true);
                caps.IsJavaScriptEnabled = true;
                caps.SetCapability(CapabilityType.Proxy, seleniumProxy);
                caps.SetCapability(FirefoxDriver.ProfileCapabilityName, profile);
                driver = new ScreenShotRemoteWebDriver(link, caps);
            }
            else
            {
                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"C:\", "wires.exe");
                service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                driver = new FirefoxDriver(service);
            }

            return driver;
        }

        private FirefoxProfile GetProfile(string downloadFolder)
        {
            var profile = new FirefoxProfile();
            profile.SetPreference("network.proxy.type", (int)ProxyKind.AutoDetect);
            profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            profile.SetPreference("browser.download.folderList", 2);
            profile.SetPreference("browser.download.dir", string.Empty + downloadFolder);
            profile.SetPreference("services.sync.prefs.sync.browser.download.manager.showWhenStarting", false);
            profile.SetPreference("browser.download.useDownloadDir", true);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/zip, application/x-gzip, application/octet-stream, application/x-msdownload");
            profile.SetPreference("general.useragent.override", "Mozilla/5.0 (Windows NT 6.0) Gecko/20100101 Firefox/34.0/KPC Automation");
            profile.SetPreference("intl.accept_languages", Configuration.BrowserLang);

            profile.SetPreference("browser.history.allowPopState", false);
            profile.SetPreference("privacy.popups.policy", "0");
            profile.SetPreference("geo.enabled", false);
            profile.SetPreference("geo.prompt.testing.allow", true);

            return profile;
        }
    }
}
