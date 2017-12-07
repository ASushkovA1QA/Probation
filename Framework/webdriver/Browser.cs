using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using WebdriverFramework.Framework.util;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// class for setting parameters of the browser
    /// </summary>
    public class Browser
    {
        private static Browser _currentInstance;
        private static Browser _firstInstance;
        private static Browser _secondInstance;
        private static RemoteWebDriver _currentDriver;
        private static RemoteWebDriver _firstDriver;
        private static int _counter = 1;
        private static readonly Logger Log = Logger.Instance;
        private static bool _isRemote;
        private static Uri _remoteUri;

        /// <summary>
        /// stores value of the current Browser type
        /// </summary>
        public static BrowserFactory.BrowserType CurrentBrowser;

        private static string _propBrowser;
        /// <summary>
        /// stores value of the implicity wait
        /// </summary>
        public static double ImplWait;
        private static double _timeoutForPageToLoad;
        /// <summary>
        /// stores value of the time when autotest will be wait until element exists
        /// </summary>
        public static double TimeoutForElementWaiting;
       /// <summary>
       /// stores value to the base application url
       /// very often it is login page
       /// </summary>
        public static string LoginPage;

        /// <summary>
        /// directory to store any artifacts of tests
        /// for example: screenshots of the steps
        /// </summary>
        public const string ActiveDir = "..\\artifacts";

        /// <summary>
        /// constructor
        /// </summary>
        private Browser()
        {
            InitProperties();
            _firstDriver = BrowserFactory.SetUp(_propBrowser, _isRemote, _remoteUri, out CurrentBrowser);
            _firstDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(ImplWait));
            Logger.Instance.Info(string.Format("Instance of Browser '{0}' was constructed", CurrentBrowser));
            _currentDriver = _firstDriver;
        }

        /// <summary>
        /// read params
        /// </summary>
        private static void InitProperties()
        {
            //read params
            LoginPage = Configuration.StartUrl;
            ImplWait = 0;
            _timeoutForPageToLoad = Convert.ToDouble(Configuration.PageTimeout);
            TimeoutForElementWaiting = Convert.ToDouble(Configuration.ElementTimeout);
            _propBrowser = Configuration.Browser;
            _isRemote = Configuration.Remote;
            _remoteUri = Configuration.GridHub;
        }

        /// <summary>
        /// returns default timeout for wait elements
        /// migth be used for other waitings
        /// </summary>
        /// <returns>timeout</returns>
        public static TimeSpan GetElementTimeoutInSeconds()
        {
            return TimeSpan.FromSeconds(Convert.ToDouble(Configuration.ElementTimeout));
        }

        /// <summary>
        /// change the current settings Browser
        /// </summary>
        public void SwitchToFirstInstance()
        {
            _currentInstance = _firstInstance;
            _currentDriver = _firstDriver;
        }

        /// <summary>
        /// singleton
        /// </summary>
        public static Browser Instance
        {
            get { return _currentInstance ?? (_currentInstance = new Browser()); }
        }

        /// <summary>
        /// start page navigation
        /// </summary>
        public void NavigateToStartPage()
        {
            _currentDriver.Navigate().GoToUrl(LoginPage);
            WaitForPageToLoad();
        }

        /// <summary>
        /// wait for page to load
        /// </summary>
        public void WaitForPageToLoad()
        {
            var wait = new WebDriverWait(_currentDriver, TimeSpan.FromSeconds(Convert.ToDouble(_timeoutForPageToLoad)));
            try
            {
                wait.Until(waiting =>
                {
                    var result =
                        ((IJavaScriptExecutor) _currentDriver).ExecuteScript(
                            "return document['readyState'] ? 'complete' == document.readyState : true");
                    return result is Boolean && (Boolean) result;
                });
            }
            catch (Exception e)
            {
                Logger.Instance.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// waits for alert
        /// <returns>alert</returns>
        /// </summary>
        private IAlert WaitForAlert()
        {
            var wait = new WebDriverWait(_currentDriver, GetElementTimeoutInSeconds());
            try
            {
                IAlert alert = wait.Until(waiting =>
                {
                    try
                    {
                        IAlert expectedAlert = _currentDriver.SwitchTo().Alert();
                        return expectedAlert;
                    }
                    catch (NoAlertPresentException)
                    {
                        return null;
                    }
                });
                return alert;
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception("No alert was shown in " + GetElementTimeoutInSeconds());
            }
        }

        /// <summary>
        /// wait until alert is present and the click Accept button
        /// <returns>text from alert</returns>
        /// </summary>
        public string WaitAlertAndAccept()
        {
            var alert = WaitForAlert();
            string text = alert.Text;
            Logger.Instance.Info("Text in the Alert: " + text);
            alert.Accept();
            WaitForPageToLoad();
            return text;
        }

        /// <summary>
        /// Wait before spenner is not shown
        /// </summary>
        public void WaitForSpinnerAppears()
        {
            var wait = new WebDriverWait(_currentDriver, TimeSpan.FromSeconds(5));
            // waiting before spinner appears
            try
            {
                wait.Until(waiting =>
                {
                    var spinnerDisplayed = _currentDriver.FindElement(By.XPath("//img[contains(@src,'spinner.gif')]")).Displayed;
                    return spinnerDisplayed;
                });
            }
            catch (Exception e)
            {
                Logger.Instance.Debug(e.StackTrace);
            }
        }


        /// <summary>
        /// waits until page url will be changed
        /// </summary>
        /// <param name="oldUrl">old value of page url</param>
        public void WaitForUrlChange(string oldUrl)
        {
            var wait = new WebDriverWait(_currentDriver, TimeSpan.FromSeconds(5));
            // waiting before spinner appears
            try
            {
                wait.Until(waiting =>
                {
                    var cuurentUrl = _currentDriver.Url;
                    return !oldUrl.Equals(cuurentUrl);
                });
            }
            catch (Exception e)
            {
                Logger.Instance.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// waits until url will contains some value
        /// </summary>
        /// <param name="value">part of url</param>
        public void WaitForUrlContains(string value)
        {
            var wait = new WebDriverWait(_currentDriver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(waiting =>
                {
                    var cuurentUrl = _currentDriver.Url;
                    return !cuurentUrl.Contains(value);
                });
            }
            catch (Exception e)
            {
                Logger.Instance.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// refreshes page
        /// </summary>
        public static void Refresh()
        {
            Logger.Instance.Info("Browser refresh");
            _currentDriver.Navigate().Refresh();
        }

        /// <summary>
        /// get driver
        /// </summary>
        /// <returns>current driver</returns>
        public RemoteWebDriver GetDriver()
        {
            return _currentDriver;
        }

        /// <summary>
        /// switch frame by index
        /// </summary>
        /// <param name="index">index</param>
        public void SwitchFrameByIndex(int index)
        {
            _currentDriver.SwitchTo().Frame(index);
            _currentDriver.SwitchTo().Frame("redirect-url");
            Logger.Instance.Info(string.Format("Switching to frame '{0}' ", index));
        }

        /// <summary>
        /// switch frame by id
        /// </summary>
        /// <param name="id">id</param>
        public void SwitchFrameById(string id)
        {
            _currentDriver.SwitchTo().Frame(id);
            Logger.Instance.Info(string.Format("Switching to frame '{0}' ", id));
        }

        /// <summary>
        /// switch to last window
        /// </summary>
        public void SwtichToLastWindow()
        {
            var availableWindows = _currentDriver.WindowHandles;
            if (availableWindows.Count > 1)
            {
                _currentDriver.SwitchTo().Window(availableWindows[availableWindows.Count - 1]);
            }

        }

        /// <summary>
        /// switch to first window
        /// </summary>
        public void SwtichToFirstWindow()
        {
            _currentDriver.SwitchTo().Window(_currentDriver.WindowHandles[0]);
        }

        /// <summary>
        /// quit
        /// </summary>
        public void Quit()
        {
            if (_currentDriver != null)
            {
                SaveScreenShot(GetType().Name + _counter++);
                _firstDriver.Quit();
                _firstInstance = null;
                _currentInstance = null;
                _currentDriver = null;
                Logger.Instance.Info(string.Format("Instance of Browser '{0}' was destroyed", CurrentBrowser));
                if (_secondInstance == null) return;
                _secondInstance = null;
            }
        }

        /// <summary>
        /// get screenshot
        /// </summary>
        /// <returns>screenshot</returns>
        public static Screenshot GetScreenshot()
        {
            //remote or not
            var screenShot = _currentDriver.GetType() == typeof(ScreenShotRemoteWebDriver) ? ((ScreenShotRemoteWebDriver)_currentDriver).GetScreenshot() : ((ITakesScreenshot)_currentDriver).GetScreenshot();
            return screenShot;
        }

        /// <summary>
        /// fullscreen
        /// now this works with the standart function for all possible browsers: IE, FireFox and Chrome
        /// </summary>
        public void WindowMaximise()
        {
            if (!Configuration.MobileTesting || Configuration.Browser.ToLower() != "browserstack")
                _currentDriver.Manage().Window.Maximize();
            //((IJavaScriptExecutor)_currentDriver).ExecuteScript("if (window.screen) {window.moveTo(0, 0);window.resizeTo(window.screen.availWidth,window.screen.availHeight);};");
        }
        /// <summary>
        /// sleep process
        /// </summary>
        /// <param name="mSeconds">the number of seconds for sleep</param>
        public static void Sleep(int mSeconds)
        {
            Thread.Sleep(mSeconds);
        }

        /// <summary>
        /// sleep process
        /// </summary>
        public void Sleep()
        {
            Thread.Sleep(3000);
        }

        /// <summary>
        /// save screenshot
        /// </summary>
        /// <param name="fileName">name screenshot</param>
        public static void SaveScreenShot(string fileName)
        {
            var screenshot = GetScreenshot();
            //Create a new subfolder under the current active folder
            //var newPath = System.IO.Path.Combine(ActiveDir, Configuration.ScreenShotDir);
            string newPath = Path.Combine(Directory.GetCurrentDirectory(),ActiveDir);
            // Create the subfolder
            Directory.CreateDirectory(newPath);
            newPath = Path.Combine(newPath, fileName + ".jpg");
            screenshot.SaveAsFile(newPath, ImageFormat.Jpeg);
            Logger.Instance.Info(string.Format("Screenshot was saved '{0}'", newPath));
            System.Diagnostics.Debug.Write(string.Format("##teamcity[publishArtifacts 'RealCapitalMarkets/RealCapitalMarkets/bin/artifacts/screenshots/{0}']", fileName + ".jpg"));
        }

        /// <summary>
        /// save screenshot
        /// </summary>
        /// <param name="fileName">name screenshot</param>
        public static string SaveAndGetPathToScreenShot(string fileName)
        {
            var screenshot = GetScreenshot();
            //Create a new subfolder under the current active folder
            //var newPath = System.IO.Path.Combine(ActiveDir, Configuration.ScreenShotDir);
            string newPath = Path.Combine(Directory.GetCurrentDirectory(), ActiveDir);
            // Create the subfolder
            Directory.CreateDirectory(newPath);
            newPath = Path.Combine(newPath, fileName + ".jpg");
            screenshot.SaveAsFile(newPath, ImageFormat.Jpeg);
            Logger.Instance.Info(string.Format("Screenshot was saved '{0}'", newPath));
            System.Diagnostics.Debug.Write(string.Format("##teamcity[publishArtifacts 'RealCapitalMarkets/RealCapitalMarkets/bin/artifacts/screenshots/{0}']", fileName + ".jpg"));
            return newPath;
        }
        
        /// <summary>
        /// Navigate to page with URL "url"
        /// </summary>
        /// <param name="url">Page URL</param>
        public static void NavigateTo(string url)
        {
            Logger.Instance.Info("Navigate to url:" + url);
            _currentDriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Navigate to back page
        /// </summary>
        public static void NavigateBack()
        {
            Logger.Instance.Info("Navigate to back page");
            _currentDriver.Navigate().Back();
        }

        /// <summary>
        /// waiting, while number of open windows will be more than previous
        /// </summary>
        /// <param name="prevWndCount">number of windows before some action</param>
        public void WaitForNewWindow(int prevWndCount)
        {
            int wndCount = prevWndCount;
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(TimeoutForElementWaiting));
            wait.Until(d => d.WindowHandles.Count > wndCount);
        }

        /// <summary>
        /// waiting, while number of open windows will be smaler than previous
        /// </summary>
        /// <param name="prevWndCount">number of windows before some action</param>
        public void WaitForWindowClose(int prevWndCount)
        {
            int wndCount = prevWndCount;
            var wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(TimeoutForElementWaiting));
            wait.Until(d => d.WindowHandles.Count < wndCount);
        }

        /// <summary>
        /// Get number of open windows
        /// </summary>
        /// <returns>number of open windows</returns>
        public int WindowCount()
        {
            return GetDriver().WindowHandles.Count;
        }

        /// <summary>
        /// Switch to the window with index
        /// </summary>
        /// <param name="index">index of the window</param>
        public void SwitchWindow(int index)
        {
            GetDriver().SwitchTo().Window(GetDriver().WindowHandles.ToArray()[index]);
        }

        public static RemoteWebDriver GetDriverStatic()
        {
            return _currentDriver;
        }
        public static int GetTotalWidth()
        {
            var totalwidth = (long)GetDriverStatic().ExecuteScript("return document.body.offsetWidth");
            return (int)totalwidth;
        }

        public static int GetTotalHeight()
        {
            var totalHeight = (long)GetDriverStatic().ExecuteScript("return  document.body.parentNode.scrollHeight");
            return (int)totalHeight;
        }

        public static void ScrollToTopViaJs()
        {
            Log.Info("Scrolling to the top of the page, URL=" + GetCurrentUrl());
            JsExecutor.ExecuteJs("window.scrollTo(0, 0)", "Scroll to top via Js");
        }

        public static void ScrollToDownAndRightViaJs()
        {
            Log.Info("Scrolling to the down and right of the page, URL=" + GetCurrentUrl());
            JsExecutor.ExecuteJs("window.scrollTo(document.body.scrollWidth, document.body.scrollHeight)", "Scroll to down and right");
        }
        public static string GetCurrentUrl()
        {
            try
            {
                return GetDriverStatic().Url;
            }
            catch (Exception e)
            {
                Log.Warn("Не удалось получить текущий url путем: GetDriverStatic().Url \r\n" + e.Message);
                return GetDriverStatic().ExecuteScript("return document.URL;").ToString();
            }
        }
    }
}
