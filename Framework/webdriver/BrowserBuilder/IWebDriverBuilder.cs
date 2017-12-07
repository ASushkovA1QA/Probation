using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver.BrowserBuilder
{
    public interface IWebDriverBuilder
    {
        RemoteWebDriver GetDriver(bool isRemote, Uri link, Proxy seleniumProxy, string downloadFolder);
    }
}
