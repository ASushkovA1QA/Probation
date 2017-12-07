using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver.Elements
{
    /// <summary>
    /// class describes the interface element "textbox"
    /// </summary>
    public class TextBox:BaseElement
    {
        /// <summary>
        /// constructor with two parameters
        /// </summary>
        /// <param name="locator">locator By of the textbox</param>
        /// <param name="name">name of the textbox</param>
        public TextBox(By locator, string name) : base(locator, name)
        { }

        /// <summary>
        /// gets the type of the textbox 
        /// </summary>
        /// <returns>type of the textbox</returns>
        protected override string GetElementType()
        {
            return "TextBox";
        }

        /// <summary>
        /// set text without clear
        /// </summary>
        /// <param name="text">text for set</param>
        public void Type(string text)
        {
            WaitForElementIsPresent();
            Info(string.Format("Typing '{0}'",text));
            Element.SendKeys(text);
        }

        /// <summary>
        /// clear field and set text
        /// </summary>
        /// <param name="text">text for set</param>
        public void SetText(string text)
        {
            WaitForElementIsPresent();
            Element.Clear();
            Info(string.Format("Setting '{0}'", text));
            Element.SendKeys(text);
        }

        /// <summary>
        /// clear field and set text using js by elemnt's id.
        /// </summary>
        /// <param name="text">text for set</param>
        public void SendKeysViaJSById(string text)
        {
            WaitForElementIsPresent();
            Element.Clear();
            Info(string.Format("Setting '{0}'", text));
            IJavaScriptExecutor js = (IJavaScriptExecutor)Browser.GetDriver();
            string id = Element.GetAttribute("id");
            js.ExecuteScript(string.Format("document.getElementById('{0}').value = '{1}';", id, text));
        }

        /// <summary>
        /// type text into textbox without wait before element is dispayed
        /// </summary>
        /// <param name="text">typed text</param>
        public void TypeInvisible(string text)
        {
            WaitForElementExists();
            Info(string.Format("Type '{0}'", text));
            Browser.GetDriver().ExecuteScript("arguments[0].value=" + text, Element);
        }

        /// <summary>
        /// submit form
        /// </summary>
        public void Submit()
        {
            WaitForElementIsPresent();
            Info("submitting");
            Element.Submit();
        }

        /// <summary>
        /// assert if text is present
        /// </summary>
        /// <param name="text">text</param>
        public void AssertIsTextPresentPosMsg(string text)
        {
            var value = GetValue();
            KAssert.AssertTruePosMsg(FormatLogMsg("contains " + text), value.Contains(text));
        }
    }
}
