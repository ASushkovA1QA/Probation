using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "CheckBox"
    /// </summary>
    public class CheckBox: BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the checkBox</param>
        /// <param name="name">name of the checkBox</param>
        public CheckBox(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the checkBox 
        /// </summary>
        /// <returns>type of the checkBox</returns>
        protected override string GetElementType()
        {
            return "Checkbox";
        }

        /// <summary>
        /// assert state with logging
        /// </summary>
        /// <param name="expectedState">expected state</param>
        public void AssertStatePosMsg(bool expectedState)
        {
            KAssert.AssertEqualsPosMsg(FormatLogMsg(" state equals " + expectedState), expectedState, IsChecked());
        }

        /// <summary>
        /// assert state without logging
        /// </summary>
        /// <param name="expectedState"></param>
        public void AssertState(bool expectedState)
        {
            KAssert.AssertEquals(FormatLogMsg(" state equals " + expectedState), expectedState, IsChecked());
        }

        /// <summary>
        /// set value​​(to check whether it is necessary to change the current)
        /// </summary>
        /// <param name="state">value(true or false)</param>
        public void Check(bool state)
        {
            WaitForElementIsPresent();
            Info(string.Format("setting state '{0}'",state));
            if(state&&!Element.Selected)
            {
                Element.Click();
            }else if(!state&&Element.Selected)
            {
                Element.Click();
            }
            WaitForStateChanged(state);
        }

        private void WaitForStateChanged(bool state)
        {
            var wait = new WebDriverWait(Browser.GetDriver(), Browser.GetElementTimeoutInSeconds());
            Browser.WaitForPageToLoad();
            try
            {
                wait.Until(waiting =>
                {
                    try
                    {
                        Element = Browser.GetDriver().FindElement(Locator);
                        return Element.Selected == state;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception("State of checked checkbox " + Name + " was not changed! Expected state " + state);
            }
        }

        /// <summary>
        /// uncheck the checkBox
        /// </summary>
        public void Uncheck()
        {
            Check(false);
        }

        /// <summary>
        /// check the checkBox
        /// </summary>
        public void Check()
        {
            Check(true);
        }

        /// <summary>
        /// checks the selected or not
        /// </summary>
        /// <returns>true if checkbox is checked, else - false</returns>
        private bool IsChecked()
        {
            WaitForElementIsPresent();
            return Element.Selected;
        }

        /// <summary>
        /// set checkbox to state
        /// </summary>
        /// <param name="state"></param>
        public void CheckJs(bool state)
        {
            WaitForElementExists();
            Log.Info(string.Format("setting state '{0}' for checkbox " + Name, state));
            // get current state of checkbox
            var currentState = (bool)Browser.GetDriver().ExecuteScript("return arguments[0].checked", Element);
            if (currentState != state)
            {
                Browser.GetDriver().ExecuteScript("arguments[0].click();", Element);
                Browser.WaitForPageToLoad();
            }
            currentState = (bool)Browser.GetDriver().ExecuteScript("return arguments[0].checked", Element);
            Log.Info(string.Format("current state is '{0}' for checkbox " + Name, currentState));
        }
    }
}
