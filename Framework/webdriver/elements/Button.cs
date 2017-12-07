using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "button"
    /// </summary>
    public class Button: BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the button</param>
        /// <param name="name">name of the button</param>
        public Button(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the button 
        /// </summary>
        /// <returns>type of the button</returns>
        protected override string GetElementType()
        {
            return "Button";
        }
    }
}
