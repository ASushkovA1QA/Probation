using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using WebdriverFramework.Framework.util;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Framework.WebDriver.Elements;

namespace SMARTframework.Framework.forms
{
    class TestInfoForm : BaseForm
    {
        private static readonly By TitleLocator = By.XPath("//ol[contains(@class, 'breadcrumb')]//li[contains(text(),'')]");

        private List lstTestInfo = new List(By.XPath("//div[@class='panel panel-default']//div[@class='list-group']//p"), "List testInfo");
        private List lstAttachInfo = new List(By.XPath("//div[@class='panel panel-default']//table[@class='table']//td"), "List logInfo + imageInfo");
        private string lnkImage = ".//a";
        private string regexItem = "base64,([^&]*)";
        private string stringToRemove = "base64,";
        private string lnkStatus = ".//span";

        public TestInfoForm() : base(TitleLocator, "Test info") { }

        public void CheckProjName (string value)
        {
            Assert.IsTrue(value == lstTestInfo.ToList()[0].Text, "Proj name not match");
        }

        public void CheckTestName(string value)
        {
            Assert.IsTrue(value == lstTestInfo.ToList()[1].Text, "Test name not match");
        }

        public void CheckTestMethod(string value)
        {
            Assert.IsTrue(value == lstTestInfo.ToList()[2].Text, "TestMethod name not match");
        }

        public void CheckStatus(string value)
        {
            Assert.IsTrue(value == lstTestInfo.ToList()[3].FindElement(By.XPath(lnkStatus)).Text, "Status not match");
        }

        public void CheckEnvironment(string value)
        {
            Assert.IsTrue(value == lstTestInfo.ToList()[7].Text, "Environment name not match");
        }

        public void CheckLog(string value)
        {
            Assert.IsTrue(value == lstAttachInfo.ToList()[0].Text, "Log name not match");
        }

        public void CheckImage(string image, string imageType)
        {
            string imageAttribute = lstAttachInfo.ToList()[1].FindElement(By.XPath(lnkImage)).GetAttribute("href").ToString();
            string imageFromPage = Regex.Match(imageAttribute, regexItem).ToString();
            Assert.IsTrue(image == new Regex(stringToRemove).Replace(imageFromPage,""), "Image name not match");
            Assert.IsTrue(imageType == lstAttachInfo.ToList()[2].Text, "ImageType not match");
        }
    }
}
