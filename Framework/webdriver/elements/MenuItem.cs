using System.Text;
using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "menu item"
    /// </summary>
    public class MenuItem : BaseElement
    {
        private const string Delimiter = " -> "; // separator menu items (used for logging)

        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of menu item</param>
        /// <param name="name">name of menu item</param>
        public MenuItem(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of menu item 
        /// </summary>
        /// <returns>type of menu item</returns>
        protected override string GetElementType()
        {
            return "MenuItem";
        }

        /// <summary>
        /// The creation of the full name of the menu item (used for logging)
        /// </summary>
        /// <param name="names">An array of names of individual items of a complex menu</param>
        /// <returns>name</returns>
        protected static string GetName(string[] names)
        {
            var result = new StringBuilder(names[0]);
            for (int i = 1; i < names.Length; i++)
            {
                result.Append(Delimiter);
                result.Append(names[i]);
            }
            return result.ToString();
        }
    }
}
