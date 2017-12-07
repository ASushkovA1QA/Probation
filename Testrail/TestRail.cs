using Gurock.TestRail;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WebdriverFramework.Framework.WebDriver;
using WebdriverFramework.Tests.Utils;

namespace WebdriverFramework.Testrail
{
    public class TestRail
    {
        protected static String testRailAPI = ConfigReader.GetTestRailAPI();
        protected static String loginTestRail = ConfigReader.GetLoginTestRail();
        protected static String passwordTestRail = ConfigReader.GetPasswordTestRail();
        protected static String addRunAPI = ConfigReader.GetAddRunAPI();
        protected static String addResultAPI = ConfigReader.GetAddResultAPI();
        protected static String userId = ConfigReader.GetUserId();
        protected static String attrName = ConfigReader.GetAttrName();
        protected static String attrIncludeAll = ConfigReader.GetAttrIncludeAll();
        protected static String attrStatusId = ConfigReader.GetAttrStatusId();
        protected static String attrActual = ConfigReader.GetAttrActual();
        protected static String attrId = ConfigReader.GetAttrId();
        protected static String attrTestRunName = ConfigReader.GetTestRunName();

        private static APIClient GetClient()
        {
            APIClient client = new APIClient(testRailAPI);
            client.User = loginTestRail;
            client.Password = passwordTestRail;
            return client;
        }
        public static string Add_run()
        {
            var data = new Dictionary<string, object>
            {
               { attrName, attrTestRunName + DateTime.Now.ToString()},
               { attrIncludeAll, true},
            };
            JObject r = (JObject)GetClient().SendPost(addRunAPI + userId, data);
            return r.GetValue(attrId).ToString();
        }

        public static void Add_result()
        {
            JObject obj = new JObject();
            obj.Add(attrStatusId, Enums.GetTestStatus(Enums.status.PASSED));
            obj.Add(attrActual, ImageProducer.SetScreenshot());
            JObject r = (JObject)GetClient().SendPost(addResultAPI + Add_run(), obj);
        }
    }
}
