using System;
using System.Collections.Generic;
using System.Linq;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.Util
{
    public class CsvUtils : BaseEntity
    {
        protected override string FormatLogMsg(string message)
        {
            throw new NotImplementedException();
        }

        public class TestInfo
        {
            public string TestName;
            public string TestMethod;
            public string TestResult;
            public string TestStartTime;
            public string TestEndTime;
            public string TestDuration;

            public static TestInfo FromCsv(string csvLine)
            {
                string[] values = csvLine.Split(',');
                TestInfo filesList = new TestInfo();
                try
                {
                    filesList.TestName = Convert.ToString(values[0]).ToUpper();
                    filesList.TestMethod = Convert.ToString(values[1]).ToUpper();
                    filesList.TestResult = Convert.ToString(values[2]).ToUpper();
                    filesList.TestStartTime = Convert.ToString(values[3]);
                    filesList.TestEndTime = Convert.ToString(values[4]);
                    filesList.TestDuration = Convert.ToString(values[5]);
                } catch (Exception e){ Log.Error("CSV file read error in string "+ filesList.TestName+ ", error:" + e.StackTrace); }
                return filesList;
            }

            public static List<TestInfo> GetObjFromCsv(string csvTestList)
            {
                var csvStrings = csvTestList.Split('\n').Skip(1);
                List<TestInfo> testList = csvStrings.Select(v => TestInfo.FromCsv(v)).ToList();
                return testList;
            }
        }
    }
}
