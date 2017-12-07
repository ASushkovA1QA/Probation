using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    public class List : BaseElement
    {
        By locator;

        public List(By locator, string name) : base(locator, name)
        {
            this.locator = locator;
        }

        /// <summary>
        /// gets the type of the link 
        /// </summary>
        /// <returns>type of the link</returns>
        protected override string GetElementType()
        {
            return "Link";
        }

        public IList<IWebElement> ToList()
        {
            WaitForElementIsPresent();
            return Browser.GetDriver().FindElements(locator).ToList();
        }

        public Link FindElementWithTxt(string elementText)
        {
            foreach (var element in this.ToList())
            {
                if (element.Text == elementText)
                {
                    return new Link(By.LinkText(element.Text), "Element from list");
                }
            }
            return null;
        }
    }
}

