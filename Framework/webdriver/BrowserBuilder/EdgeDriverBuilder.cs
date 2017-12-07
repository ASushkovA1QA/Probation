using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public class EdgeDriverBuilder : IWebDriverBuilder
    {
        public RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder)
        {
            var options = new EdgeOptions {PageLoadStrategy = EdgePageLoadStrategy.Eager};
            return new EdgeDriver(Configuration.EdgeDriverPath, options, new TimeSpan(0, 5, 0));   
        }
    }
}
