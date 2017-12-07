using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "ComboBox"
    /// </summary>
   public class ComboBox: BaseElement
    {
        private SelectElement _select;

        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the comboBox</param>
        /// <param name="name">name of the comboBox</param>
        public ComboBox(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the comboBox 
        /// </summary>
        /// <returns>type of the comboBox</returns>
        protected override string GetElementType()
        {
            return "Combobox";
        }

        /// <summary>
        /// gets selected label of the comboBox 
        /// </summary>
        /// <returns>selected label of the comboBox</returns>
        public string GetSelectedLabel()
        {
            WaitForIsPresent();
            return _select.SelectedOption.Text;
        }

        /// <summary>
        /// assertion of the presence of the comboBox on the page
        /// </summary>
        public new void AssertIsPresent()
        {
            base.AssertIsPresent();
            _select = new SelectElement(Element);
        }

        /// <summary>
        /// wait until the comboBox is present on the page
        /// </summary>
        public void WaitForIsPresent()
        {
            WaitForElementIsPresent();
            _select = new SelectElement(Element);
        }

        /// <summary>
        /// selects an item of the comboBox by text
        /// </summary>
        public void SelectByLabel(string label)
        {
            WaitForIsPresent();
            Info(string.Format("selecting option by text '{0}'",label));
            _select.SelectByText(label);
        }

        /// <summary>
        /// selects an item of the comboBox by value
        /// </summary>
        public void SelectByValue(string value)
        {
            WaitForIsPresent();
            Info(string.Format("selecting option by value '{0}'", value));
            _select.SelectByValue(value);
        }

        /// <summary>
        /// selects an item of the comboBox by index
        /// </summary>
        public void SelectByIndex(int index)
        {
            WaitForIsPresent();
            Info(string.Format("selecting option by index '{0}'", index));
            _select.SelectByIndex(index);
        }

        /// <summary>
        /// assertion of the selected item from the drop down list is equal to the expected
        /// </summary>
        /// <param name="expected">expected</param>
        public void AssertSelectedLabelPosMsg(string expected)
        {
            KAssert.AssertEqualsPosMsg(FormatLogMsg(string.Format("selected option equals '{0}'", expected)), expected, GetSelectedLabel());
        }

        /// <summary>
        /// assertion of the selected item from the drop down list is contains label
        /// </summary>
        /// <param name="label">label</param>
        public void AssertOptionsContains(string label)
        {
            var result = false;
            AssertIsPresent();
            IList<IWebElement> list = _select.Options;
            foreach (var webElement in list)
            {
                if (webElement.Text.Contains(label))
                {
                    result = true;
                }
            }
            //  var result = list.Any(webElement => webElement.Text.Contains(label));
            KAssert.AssertTrue("all options doesn't have : " + label, result);
        }

        /// <summary>
        /// get selected item
        /// </summary>
        /// <returns>selected item</returns>
        public SelectElement GetSelect()
        {
            WaitForIsPresent();
            return _select;
        }
    }
}
