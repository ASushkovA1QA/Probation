using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.PageObjects;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Framework.WebDriver.Elements;

namespace SMARTframework.Framework.forms
{
    public class AddForm : BaseForm
    {
        private static readonly By TitleLocator = By.XPath("//div[contains(@class, 'modal-dialog')]");
        private readonly TextBox TxtField = new TextBox(By.XPath("//input[contains(@type, 'text') and contains(@name, '')]"), "TextField");
        private readonly Button BtnSumbit = new Button(By.XPath("//button[contains(@type, 'submit') or (contains(@id, 'submit'))]"), "Btn Submit");
        public readonly string lnkSumbit = "//button[contains(@type, 'submit') or (contains(@id, 'submit'))]";

        [FindsBy(How = How.XPath, Using = "//div[contains(@class, 'success')]")]
        public readonly IWebElement lnkSaveOk;
       
        public AddForm() : base(TitleLocator, "Add project form") { }

        public void TypeText(string text)
        {
            TxtField.Type(text);
        }

        public string GetText()
        {
            return TxtField.GetText();
        }

        public void ClickSubmit()
        {
            BtnSumbit.Click();
        }

        public void CheckSaveMessage()
        {
            PageFactory.InitElements(Browser.GetDriver(), this);
            KAssert.AssertTruePosMsg("Project saved", lnkSaveOk.Text != null);
        }

        public void ClosePopup()
        {
            Actions action = new Actions(Browser.GetDriver());
            action.MoveToElement(Browser.GetDriver().FindElement(By.XPath(lnkSumbit))).MoveByOffset(-200, -200).Click().Build().Perform();
            System.Threading.Thread.Sleep(500);
        }
    }
}
