using OpenQA.Selenium;
using SMARTframework.Framework.forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebdriverFramework.Framework.util;
using WebdriverFramework.Framework.Util;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Framework.WebDriver.Elements;
using static WebdriverFramework.Framework.Util.CsvUtils;

namespace WebdriverFramework.forms
{
    public class AllTestsForm : BaseForm
    {
        private static  String TitleLocator = "//ol[contains(@class, 'breadcrumb')]//li[contains(text(),'%s')]";
        private readonly Table TbTestList = new Table(By.XPath("//table[contains(@class, 'table')]"), "Test list table");
        private static string cellText = ".//a";

        public AllTestsForm(String testName) : base(By.XPath(new Regex("%s").Replace(TitleLocator, testName)), "All tests Form") { }

        public List<TestInfo> GetTestList()
        {
            List<TestInfo> testList = new List<TestInfo>();
            for (int row = 1; row < TbTestList.GetRowsCount(); row++)
            {
                TestInfo testInfo = new TestInfo();
                try { testInfo.TestName = Convert.ToString(TbTestList.GetCell(row, 0)); } catch (Exception e) { testInfo.TestName = null; };
                try { testInfo.TestMethod = Convert.ToString(TbTestList.GetCell(row, 1)); } catch (Exception e) { testInfo.TestMethod = null; };
                try { testInfo.TestResult = Convert.ToString(TbTestList.GetCell(row, 2)); } catch (Exception e) { testInfo.TestResult = null; };
                try { testInfo.TestStartTime = Convert.ToString(TbTestList.GetCell(row, 3)); } catch (Exception e) { testInfo.TestStartTime = null; };
                try { testInfo.TestEndTime = Convert.ToString(TbTestList.GetCell(row, 4)); } catch (Exception e) { testInfo.TestEndTime = null; };
                try { testInfo.TestDuration = Convert.ToString(TbTestList.GetCell(row, 5)); } catch (Exception e) { testInfo.TestDuration = null; };
                testList.Add(testInfo);
            }
            return testList;
        }

        public void ClickItem(string itemName)
        {
            for (int row = 1; row < TbTestList.GetRowsCount(); row++)
                if (TbTestList.GetCell(row, 0).ToString() == itemName)
                    TbTestList.GetCellElement(row, 0).FindElement(By.XPath(cellText)).Click();
        }

        public void CheckItem(string itemName)
        {
            KAssert.AssertFalsePosMsg("Test", TbTestList.GetCell(1, 0) != null);
        }

    }
}


