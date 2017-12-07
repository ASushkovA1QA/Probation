using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebdriverFramework.Framework.WebDriver.Elements;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// base class for creating forms, containing the necessary methods for their use
    /// </summary>
    public class BaseForm: BaseEntity
    {
        /// <summary>
        /// Locator for indentification of the form
        /// </summary>
        protected By TitleLocator;

        /// <summary>
        /// Name of the form
        /// </summary>
        protected string Title;
        private static long _counter;
        /// <summary>
        /// Name of the Form/Page
        /// Allow make log more readable
        /// </summary>
        public static string TitleForm; 
       
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="titleLocator">locator for the page</param>
        /// <param name="title">title for the page</param>
        protected BaseForm(By titleLocator, string title)
        {
            TitleLocator = titleLocator;
            Title = TitleForm= title;
            AssertIsOpen(); 
        }

        /// <summary>
        /// wait until element is absent
        /// </summary>
        /// <param name="seconds">count of seconds to wait for the absence of an element on the page</param>
        public void WaitForIsAbsent(double seconds)
        {
            try
            {
                var wait = new WebDriverWait(Browser.GetDriver(), TimeSpan.FromSeconds(seconds));
                wait.Until(d => (Browser.GetDriver().FindElements(TitleLocator).Count <= 0));
            }
            catch (TimeoutException ex)
            {
                Log.Warn(ex.Message);
                Log.Fail(Title + " is still present ");
            }
        }

        /// <summary>
        /// awaits the locator form (timeoutForElementWaiting) completes the test or if the element is not found
        /// </summary>
        public void AssertIsOpen()
        {
            var label = new Label(TitleLocator, Title);
            try
            {
                label.WaitForElementIsPresent();
                label.WaitForIsEnabled(); //added 
                Info("appears");
            }
            catch (Exception ex)
            {
                if (!label.IsPresent())
                {
                    Fatal("doesn't appear \n" + ex.Message);
                }
            }
            finally
            {
                Browser.SaveScreenShot(TitleForm + "_" + _counter++);
            }
        }

        /// <summary>
        ///  assertion what page is closed
        /// </summary>
        public void AssertIsClosed()
        {
            var label = new Label(TitleLocator, Title);
            label.AssertIsAbsent();
        }

        /// <summary>
        /// formats the value for logging "name page - log splitter - the message"
        /// </summary>
        /// <param name="message">message for format</param>
        /// <returns>a formatted value for logging "element type - name - log splitter - the message"</returns>
        protected override string FormatLogMsg(string message)
        {
            return string.Format("Form '{0}' {1} {2}", Title, Logger.LogDelimiter, message);
        }

        /// <summary>
        /// click main menu item
        /// </summary>
        /// <param name="menuItemText">menu item text</param>
        public void ClickMainMenuItem(string menuItemText)
        {
            string btnMenuItemXPath = string.Format("//a[contains(.,'{0}')]", menuItemText);
            var btnMenuItem = new Button(By.XPath(btnMenuItemXPath), menuItemText);
            btnMenuItem.Click();
        }
    }
}
