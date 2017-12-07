using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// base class to describe the forms of dialogue
    /// </summary>
    class BaseDialog : BaseForm
    {

        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="titleLocator">locator</param>
        /// <param name="title">title of the dialog</param>
       protected BaseDialog(By titleLocator, string title)
            : base(titleLocator, title)
        {
        }
        
        /// <summary>
        /// formats the value for logging "element type - name - log splitter - the message"
        /// </summary>
       /// <param name="message">message for format</param>
       /// <returns>a formatted value for logging "element type - name - log splitter - the message"</returns>
        protected override string FormatLogMsg(string message)
        {
            return string.Format("Dialog '{0}' {1} {2}", Title, Logger.LogDelimiter, message);
        }
    }
}
