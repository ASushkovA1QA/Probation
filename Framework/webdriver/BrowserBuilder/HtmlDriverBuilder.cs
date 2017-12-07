using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public class HtmlDriverBuilder : IWebDriverBuilder
    {
        public RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            var capsHt = DesiredCapabilities.HtmlUnit();
            capsHt.IsJavaScriptEnabled = true;
            var driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"), capsHt);
            return driver;
        }
    }
}
