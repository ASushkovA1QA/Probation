using System.Collections.Generic;
using OpenQA.Selenium;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// executes javascripts
    /// </summary>
    public class JsExecutor : BaseEntity
    {
        /// <summary>
        /// execute some javascript
        /// </summary>
        /// <param name="jScript"></param>
        /// <param name="logInfo">information for log</param>
        public static void ExecuteJs(string jScript, string logInfo)
        {
            Logger.Instance.Info(logInfo);
            Logger.Instance.Debug("Execute scrips:" + jScript);
            ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript(jScript);
            Browser.WaitForPageToLoad();
        }
        /// <summary>
        /// execute some javascript
        /// </summary>
        /// <param name="jScript">specified javascript</param>
        /// <param name="logInfo">information for log</param>
        /// <returns>result of specified script</returns>
        public static object ReturnExecutesJsResult(string jScript, string logInfo)
        {
            Logger.Instance.Info(logInfo);
            Logger.Instance.Debug("Execute script:" + jScript);
            var result = ((IJavaScriptExecutor)Browser.GetDriver()).ExecuteScript(jScript);
            Browser.WaitForPageToLoad();
            return result;
        }

        /// <summary>
        /// accept all alerts on the page
        /// </summary>
        public static void AcceptAlertsOnThePage()
        {
            ExecuteJs("window.confirm = function () { return true }", "Accept alert message");
        }
        /// <summary>
        /// format log message
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>"JS Executor: " + message</returns>
        protected override string FormatLogMsg(string message)
        {
            return "JS Executor: " + message;
        }
    }
}
