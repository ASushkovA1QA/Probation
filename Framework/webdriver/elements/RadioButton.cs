using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "radio button"
    /// </summary>
    public class RadioButton: BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the radio button</param>
        /// <param name="name">name of the radio button</param>
        public RadioButton(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the radio button 
        /// </summary>
        /// <returns>type of the radio button</returns>
        protected override string GetElementType()
        {
            return "RadioButton";
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
        /// ssert state without logging
        /// </summary>
        /// <param name="expectedState">expected state</param>
        public void AssertState(bool expectedState)
        {
            KAssert.AssertEquals(FormatLogMsg(" state equals " + expectedState), expectedState, IsChecked());
        }

        /// <summary>
        /// set value (with checking)
        /// </summary>
        /// <param name="state">state(true or false)</param>
        public void Check(bool state)
        {
            WaitForElementIsPresent();
            Info(string.Format("setting state '{0}'", state));
            if (state && !Element.Selected)
            {
                Element.Click();
            }
            else if (!state && Element.Selected)
            {
                Element.Click();
            }
        }

        /// <summary>
        /// check radio button is true
        /// </summary>
        public void Check()
        {
            Check(true);
        }

        /// <summary>
        /// asserts element is present and true if checked
        /// </summary>
        /// <returns>true if checked else false</returns>
        private bool IsChecked()
        {
            AssertIsPresent();
            return Element.Selected;
        }
    }
}
