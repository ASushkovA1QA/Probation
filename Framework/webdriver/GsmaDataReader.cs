using System;
using WebdriverFramework.Framework.Util;
using WebdriverFramework.resources;

namespace WebdriverFramework.Framework.WebDriver
{
    public class ConfigReader : BaseEntity
    {
        private readonly string _envPath;
       
        protected override string FormatLogMsg(string message)
        {
            throw new System.NotImplementedException();
        }

        public static string GetUrlAPI()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//urlAPI");
        }

        public static string GetUrlHomePage()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//urlHomePage");
        }

        internal static string GetAddResultAPI()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//addResultAPI");
        }

        internal static string GetUserId()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//userId");
        }

        internal static string GetAttrIncludeAll()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrIncludeAll");
        }

        internal static string GetAttrId()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrId");
        }

        internal static string GetTestRunName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//testRunName");
        }

        internal static string GetAttrStatusId()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrStatusId");
        }

        internal static string GetAttrActual()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrActual");
        }

        internal static string GetAttrName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrName");
        }

        internal static string GetAddRunAPI()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//addRunAPI");
        }

        public static string GetPathToken()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//requestToken");
        }

        public static string GetPathTest()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//requestTest");
        }

        public static string GetPathTestLog()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//requestTestLog");
        }

        public static string GetPathTestAttach()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//requestTestAttach");
        }

        public static string GetPathCsvTestList()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//requestCsvTestList");
        }

        internal static string GetAttrTokenName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//attrTokenName");
        }

        internal static string GetProjNameDefault()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//projNameDefault");
        }
        
        public static string GetLoginHomePage()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//loginHomePage");
        }

        public static string GetPasswordHomePage()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//passwordHomePage");
        }

        public static string GetTokenValueName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//tokenValueName");
        }

        public static string GetTokenValue()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//tokenValue");
        }

        public static string GetProjectValueName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//projectValueName");
        }

        public static string GetProjectValue()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//projectValue");
        }

        internal static string GetNewTestStatus()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//newTestStatus");
        }

        internal static string GetTestRailAPI()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//testRailAPI");
        }

        internal static string GetPasswordTestRail()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//passwordTestRail");
        }

        internal static string GetLoginTestRail()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//loginTestRail");
        }

        public static string GetCookiePath()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//cookiePath");
        }

        public static string GetProjNameLength()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//projNameLength");
        }

        public static string GetCloudName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//cloudName");
        }

        public static string GetCloudApiKey()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//cloudApiKey");
        }

        public static string GetCloudApiSecret()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//cloudApiSecret");
        }

        public static string GetScreenshotName()
        {
            return "\\" + XmlUtils.GetNodeText(Constants.TestConfiguration, "//screenshotName");
        }

        public static string GetLogName()
        {
            return "\\" + XmlUtils.GetNodeText(Constants.TestConfiguration, "//logName");
        }

        public static string GetSID()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//SID");
        }

        public static string GetProjectName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//projectName");
        }

        public static string GetTestName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//testName");
        }

        public static string GetMethodName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//methodName");
        }

        public static string GetHostname()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//hostname");
        }
        
        public static string GetTestIdName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//testId");
        }

        public static string GetContentName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//content");
        }
        
        public static string GetContentTypeName()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//contentType");
        }

        public static string GetContentType()
        {
            return XmlUtils.GetNodeText(Constants.TestConfiguration, "//contentTypeImage");
        }
    }
}
