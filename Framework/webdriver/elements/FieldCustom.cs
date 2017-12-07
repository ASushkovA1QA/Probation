using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface custom field
    /// </summary>
    class FieldCustom : BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of custom field</param>
        /// <param name="name">name of custom field</param>
        public FieldCustom(By locator, string name)
            : base(locator, name)
        { }

        /// <summary>
        /// gets the type of custom field 
        /// </summary>
        /// <returns>type of custom field</returns>
        protected override string GetElementType()
        {
            return "Field";
        }
    }
}
