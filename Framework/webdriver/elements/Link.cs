using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "link"
    /// </summary>
    public class Link: BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the link</param>
        /// <param name="name">name of the link</param>
        public Link(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the link 
        /// </summary>
        /// <returns>type of the link</returns>
        protected override string GetElementType()
        {
            return "Link";
        }

        /// <summary>
        /// gets attribute href
        /// </summary>
        /// <returns>attribute href of the link</returns>
        public string GetHref()
        {
            return GetElement().GetAttribute("href");
        }
    }
}
