using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Framework.WebDriver.Elements;
using static WebdriverFramework.Framework.Util.CsvUtils;

namespace WebdriverFramework.forms
{
    public class ProjectNexageForm : BaseForm
    {
        private static readonly By TitleLocator = By.XPath("//ol[contains(@class, 'breadcrumb')]//li[contains(text(),'Nexage')]");
        private readonly Table TbTestList = new Table(By.XPath("//table[contains(@class, 'table')]"), "Test list table");

        public ProjectNexageForm() : base(TitleLocator, "Project Nexage Form") { }

        public List<TestInfo> GetTestList()
        {
            List<TestInfo> testList = new List<TestInfo>();
            for (int row = 1; row < TbTestList.GetRowsCount(); row++)
            {
                TestInfo testInfo = new TestInfo();
                try { testInfo.TestName = Convert.ToString(TbTestList.GetCell(row, 0)).ToUpper(); } catch (Exception e) { testInfo.TestName = null; };
                try { testInfo.TestMethod = Convert.ToString(TbTestList.GetCell(row, 1)).ToUpper(); } catch (Exception e) { testInfo.TestMethod = null; };
                try { testInfo.TestResult = Convert.ToString(TbTestList.GetCell(row, 2)).ToUpper(); } catch (Exception e) { testInfo.TestResult = null; };
                try { testInfo.TestStartTime = Convert.ToString(TbTestList.GetCell(row, 3)); } catch (Exception e) { testInfo.TestStartTime = null; };
                try { testInfo.TestEndTime = Convert.ToString(TbTestList.GetCell(row, 4)); } catch (Exception e) { testInfo.TestEndTime = null; };
                try { testInfo.TestDuration = Convert.ToString(TbTestList.GetCell(row, 5)); } catch (Exception e) { testInfo.TestDuration = null; };
                testList.Add(testInfo);
            }
            return testList;
        }

        public List<TestInfo> CheckTestList(List<TestInfo> testList)
        {
            List<TestInfo> formTestList = this.GetTestList();
            List<TestInfo> wrongTestList = new List<TestInfo>(formTestList);

            foreach (var fileInfo in formTestList)
            {
                foreach (var testinfo in testList)
                {
                    if (testinfo.TestName == fileInfo.TestName &&
                        testinfo.TestDuration == fileInfo.TestDuration &&
                        testinfo.TestEndTime == fileInfo.TestEndTime &&
                        testinfo.TestMethod == fileInfo.TestMethod &&
                        testinfo.TestResult == fileInfo.TestResult &&
                        testinfo.TestStartTime == fileInfo.TestStartTime )
                        wrongTestList.Remove(fileInfo);
                }
            }
            if (wrongTestList != null)
               wrongTestList.ForEach(v => Log.Error("Test not match API response:" + v.TestName.ToString()));
            return wrongTestList;
        }


        public List<TestInfo> CheckDateSort()
        {
            List<TestInfo> formTestList = this.GetTestList();
            List<TestInfo> wrongTestList = new List<TestInfo>();
            DateTime date = DateTime.Now;
            foreach (var fileInfo in formTestList)
            {
                if (Convert.ToDateTime(fileInfo.TestStartTime) < date)
                    date = Convert.ToDateTime(fileInfo.TestStartTime);
                else wrongTestList.Add(fileInfo);
            }
            if (wrongTestList != null)
                wrongTestList.ForEach(v => Log.Error("Test not sort correctly: " + v.TestName.ToString()));
            return wrongTestList;
        }
    }
}

