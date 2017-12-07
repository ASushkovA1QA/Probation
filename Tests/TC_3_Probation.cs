using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMARTframework.Framework.forms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using WebdriverFramework.forms;
using WebdriverFramework.Framework.util;
using WebdriverFramework.Framework.Util;
using WebdriverFramework.Framework.WebDriver;
using static WebdriverFramework.Framework.Util.CsvUtils;
using WebdriverFramework.Testrail;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace WebdriverFramework.Tests
{
    [TestClass]
    public class TC_3_Probation : BaseTest
    {
        [TestMethod]
        [DeploymentItem("chromedriver.exe")]
        public override void RunTest()
        {
            var step = 0;

            Log.LogStep(step++, "Request token");
            var values = new NameValueCollection();
            values[tokenValueName] = tokenValue;
            var token = HttpUtil.PostRequest(urlAPI + requestToken, values);
            KAssert.AssertFalsePosMsg("Token response OK", token == null);
            Log.Info("Token=" + token);
            
            Log.LogStep(step++, "Go to projectPage");
            Browser.NavigateTo(urlHomePageAuth);
            SetBrowserCookie(token);
            Browser.Refresh();
            var projectListForm = new ProjectListForm();
            KAssert.AssertTrue("Version Id match", projectListForm.GetVersionId() == tokenValue);

            Log.LogStep(step++, "Go to Nexage page");
            projectListForm = new ProjectListForm();
            projectListForm.ClickProj(projNameDefault);            
            string csvTestList = GetCsvTestList();
            KAssert.AssertFalsePosMsg("CsvTestList get OK", csvTestList == null);

            var nexageForm = new ProjectNexageForm();
            List<TestInfo> testList = TestInfo.GetObjFromCsv(csvTestList);
            KAssert.AssertTruePosMsg("Test not match API response", nexageForm.CheckTestList(testList) != null);
            KAssert.AssertFalsePosMsg("Test not sort correctly", nexageForm.CheckDateSort().Count != 0);

            Log.LogStep(step++, "Go to Back page");
            Browser.NavigateBack();           
            projectListForm = new ProjectListForm();
            projectListForm.ClickBtnAddProj();
            var addProjectForm = new AddForm();
            var randProjName = RegexUtil.GetRandomAlphaNumeric(projNameLength);
            addProjectForm.TypeText(randProjName);
            addProjectForm.ClickSubmit();
            addProjectForm.CheckSaveMessage();
            addProjectForm.ClosePopup();
            addProjectForm.AssertIsClosed();
            Browser.Refresh();

            projectListForm = new ProjectListForm();
            projectListForm.IsProjPresent(randProjName);

            Log.LogStep(step++, "Go to our project form");
            projectListForm.ClickProj(randProjName);

            var testId = SendTestIdRequest(randProjName);
            string logPath = SendLogRequestWithTestId(testId);
            string base64ImageFile = SendImgRequestWithTestId(testId);
            
            var allTestsForm = new AllTestsForm(randProjName);
            Assert.IsTrue( allTestsForm.GetTestList()[0].TestName == requestTestName, "Test name not match");

            Log.LogStep(step++, "Go to test form");
            allTestsForm.ClickItem(requestTestName);
            var testInfoForm = new TestInfoForm();
            testInfoForm.CheckProjName(randProjName);
            testInfoForm.CheckTestName(requestTestName);
            testInfoForm.CheckTestMethod(requestMethodName);
            testInfoForm.CheckStatus(newTestStatus);
            testInfoForm.CheckEnvironment(Dns.GetHostName());
            testInfoForm.CheckLog(logPath);
            testInfoForm.CheckImage(base64ImageFile, requestContentType);
            TestRail.Add_result();
        }
    }
}
