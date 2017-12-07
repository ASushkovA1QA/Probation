using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// class to set the settings screenshot
    /// </summary>
    public class ScreenShotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        /// <summary> 
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen. 
        /// </summary> 
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns> 
        public ScreenShotRemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities)
        {
        }

        /// <summary> 
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen. 
        /// </summary> 
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns> 
        public ScreenShotRemoteWebDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
        }

        /// <summary> 
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen. 
        /// </summary> 
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns> 
        public ScreenShotRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }

        /// <summary> 
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen. 
        /// </summary> 
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns> 
        public ScreenShotRemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
        }

        /// <summary>
        /// get screenshot
        /// </summary>
        /// <returns>screenshot</returns>
        public new Screenshot GetScreenshot()
        {
            // Get the screenshot as base64. 
            Response screenshotResponse = Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();
            // ... and convert it. 
            return new Screenshot(base64);
        }
    }
}
