using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebdriverFramework.Framework.util;
using OpenQA.Selenium.Chrome;
using WebdriverFramework.Framework.WebDriver.Elements;
using System.Drawing.Imaging;
using OpenQA.Selenium;
using WebdriverFramework.Framework.Util;
using System.Collections.Specialized;
using System.Net;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// Base test class. Does browser initialization and closes it after test is finished.
    /// </summary>
    [TestClass]
    public abstract class BaseTest: BaseEntity
    {
        protected static String urlAPI = ConfigReader.GetUrlAPI();
        protected static String urlHomePage = ConfigReader.GetUrlHomePage();
        protected static String requestToken = ConfigReader.GetPathToken();
        protected static String requestCsvTestList = ConfigReader.GetPathCsvTestList();
        protected static String login = ConfigReader.GetLoginHomePage();
        protected static String password = ConfigReader.GetPasswordHomePage();
        protected static String tokenValueName = ConfigReader.GetTokenValueName();
        protected static String tokenValue = ConfigReader.GetTokenValue();
        protected static String projectValueName = ConfigReader.GetProjectValueName();
        protected static String projectValue = ConfigReader.GetProjectValue();
        protected static String cookiePath = ConfigReader.GetCookiePath();
        protected static int projNameLength = Convert.ToInt32(ConfigReader.GetProjNameLength());
        protected static String requestAddTest = ConfigReader.GetPathTest();
        protected static String requestAddTestLog = ConfigReader.GetPathTestLog();
        protected static String requestAddTestAttach = ConfigReader.GetPathTestAttach();
        protected static String requestSID = ConfigReader.GetSID();
        protected static String requestProjName = ConfigReader.GetProjectName();
        protected static String requestTestName = ConfigReader.GetTestName();
        protected static String requestMethodName = ConfigReader.GetMethodName();
        protected static String requestHostName = ConfigReader.GetHostname();
        protected static String requestTestIdName = ConfigReader.GetTestIdName();
        protected static String requestContentName = ConfigReader.GetContentName();
        protected static String requestContentTypeName = ConfigReader.GetContentTypeName();
        protected static String requestContentType = ConfigReader.GetContentType();
        protected static String newTestStatus = ConfigReader.GetNewTestStatus();
        protected static String attrTokenName = ConfigReader.GetAttrTokenName();
        protected static String projNameDefault = ConfigReader.GetProjNameDefault();
        

        protected String urlHomePageAuth = new Regex("//").Replace(urlHomePage, "//" + login + ":" + password + "@");

        private readonly string _testTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        private static string PathToDownloadDirectory { get; set; }

        public static StringBuilder Content = new StringBuilder();
        /// <summary>
        /// allow marks test as failure or success
        /// if HasWarn has true value test will be failed in the end
        /// this variable used in the Checker class for make soft assertions
        /// </summary>
        public static bool HasWarn;
        /// <summary>
        /// Context of the current test execution
        /// </summary>
        public virtual TestContext TestContext { get; set; }
        
        /// <summary>
        /// override method toString()
        /// </summary>
        /// <returns>name</returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Initialization before test case.
        /// </summary>
        [TestInitialize]
        public virtual void InitBeforeTest()
        {
            Checker.Messages = new CheckMessList();
            HasWarn = false;
            Browser = Browser.Instance;
            Browser.NavigateToStartPage();
            Log.TestName(GetType().Name);
            try
            {
                Browser.WindowMaximise();
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("Session not started or terminated")) // Sometimes browserstack terminates connection unexpectedly. Just workaroud below
                {
                    var type = typeof(Browser);
                    type.GetField("_currentInstance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
                    type.GetField("_currentDriver", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
                    type.GetField("_firstInstance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
                    InitBeforeTest();
                }
            }

            SetPreconditions();
        }

        /// <summary>
        /// should be implemented in childs
        /// </summary>
        public abstract void RunTest();

        /// <summary>
        /// получает список некритичных сообщений и выводит в лог
        /// </summary>
        protected void ProcessingErrors()
        {
            if (HasWarn)
            {
                String allMessages = "";
                Info("====================== See autotest errors below ===============");
                for (int i = 0; i < Checker.Messages.Count; i++)
                {
                    allMessages = allMessages + Checker.Messages[i];
                    Error("Error " + (i + 1) +" : " + Checker.Messages[i]);
                }
                Info("====================== end of errors ======================");
                Assert.IsFalse(HasWarn, allMessages);
            }
        }

        /// <summary>
        /// closing browser
        /// </summary>
        [TestCleanup]
        public virtual void CleanAfterTest()
        {
            try
            {
                AddDataForHtmlreport();
                ProcessingErrors();
            }
            catch (Exception e)
            {
                Logger.Instance.Fail("Test was finished but there are some issues with result analyzing. Please check that all right configured\r\n" +
                                     "Exception:" + e.Message);
            }
            finally
            {
                SQLConnection.CloseConnectons();
                if (Browser != null)
                {
                    Browser.Quit(); 
                }
            }
        }

        /// <summary>
        /// add data for creating html report
        /// </summary>
        protected void AddDataForHtmlreport()
        {
            const int maxMessageLength = 1200;
            const string testdatatmp = "<tr><td>{0}</td><td style='color: {1}'>{2}</td><td><a href='{3}'>Screen</a></td><td><a href='{4}'>Test data</a></td><td>{5}</td></tr>";
            //string path;

            // add test's result row
            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && !HasWarn)
            {
                //Content.AppendLine(string.Format(testdatatmp, GetTestName(), "green", "Passed", path, GetDownloadDirectory().Replace(Configuration.DriveTestData + ":\\", "\\\\" + Environment.MachineName + "\\"), PathToVideo));
                Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>> PASSED <<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                var message = Checker.Messages.Aggregate("", (current, m) => current + m);
                message = Regex.Replace(message, "\r\n", "<br/>").Replace("\n", "<br/>");
                message = message.Length < maxMessageLength ? message : message.Substring(0, maxMessageLength);
                Content.AppendLine(string.Format(testdatatmp, GetTestName(), "red", "Failed", /*path,*/ GetDownloadDirectory().Replace(Configuration.DriveTestData + ":\\", "\\\\" + Environment.MachineName + "\\"), message));
                Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>> TEST FAILED <<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
        }

        protected void SetPreconditions()
        {
            if (Browser.GetDriver() is ChromeDriver)
            {
                Browser.GetDriver().Navigate().GoToUrl("chrome://settings/content");
                Browser.GetDriver().SwitchTo().Frame("settings");
                RadioButton location = new RadioButton(By.XPath("//*[@name='location' and @value='allow']"), "Allow");
                System.Threading.Thread.Sleep(500);
                location.ClickJs();
                Browser.GetDriver().FindElement(By.XPath("//*[@id='content-settings-overlay-confirm']")).Click();
            }

        }

        /// <summary>
        /// возвращает название класса теста
        /// </summary>
        /// <returns>название класса теста</returns>
        protected string GetTestName()
        {
            return GetType().ToString().Split('.')[GetType().ToString().Split('.').Length - 1];
        }

        /// <summary>
        /// формирует путь к загрузочной директории
        /// </summary>
        /// <returns>путь</returns>
        private string GetDownloadDirectory()
        {
            var testname = GetTestName();
            var path = Path.Combine(BrowserFactory.GetDownloadFolder(),
                                        testname + "_" + _testTime + @"\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            SetPath(path);
            return path;
        }

        /// <summary>
        /// статический метод для инициализации пути к директории загрузки
        /// </summary>
        /// <param name="path">путь</param>
        public static void SetPath(string path)
        {
            PathToDownloadDirectory = path;
        }

        /// <summary>
        /// formats the value for logging "name test - log splitter - the message"
        /// </summary>
        /// <param name="message">message for format</param>
        /// <returns>a formatted value for logging "element type - name - log splitter - the message"</returns>
        protected override string FormatLogMsg(string message)
        {
            return message;
        }

        protected OpenQA.Selenium.Cookie SetBrowserCookie(string token)
        {
            var cookieToken = new OpenQA.Selenium.Cookie(attrTokenName, token, cookiePath);
            Browser.GetDriver().Manage().Cookies.AddCookie(cookieToken);
            return cookieToken;
        }

        protected String GetCsvTestList()
        {
            var values = new NameValueCollection();
            values[projectValueName] = projectValue;
            string csvTestList = HttpUtil.PostRequest(urlAPI + requestCsvTestList, values);
            return csvTestList;
        }

        protected String SendTestIdRequest(string randProjName)
        {
            var values = new NameValueCollection();
            values[requestSID] = RegexUtil.GetRandomAlphaNumeric(projNameLength);
            values[requestProjName] = randProjName;
            values[requestTestName] = requestTestName;
            values[requestMethodName] = requestMethodName;
            values[requestHostName] = Dns.GetHostName();
            var testId = HttpUtil.PostRequest(urlAPI + requestAddTest, values);
            return testId;
        }

        protected String SendLogRequestWithTestId(string testId)
        {
            var values = new NameValueCollection();
            var logPath = Directory.GetCurrentDirectory() + ConfigReader.GetLogName();
            values[requestTestIdName] = testId;
            values[requestContentName] = logPath;
            HttpUtil.PostRequest(urlAPI + requestAddTestLog, values);
            return logPath;
        }

        protected String SendImgRequestWithTestId(string testId)
        {
            var values = new NameValueCollection();
            var filePath = Directory.GetCurrentDirectory() + ConfigReader.GetScreenshotName();
            Browser.GetScreenshot().SaveAsFile(filePath, ImageFormat.Jpeg);
            var imageFile = File.ReadAllBytes(filePath);
            var base64ImgFile = Convert.ToBase64String(imageFile);
            values[requestTestIdName] = testId;
            values[requestContentName] = base64ImgFile;
            values[requestContentTypeName] = requestContentType;
            HttpUtil.PostRequest(urlAPI + requestAddTestAttach, values);
            return base64ImgFile;
        }
    }
}

