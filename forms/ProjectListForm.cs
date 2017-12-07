using OpenQA.Selenium;
using System;
using WebdriverFramework.Framework.util;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Framework.WebDriver.Elements;

namespace WebdriverFramework.forms
{
    public class ProjectListForm : BaseForm
    {
        private static readonly By TitleLocator = By.XPath("//ol[contains(@class, 'breadcrumb')]//a[contains(@href, 'projects')]");
        private readonly List ListNexageProj = new List(By.XPath("//div[contains(@class, 'list-group')]//a[contains(@class, 'list-group-item')]"), "Project list");
        private readonly Button BtnAddProj = new Button(By.XPath("//button[contains(@class, 'btn-xs')]"), "Btn addProject");
        private readonly Link LnkFooter = new Link(By.XPath("//footer[contains(@class, 'footer')]"), "Footer text");

        private String RegexSpecialsText = @"\d";

        public ProjectListForm() : base(TitleLocator, "ProjectList Form") { }

        public void ClickProj(string projName)
        {
            ListNexageProj.FindElementWithTxt(projName).Click();
        }

        public void ClickBtnAddProj() => BtnAddProj.Click();

        public String GetVersionId()
        {
            return RegexUtil.GetRegexItem(LnkFooter.GetText(), RegexSpecialsText).ToString();
        }

        public bool IsProjPresent(string projName)
        {
            return ListNexageProj.FindElementWithTxt(projName).IsPresent();
        }
    }

}
