 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
 using System.Text.RegularExpressions;
 using System.Xml;
using System.Diagnostics;

namespace MSTestResultViewer.Consol
{
    class Program
    {
        #region "Variables"
        const int SLEEP_EXECUTION_TIME = 200;
        const string TAG_UNITTEST = "UnitTest";
        const string TAG_ERRORINFO = "ErrorInfo";

        const string TABLE_TESTMETHOD = "TestMethod";
        const string TABLE_TESTRUN = "TestRun";
        const string TABLE_UNITTESTRESULT = "UnitTestResult";
        const string TABLE_TIMES = "Times";

        const string TABLE_TEST = "StdOut";

        const string COLUMN_CLASSNAME = "className";
        const string COLUMN_NAME = "name";
        const string COLUMN_RUNUSER = "runUser";
        const string COLUMN_MESSAGE = "Message";
        const string COLUMN_STACKTRACE = "StackTrace";
        const string COLUMN_COUNTERS = "Counters";
        const string COLUMN_TOTAL = "total";
        const string COLUMN_PASSED = "passed";
        const string COLUMN_FAILED = "failed";
        const string COLUMN_INCONCLUSIVE = "inconclusive";
        const string COLUMN_TESTNAME = "testName";
        const string COLUMN_OUTCOME = "outcome";
        const string COLUMN_DURATION = "duration";
        const string COLUMN_CREATION = "creation";
        const string COLUMN_CODEBASE = "codebase";

        const string ATTRIBUTE_TESTMETHODID = "TestMethodID";
        const string ATTRIBUTE_ID = "id";
        const string ATTRIBUTE_TESTID = "testId";
        const string FILENAME_TRX = "results.trx";

        private static TestEnvironmentInfo testEnvironmentInfo;
        private static TestResultSummary testResultSummary;
        private static List<TestProjects> testProjects;

        static string projectChartDataValue = "";
        static string classChartDataValue = "";
        static string classChartDataText = "";
        static string methoChartDataValue = "";
        static string methoChartDataText = "";
        static string methoChartDataColor = "";
        static string folderPath = "";
        static string trxFilePath = "";

        static string MSTestExePathParam = "";
        static string TestContainerFolderPathParam = "";
        static string DestinationFolderParam = "";
        static string LogFileParam = "";
        static string HelpFileParam = "";
        #endregion

        #region "Properties"
        public static string Title
        {
            get { return GetValue<AssemblyTitleAttribute>(a => a.Title); }
        }
        public static string Version
        {
            get { return "2.0"; } //GetValue<AssemblyVersionAttribute>(a => a.Version); }
        }
        static string GetValue<T>(Func<T, string> getValue) where T : Attribute
        {
            T a = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return a == null ? "" : getValue(a);
        }
        #endregion

        #region "Program Startup"
        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("{0} [Version {1}]\n", Title, Version));
            Console.WriteLine(string.Format("Copyright (c) 2012 Saffroze Technology Pvt. Ltd.\n"));
            Console.WriteLine(string.Format("Modified by D. Bogatko, S. Hamzatov, A1QA Software Testing Company, 2014\n"));
            if (RecognizeParameters(args))
            {
                try
                {
                    Directory.CreateDirectory(DestinationFolderParam);
                    Directory.CreateDirectory(string.Format("{0}\\Screen", DestinationFolderParam));
                    
                    string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string _path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
                    folderPath = Path.Combine(Path.Combine(appPath, "Data"));
                    Directory.CreateDirectory(folderPath);

                    
                    if (MSTestExePathParam != "")
                    {
                        GenerateTRXFile(MSTestExePathParam, TestContainerFolderPathParam);
                    }
                    Console.WriteLine(string.Format("Start TRX file transformation....\n"));

                    Transform(trxFilePath);

                    Console.WriteLine(string.Format("Transfering files at \"{0}\"\n", DestinationFolderParam));
                    
                    CopyFilesWithSubFolders(Path.Combine(_path, "Images"), Path.Combine(DestinationFolderParam, "Images"), true);
                    CopyFilesWithSubFolders(Path.Combine(_path, "Javascripts"), Path.Combine(DestinationFolderParam, "Javascripts"), true);
                    CopyFilesWithSubFolders(Path.Combine(_path, "Library"), Path.Combine(DestinationFolderParam, "Library"), true);
                    CopyFilesWithSubFolders(Path.Combine(_path, "RGraph"), Path.Combine(DestinationFolderParam, "RGraph"), true);
                    CopyFilesWithSubFolders(Path.Combine(_path, "Styles"), Path.Combine(DestinationFolderParam, "Styles"), true);
                    CopyFilesWithSubFolders(Path.Combine(appPath, "Data"), Path.Combine(DestinationFolderParam, "Data"), true, "*.js");

                    CopyFilesWithSubFolders(_path, DestinationFolderParam, true, "*.htm");
                    CopyFilesWithSubFolders(_path, DestinationFolderParam, true, "*.cur");
                    Console.WriteLine(string.Format("File Transfer completed\n"));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error: ", ex.Message));
                }
            }
            else
            {
                DisplayCommandLineHelp();
            }

            ParseHTML(DestinationFolderParam);
        }
        #endregion


        #region "Private Methods"
        private static void Transform(string fileName )
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(fileName))
            {
                xmlDoc.Load(fileName);
                //create copy of xml
                string justFileName = fileName.Split('\\').Last();
                string newTrxFileName = justFileName.Split('.')[0] + "Modified.trx";
                string newFileName = Regex.Replace(fileName, justFileName, newTrxFileName);
                if (File.Exists(newFileName)) File.Delete(newFileName);
                File.Copy(fileName, newFileName);
                
                // remove inner content of tag Results
                XmlDocument newXmlDoc = new XmlDocument();
                newXmlDoc.Load(newFileName);
                XmlNodeList results = newXmlDoc.GetElementsByTagName("Results");
                results[0].RemoveAll();
                newXmlDoc.Save(newFileName);
                
                // get all UnitTestResult and extract datadriven results
                XmlNodeList utResults = xmlDoc.GetElementsByTagName("UnitTestResult");
                var utnodes = new List<XmlNode>();
                bool hasInnerResults = false;
                foreach (XmlNode xmlNode in utResults)
                {
                    hasInnerResults = false;
                    foreach (XmlNode childs in xmlNode.ChildNodes)
                    {
                        if (childs.Name == "InnerResults")
                        {
                            hasInnerResults = true;
                            foreach (XmlNode ddutResult in childs.ChildNodes)
                            {
                                utnodes.Add(ddutResult);
                            }
                        }
                    }
                    if (!hasInnerResults && xmlNode.Attributes["parentExecutionId"] == null)
                    {
                        utnodes.Add(xmlNode);
                    }
                }
                // add result to new xml
                foreach (XmlNode node in utnodes) {

                    //necessary for crossing XmlDocument contexts
                    //XmlNode importNode = newXmlDoc.OwnerDocument.ImportNode(node, true);

                    results[0].InnerXml = results[0].InnerXml + node.OuterXml;
                }
                newXmlDoc.Save(newFileName);

                // reload xmlDoc
                xmlDoc.Load(newFileName);
                
                XmlNodeList list = xmlDoc.GetElementsByTagName(TAG_UNITTEST);
                foreach (XmlNode node in list)
                {
                    XmlAttribute newAttr = xmlDoc.CreateAttribute(ATTRIBUTE_TESTMETHODID);
                    newAttr.Value = node.Attributes[ATTRIBUTE_ID].Value;
                    node.ChildNodes[1].Attributes.Append(newAttr);
                }

                list = xmlDoc.GetElementsByTagName(TAG_ERRORINFO);
                foreach (XmlNode node in list)
                {
                    XmlAttribute newAttr = xmlDoc.CreateAttribute(ATTRIBUTE_TESTMETHODID);
                    newAttr.Value = (((node).ParentNode).ParentNode).Attributes[ATTRIBUTE_TESTID].Value;
                    node.Attributes.Append(newAttr);
                }

                //xmlDoc.Save(fileName);

                DataSet ds = new DataSet();
                ds.ReadXml(new XmlNodeReader(xmlDoc));
                
                if (ds != null && ds.Tables.Count >= 4)
                {
                    Console.WriteLine(string.Format("Start gathering test environment information...\n"));
                    System.Threading.Thread.Sleep(SLEEP_EXECUTION_TIME);

                    SetTestEnvironmentInfo(ds);

                    Console.WriteLine(string.Format("Start gathering test result summary...\n"));
                    System.Threading.Thread.Sleep(SLEEP_EXECUTION_TIME);
                    SetTestResultSummary(ds);

                    Console.WriteLine(string.Format("Start gathering test classes methods information...\n"));
                    System.Threading.Thread.Sleep(SLEEP_EXECUTION_TIME);
                    SetTestClassMethods(ds, xmlDoc);

                    if (testProjects.Count >= 1)
                    {
                        Console.WriteLine(string.Format("Start transforming test result into html...\n"));
                        System.Threading.Thread.Sleep(SLEEP_EXECUTION_TIME);

                        CreateTestHierarchy();

                        CreateTestResultTable();

                        CreateTestResultChart();

                        CreateTestLogs(xmlDoc);

                        SaveUnitTestResultsOutput(xmlDoc);

                        CreateSuites(xmlDoc);
                        
                        CreateOverview(xmlDoc);
                        
                        Console.WriteLine(string.Format("TRX file transformation completed successfully. \nFile generated at: \"{0}.htm\"\n", trxFilePath));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("No test cases are available for test\n"));
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("No test cases are available for test\n"));
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine(string.Format("Test Result File (.trx) not found at \"" + trxFilePath + "\"!\n"));
                Console.ReadLine();
            }
        }
        private static void SetTestEnvironmentInfo(DataSet ds)
        {
            DataRow dr = ds.Tables[TABLE_TESTRUN].Rows[0];
            string trxfile = dr[COLUMN_NAME].ToString();
            int idxattherate = trxfile.IndexOf("@") + 1;
            int idxspace = trxfile.IndexOf(" ");
            string machine = trxfile.Substring(idxattherate, idxspace - idxattherate);
            DateTime time = (ds.Tables[TABLE_TIMES] != null && ds.Tables[TABLE_TIMES].Rows.Count > 0) ? Convert.ToDateTime(ds.Tables[TABLE_TIMES].Rows[0][COLUMN_CREATION]) : DateTime.Now;
            string codebase = (ds.Tables[TABLE_TESTMETHOD] != null && ds.Tables[TABLE_TESTMETHOD].Rows.Count > 0) ? ds.Tables[TABLE_TESTMETHOD].Rows[0][COLUMN_CODEBASE].ToString() : "";
            testEnvironmentInfo = new TestEnvironmentInfo()
            {
                MachineName = machine,
                OriginalTRXFile = trxfile,
                TestCodebase = codebase,
                UserName = dr[COLUMN_RUNUSER].ToString(),
                Timestamp = time
            };
        }
        private static void SetTestResultSummary(DataSet ds)
        {
            DataRow dr = ds.Tables[COLUMN_COUNTERS].Rows[0];
            testResultSummary = new TestResultSummary()
            {
                Total = Convert.ToInt16(dr[COLUMN_TOTAL].ToString()),
                Passed = Convert.ToInt16(dr[COLUMN_PASSED].ToString()),
                Failed = Convert.ToInt16(dr[COLUMN_FAILED].ToString()),
                Ignored = Convert.ToInt16(dr[COLUMN_INCONCLUSIVE].ToString()),
                Duration = "--",
                TestEnvironment = testEnvironmentInfo
            };
        }
        private static void SetTestClassMethods(DataSet ds, XmlDocument xmlDoc)
        {
            var a = xmlDoc.GetElementsByTagName("UnitTestResult");


            DataView view = new DataView(ds.Tables[TABLE_TESTMETHOD]);
            DataTable distinctValues = view.ToTable(true, COLUMN_CLASSNAME);
            char[] delimiters = new char[] { ',' };

            //Getting Distinct Project
            testProjects = new List<TestProjects>();
            foreach (DataRow dr in distinctValues.Rows)
            {
                string _project = dr[COLUMN_CLASSNAME].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                int cnt = (from t in testProjects where t.Name == _project select t).Count();
                if (cnt == 0) testProjects.Add(new TestProjects() { Name = _project.Trim() });
            }

            //Iterate through all the projects for getting its classes
            foreach (TestProjects project in testProjects)
            {
                DataRow[] classes = distinctValues.Select(COLUMN_CLASSNAME + " like '% " + project.Name + ", %'");
                if (classes != null && classes.Count() > 0)
                {
                    project.Classes = new List<TestClasses>();
                    foreach (DataRow dr in classes)
                    {
                        string _class = dr[COLUMN_CLASSNAME].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                        project.Classes.Add(new TestClasses() { Name = _class });
                    }
                }
            }

            //Iterate through all the projects and then classes to get test methods details
            TimeSpan durationProject = TimeSpan.Parse("00:00:00.00");
            foreach (TestProjects _project in testProjects)
            {
                Int32 _totalPassed = 0;
                Int32 _totalFailed = 0;
                Int32 _totalIgnored = 0;
                foreach (TestClasses _class in _project.Classes)
                {
                    TimeSpan durationClass = TimeSpan.Parse("00:00:00.00");
                    DataRow[] methods = ds.Tables[TABLE_TESTMETHOD].Select(COLUMN_CLASSNAME + " like '" + _class.Name + ", " + _project.Name + ", %'");
                    if (methods != null && methods.Count() > 0)
                    {
                        _class.Methods = new List<TestClassMethods>();
                        Int32 _passed = 0;
                        Int32 _failed = 0;
                        Int32 _ignored = 0;
                        foreach (DataRow dr in methods)
                        {
                            var testId = a[_totalPassed+_totalFailed+_totalIgnored].Attributes.GetNamedItem("testId").Value;
                            TestClassMethods _method = GetTestMethodDetails(ds, testId);
                            //TestClassMethods _method = GetTestMethodDetails(ds, dr[ATTRIBUTE_TESTMETHODID].ToString());
                            switch (_method.Status.ToUpper())
                            {
                                case "PASSED":
                                    _passed++;
                                    break;
                                case "FAILED":
                                    _failed++;
                                    break;
                                default:
                                    _ignored++;
                                    break;
                            }
                            _class.Passed = _passed;
                            _class.Failed = _failed;
                            _class.Ignored = _ignored;
                            _class.Total = (_passed + _failed + _ignored);
                            _class.Methods.Add(_method);

                            durationClass += TimeSpan.Parse(_method.Duration);
                        }
                    }
                    _totalPassed += _class.Passed;
                    _totalFailed += _class.Failed;
                    _totalIgnored += _class.Ignored;

                    _class.Duration = durationClass.ToString();
                    durationProject += TimeSpan.Parse(_class.Duration);
                }
                _project.Passed = _totalPassed;
                _project.Failed = _totalFailed;
                _project.Ignored = _totalIgnored;
                _project.Total = (_totalPassed + _totalFailed + _totalIgnored);

                _project.Duration = durationProject.ToString();
                durationProject += TimeSpan.Parse(_project.Duration);
            }
        }
        private static TestClassMethods GetTestMethodDetails(DataSet ds, string testID)
        {
            TestClassMethods _method = null;
            DataRow[] methods = ds.Tables[TABLE_UNITTESTRESULT].Select(ATTRIBUTE_TESTID + "='" + testID + "'");
            if (methods != null && methods.Count() > 0)
            {
                _method = new TestClassMethods();
                foreach (DataRow dr in methods)
                {
                    _method.Name = dr[COLUMN_TESTNAME].ToString();
                    _method.Status = dr[COLUMN_OUTCOME].ToString();//(Enums.TestStatus)Enum.Parse(typeof(Enums.TestStatus), dr[COLUMN_OUTCOME].ToString());
                   //_method.Error = GetErrorInfo(ds, testID);
                    _method.Duration = dr[COLUMN_DURATION].ToString();
                }
            }
            return _method;
        }
        private static ErrorInfo GetErrorInfo(DataSet ds, string testID)
        {
            ErrorInfo _error = null;
            DataRow[] errorMethod = ds.Tables[TAG_ERRORINFO].Select(ATTRIBUTE_TESTMETHODID + "='" + testID + "'");
            if (errorMethod != null && errorMethod.Count() > 0)
            {
                _error = new ErrorInfo();
                string[] delimiters = new string[] { ":line " };
                foreach (DataRow dr in errorMethod)
                {
                    _error.Message = dr[COLUMN_MESSAGE].ToString();
                    _error.StackTrace = dr[COLUMN_STACKTRACE].ToString();

                    string strLineNo = _error.StackTrace.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)[1];
                    Int32 LineNo;
                    if (Int32.TryParse(strLineNo, out LineNo))
                    {
                        LineNo = Convert.ToInt32(strLineNo);
                    }
                    else
                    {
                        delimiters = strLineNo.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        if (delimiters.Length > 0)
                        {
                            if (Int32.TryParse(delimiters[0], out LineNo))
                            {
                                LineNo = Convert.ToInt32(delimiters[0]);
                            }
                        }
                    }
                    _error.LineNo = LineNo;
                    
                }
            }
            return _error;
        }
        private static void CreateTestHierarchy()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("var treeData = '");
            foreach (var _project in testProjects)
            {
                sb.Append("<li><span class=\"testProject\">" + _project.Name + "</span>");
                sb.Append("<ul>");
                foreach (var _class in _project.Classes)
                {
                    //Remove project name from name space if exists.
                    string classname = _class.Name;
                    string projectname = _project.Name + ".";
                    string[] tmp = _class.Name.Split(new string[] { projectname }, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length >= 2)
                        classname = (tmp[0] == _project.Name) ? ConvertStringArrayToString(tmp, 1) : tmp[0];
                    else if (tmp.Length == 1)
                        classname = tmp[0];

                    string[] arrClassName = classname.Split('.');
                    classname = arrClassName[arrClassName.Length - 1];

                    sb.Append("<li><span class=\"testClass\">" + classname + "</span>");
                    sb.Append("<ul>");
                    foreach (var _method in _class.Methods)
                    {
                        string imgStatus = "StatusFailed";
                        switch (_method.Status)
                        {
                            case "Passed":
                                imgStatus = "StatusPassed";
                                break;
                            case "Ignored":
                                imgStatus = "StatusIngnored";
                                break;
                            case "Failed":
                                imgStatus = "StatusFailed";
                                break;
                        }
                        sb.Append("<li><span class=\"testMethod\">" + _method.Name + "<img src=\"Images/" + imgStatus + ".png\" height=\"10\" width=\"10\" /></span></li>");
                    }
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
                sb.Append("</li>");
            }
            sb.Append("';");
            string htmlTestHierarchy = sb.ToString();
            WriteFile("TestHierarchy.js", htmlTestHierarchy);
        }
        private static string ConvertStringArrayToString(string[] array, int startIndex)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = startIndex; i < array.Length; i++)
            {
                builder.Append(array[i]);
            }
            return builder.ToString();
        }
        private static void CreateTestResultTable()
        {
            try
            {
                StringBuilder sbenv = new StringBuilder();
                sbenv.Append("var environment = {");
                sbenv.Append("'TestCodebase':'" + testEnvironmentInfo.TestCodebase + "',");
                sbenv.Append("'Timestamp':'" + testEnvironmentInfo.Timestamp + "',");
                sbenv.Append("'MachineName':'" + testEnvironmentInfo.MachineName + "',");
                sbenv.Append("'UserName':'" + testEnvironmentInfo.UserName + "',");
                sbenv.Append("'OriginalTRXFile':'" + testEnvironmentInfo.OriginalTRXFile + "'");
                sbenv.Append("};");
                WriteFile("Environment.js", sbenv.ToString());

                StringBuilder sb = new StringBuilder();
                sb.Append("$(function () {");
                sb.Append(" $('#dvTestCodebase').text(environment.TestCodebase);");
                sb.Append(" $('#dvGeneratedDate').text(environment.Timestamp);");
                sb.Append(" $('#dvMachineName').text(environment.MachineName);");
                sb.Append(" $('#dvUserName').text(environment.UserName);");
                sb.Append(" $('#dvTRXFileName').text(environment.OriginalTRXFile);");
                sb.Append("var mydata = [");
                int counter = 0;

                foreach (var _project in testProjects)
                {
                    counter++;
                    int total = _project.Passed + _project.Failed + _project.Ignored;
                    double percentPass = (_project.Passed * 100);
                    if (percentPass > 0) percentPass = percentPass / total;
                    double percentFail = (_project.Failed * 100);
                    if (percentFail > 0) percentFail = percentFail / total;
                    double percentIgnore = (_project.Ignored * 100);
                    if (percentIgnore > 0) percentIgnore = percentIgnore / total;
                    string strPercent = string.Format("{0},{1},{2}", percentPass, percentFail, percentIgnore);
                    string strProject = string.Format("{{id: \"{0}\", parent: \"{1}\", level: \"{2}\", Name:  \"{3}\", Passed: \"{4}\", Failed: \"{5}\", Ignored: \"{6}\", Percent: \"{7}\", Progress: \"{8}\", Time: \"{9}\", Message: \"{10}\", StackTrace: \"{11}\", LineNo: \"{12}\", isLeaf: {13}, expanded: {14}, loaded: {15}}},", counter, "", "0", _project.Name, _project.Passed, _project.Failed, _project.Ignored, string.Format("{0:00.00}", percentPass), strPercent, TimeSpan.Parse(_project.Duration).TotalMilliseconds, "", "", "", "false", "true", "true");
                    sb.Append(strProject);
                    int projParent = counter;

                    projectChartDataValue = "var projectData = [" + _project.Passed + ", " + _project.Failed + ", " + _project.Ignored + "];";

                    foreach (var _class in _project.Classes)
                    {
                        counter++;
                        total = _class.Passed + _class.Failed + _class.Ignored;
                        percentPass = (_class.Passed * 100);
                        if (percentPass > 0) percentPass = percentPass / total;
                        percentFail = (_class.Failed * 100);
                        if (percentFail > 0) percentFail = percentFail / total;
                        percentIgnore = (_class.Ignored * 100);
                        if (percentIgnore > 0) percentIgnore = percentIgnore / total;
                        strPercent = string.Format("{0},{1},{2}", percentPass, percentFail, percentIgnore);

                        //Remove project name from name space if exists.
                        string classname = _class.Name;
                        string projectname = _project.Name + ".";
                        string[] tmp = _class.Name.Split(new string[] { projectname }, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length >= 2)
                            classname = (tmp[0] == _project.Name) ? ConvertStringArrayToString(tmp, 1) : tmp[0];
                        else if (tmp.Length == 1)
                            classname = tmp[0];

                        string[] arrClassName = classname.Split('.');
                        classname = arrClassName[arrClassName.Length - 1];

                        string strClass = string.Format("{{id: \"{0}\", parent: \"{1}\", level: \"{2}\", Name:  \"{3}\", Passed: \"{4}\", Failed: \"{5}\", Ignored: \"{6}\", Percent: \"{7}\", Progress: \"{8}\", Time: \"{9}\", Message: \"{10}\", StackTrace: \"{11}\", LineNo: \"{12}\", isLeaf: {13}, expanded: {14}, loaded: {15}}},", counter, projParent, "1", classname, _class.Passed, _class.Failed, _class.Ignored, string.Format("{0:00.00}", percentPass), strPercent, TimeSpan.Parse(_class.Duration).TotalMilliseconds, "", "", "", "false", "true", "true");
                        sb.Append(strClass);
                        int classParent = counter;

                        classChartDataValue += "[" + _class.Passed + ", " + _class.Failed + ", " + _class.Ignored + "],";
                        classChartDataText += "'" + classname + "',";

                        foreach (var _method in _class.Methods)
                        {
                            counter++;
                            int _passed = 0;
                            int _failed = 0;
                            int _ignored = 0;
                            percentPass = 0.0;
                            strPercent = "";

                            methoChartDataValue += TimeSpan.Parse(_method.Duration).TotalMilliseconds + ",";
                            methoChartDataText += "'" + _method.Name + "',";

                            switch (_method.Status)
                            {
                                case "Passed":
                                    _passed = 1;
                                    percentPass = 100;
                                    strPercent = "100,0,0";
                                    methoChartDataColor += "testResultColor[0],";
                                    break;
                                case "Failed":
                                    _failed = 1;
                                    strPercent = "0,100,0";
                                    methoChartDataColor += "testResultColor[1],";
                                    break;
                                case "Ignored":
                                    _ignored = 1;
                                    strPercent = "0,0,100";
                                    methoChartDataColor += "testResultColor[2],";
                                    break;
                            }

                            string strError = "";
                            string strStack = "";
                            string strLine = "";
                            if (_method.Error != null)
                            {
                                strError = _method.Error.Message.Replace("\r\n", "").Replace("\"", "'");
                                strStack = _method.Error.StackTrace.Replace("\r\n", "").Replace("\"", "'");
                                strLine = _method.Error.LineNo.ToString();
                            }

                            string strMethod = string.Format("{{id: \"{0}\", parent: \"{1}\", level: \"{2}\", Name:  \"{3}\", Passed: \"{4}\", Failed: \"{5}\", Ignored: \"{6}\", Percent: \"{7}\", Progress: \"{8}\", Time: \"{9}\", Message: \"{10}\", StackTrace: \"{11}\", LineNo: \"{12}\", isLeaf: {13}, expanded: {14}, loaded: {15}}},", counter, classParent, "2", _method.Name, _passed, _failed, _ignored, string.Format("{0:00.00}", percentPass), strPercent, TimeSpan.Parse(_method.Duration).TotalMilliseconds, strError, strStack, strLine, "true", "false", "true");
                            sb.Append(strMethod);
                        }
                    }

                    classChartDataValue = "var classDataValue = [" + classChartDataValue + "];";
                    classChartDataText = "var classDataText = [" + classChartDataText + "];";

                    methoChartDataValue = "var methoDataValue = [" + methoChartDataValue + "];";
                    methoChartDataText = "var methoDataText = [" + methoChartDataText + "];";
                    methoChartDataColor = "var methoDataColor = [" + methoChartDataColor + "];";
                }
                sb.Append("],");
                sb.Append("getColumnIndexByName = function (grid, columnName) {");
                sb.Append("var cm = grid.jqGrid('getGridParam', 'colModel');");
                sb.Append("for (var i = 0; i < cm.length; i += 1) {");
                sb.Append("if (cm[i].name === columnName) {");
                sb.Append("return i;");
                sb.Append("}");
                sb.Append("}");
                sb.Append("return -1;");
                sb.Append("},");
                sb.Append("grid = $('#treegrid');");
                sb.Append("grid.jqGrid({");
                sb.Append("datatype: 'jsonstring',");
                sb.Append("datastr: mydata,");
                sb.Append("colNames: ['Id', 'Name', 'Passed', 'Failed', 'Ignored', '%', '', 'Time', 'Message','StackTrace','LineNo'],");
                sb.Append("colModel: [");
                sb.Append("{ name: 'id', index: 'id', width: 1, hidden: true, key: true },");
                sb.Append("{ name: 'Name', index: 'Name', width: 380 },");
                sb.Append("{ name: 'Passed', index: 'Passed', width: 70, align: 'right', formatter: testCounterFormat },");
                sb.Append("{ name: 'Failed', index: 'Failed', width: 70, align: 'right', formatter: testCounterFormat },");
                sb.Append("{ name: 'Ignored', index: 'Ignored', width: 70, align: 'right', formatter: testCounterFormat },");
                sb.Append("{ name: 'Percent', index: 'Percent', width: 50, align: 'right' },");
                sb.Append("{ name: 'Progress', index: 'Progress', width: 200, align: 'right', formatter: progressFormat },");
                sb.Append("{ name: 'Time', index: 'Time', width: 75, align: 'right'},");
                sb.Append("{ name: 'Message', index: 'Message', hidden: true, width: 100, align: 'right'},");
                sb.Append("{ name: 'StackTrace', index: 'StackTrace', hidden: true, width: 100, align: 'right'},");
                sb.Append("{ name: 'LineNo', index: 'LineNo', width: 100, hidden: true, align: 'right'}],");
                sb.Append("height: 'auto',");
                sb.Append("gridview: true,");
                sb.Append("rowNum: 10000,");
                sb.Append("sortname: 'id',");
                sb.Append("treeGrid: true,");
                sb.Append("treeGridModel: 'adjacency',");
                sb.Append("treedatatype: 'local',");
                sb.Append("ExpandColumn: 'Name',");

                sb.Append("ondblClickRow: function(id) {");
                sb.Append("parent.innerLayout.open('south');");
                sb.Append("setErrorInfo(id);");
                sb.Append("},");

                sb.Append("onSelectRow: function(id){");
                sb.Append("setErrorInfo(id);");
                sb.Append("},");

                sb.Append("jsonReader: {");
                sb.Append("repeatitems: false,");
                sb.Append("root: function (obj) { return obj; },");
                sb.Append("page: function (obj) { return 1; },");
                sb.Append("total: function (obj) { return 1; },");
                sb.Append("records: function (obj) { return obj.length; }");
                sb.Append("}");
                sb.Append("});");

                sb.Append("function setErrorInfo(id) {");
                sb.Append("var doc = $('#tblError', top.document);");
                sb.Append("doc.find('#dvErrorMessage').text($('#treegrid').getRowData(id)['Message']);");
                sb.Append("doc.find('#dvLineNumber').text($('#treegrid').getRowData(id)['LineNo']);");
                sb.Append("doc.find('#dvStackTrace').text($('#treegrid').getRowData(id)['StackTrace']);");
                sb.Append("}");

                sb.Append("function progressFormat(cellvalue, options, rowObject) {");
                sb.Append("var progress = cellvalue.split(',');");
                sb.Append("var pass = Math.round(progress[0]) * 2;");
                sb.Append("var fail = Math.round(progress[1]) * 2;");
                sb.Append("var ignore = Math.round(progress[2]) * 2;");
                sb.Append("var strProgress = \"<div class='ProgressWrapper'>");
                sb.Append("<div class='ProgressPass' title='\"+ Number(progress[0]).toFixed(2) +\"% Passed' style='width: \" + pass + \"px'></div>");
                sb.Append("<div class='ProgressFail' title='\"+ Number(progress[1]).toFixed(2) +\"% Failed' style='width: \" + fail + \"px'></div>");
                sb.Append("<div class='ProgressIgnore' title='\"+ Number(progress[2]).toFixed(2) +\"% Ignored' style='width: \" + ignore + \"px'></div>");
                sb.Append("</div>\";");
                sb.Append("return strProgress;");
                sb.Append("}");

                sb.Append("function testCounterFormat(cellvalue, options, rowObject) {");
                sb.Append("return cellvalue;");
                sb.Append("}");
                sb.Append("grid.jqGrid('setLabel', 'Passed', '', { 'text-align': 'right' });");
                sb.Append("grid.jqGrid('setLabel', 'Failed', '', { 'text-align': 'right' });");
                sb.Append("grid.jqGrid('setLabel', 'Ignored', '', { 'text-align': 'right' });");
                sb.Append("grid.jqGrid('setLabel', 'Percent', '', { 'text-align': 'right' });");
                sb.Append("});");
                string xmlTestResultTable = sb.ToString().Replace("},],", "}],");
                WriteFile("Table.js", xmlTestResultTable);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private static void CreateTestResultChart()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("var testResultStatus = ['Passed', 'Failed', 'Ignored'];");
            sb.Append("var testResultColor = ['#ABD874', '#E18D87', '#F4AD7C'];");
            sb.Append(projectChartDataValue);
            sb.Append(classChartDataValue);
            sb.Append(classChartDataText);
            sb.Append(methoChartDataValue);
            sb.Append(methoChartDataText);
            sb.Append(methoChartDataColor);

            string xmlTestResultTable = sb.ToString().Replace(",]", "]");
            WriteFile("Chart.js", xmlTestResultTable);
        }
        private static void WriteFile(string FileName, string FileContent)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(folderPath, FileName)))
            {
                file.WriteLine(FileContent);
            }
        }
        private static void CreateTestLogs(XmlDocument xmlDoc)
        {
            var a = xmlDoc.GetElementsByTagName("UnitTestResult");
            var i = 0;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbMessage = new StringBuilder();

                sb.Append("var treeData = '");
                sbMessage.Append("var treeDataMessage = '");
                foreach (var _project in testProjects)
                {
                    sb.Append("<li><span class=\"testProject\">" + _project.Name + "</span>");
                    sb.Append("<ul>");

                    sbMessage.Append("<li><span class=\"testProject\">" + _project.Name + "</span>");
                    sbMessage.Append("<ul>");

                    foreach (var _class in _project.Classes)
                    {
                        //Remove project name from name space if exists.
                        var text = string.Empty;
                        string classname = _class.Name;
                        string projectname = _project.Name + ".";
                        string[] tmp = _class.Name.Split(new string[] { projectname }, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length >= 2)
                            classname = (tmp[0] == _project.Name) ? ConvertStringArrayToString(tmp, 1) : tmp[0];
                        else if (tmp.Length == 1)
                            classname = tmp[0];

                        string[] arrClassName = classname.Split('.');
                        classname = arrClassName[arrClassName.Length - 1];

                        
                        foreach (var _method in _class.Methods)
                        {
                            //Параметр сolor определяет цвет тестсьюта в зависимости от результата
                            //Параметр backcolor определяет цвет фона лога в зависимости от результата
                            string color, backcolor, imgStatus = string.Empty;
                            //Итоговые цвета результата тест-сьюта на вкладке Results
                            switch (_method.Status)
                            {
                                case "Passed":
                                    imgStatus = "StatusPassed";
                                    color = "green";
                                    backcolor = "GreenYellow";
                                    break;
                               case "Failed":
                                    imgStatus = "StatusFailed";
                                    color = "red";
                                    backcolor = "Tomato";
                                    break;
                               default:
                                    imgStatus = "StatusIngnored";
                                    color = "#EF9100";
                                    backcolor = "#FAFAFA";
                                    break;
                            }

                            sb.Append("<li style=\"border-bottom: 1px solid #D7EBF9;\"class=\"expandable\"><span class=\"testClass\" style=\"color:"+color+";\">" + classname + "<img src=\"Images/" + imgStatus + ".png\" height=\"10\" width=\"10\" /></span>");
                            sb.Append("<ul style=\"display: none;\">");
                            sb.Append("<li><span class=\"testMethod\">" + _method.Name + "<img src=\"Images/" + imgStatus + ".png\" height=\"10\" width=\"10\" /></span>");
   
                            var z = a[i].ChildNodes[0].ChildNodes[0].InnerText.Replace("\"", "").Replace("\'", "").Replace("\r","");

                            //стиль блока под RunTest в TreeView
                            sb.Append("<div style=\"padding: 5px 10px; margin: 5px 10px;  border: 2px outset #D7EBF9; background: " + backcolor + ";\">");
                            sb.Append("<p style=\"word-wrap: break-word;\">");

                            string[] arr = z.Split('\n');

                            foreach (var w in arr)
                            {
                                if (w.Contains(@"\"))
                                {
                                    // здесь регэкспом выдираем место где хранится фоточка
                                    var sourcePath = Regex.Match(w,  @"(\s\S:[\\].*)|([\\].*)").Value;
                                   
                                    if (sourcePath.Contains(".jpg") || sourcePath.Contains(".png") ||
                                        sourcePath.Contains(".jpeg") || sourcePath.Contains(".gif") || sourcePath.Contains(".avi") || sourcePath.Contains(".mp4"))
                                    {
                                        string destPath = string.Format("{0}\\{1}",
                                                                        Path.Combine(DestinationFolderParam, "Screen"),
                                                                        /*GetDeleteDigit(*/sourcePath.Split('\\').Last()/*)*/);
                                        try
                                        {
                                            if (!File.Exists(sourcePath))
                                            {
                                                sourcePath = sourcePath.Replace(sourcePath.Split('\\').First(),
                                                                                testEnvironmentInfo.MachineName);
                                                sourcePath = string.Format(@"\\{0}", sourcePath);
                                            }
                                            
                                            // txt file
                                            var txtPath = ReplaceLastPath(sourcePath);
                                            txtPath = ReplaceLastPath(txtPath);
                                      
                                            var txtPathWithTxt = string.Format("{0}\\{1}", txtPath, "TestData.txt");

                                            if (File.Exists(txtPathWithTxt))
                                            {
                                                var txtDestPath = string.Format("{0}\\{1}\\{2}",
                                                                        Path.Combine(DestinationFolderParam, "Screen"), GetDeleteDigit(txtPath.Split('\\').Last()),
                                                                        txtPathWithTxt.Split('\\').Last());
                                                Directory.CreateDirectory(string.Format("{0}\\Screen\\{1}",
                                                                                        DestinationFolderParam,
                                                                                        GetDeleteDigit(txtPath.Split('\\').Last())));
                                                File.Copy(txtPathWithTxt, txtDestPath, true);
                                                text = File.ReadAllText(txtDestPath);
                                            }
                                            
                                            File.Copy(sourcePath, destPath, true);
                                        }
                                        catch (Exception)
                                        {
                                            sourcePath = @"\Не найден сетевой путь";
                                        }
                                        if (w.Contains(".avi") || w.Contains(".mp4"))
                                        {
                                            // здесь покоится отображение проигрывателя на странице
                                            //sb.Append(w);
                                            //sb.Append("</p><p style=\"word-wrap: break-word;\">");
                                            //sb.Append("</p><div><object width=\"470\" height=\"353\"><param name=\"movie\" value=\"");
                                            //sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                                            //sb.Append("\"><param name=\"wmode\" value=\"window\"><param name=\"allowFullScreen\" value=\"true\"><param name=\"autostart\" value=\"0\"><embed src=\"");
                                            //sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                                            //sb.Append("\" type=\"application/x-shockwave-flash\" wmode=\"window\" allowfullscreen=\"true\" width=\"470\" height=\"353\"/></object></div><p>");

                                            sb.Append("</p><div><a href=\"");
                                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                                            sb.Append("\">");
                                            sb.Append(w.Replace('\\','/'));
                                            sb.Append("</a></div><p>");

                                        }
                                        else
                                        {
                                            sb.Append("</p><div><img src=\"");
                                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                                            sb.Append("\" alt=\"");
                                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                                            sb.Append("\" class=\"magnify\" style=\"width:180px; height:140px;\"/></div><p>");
                                        }
                                    }
                                    else
                                    {
                                        var describeLink = w.Replace(sourcePath, string.Empty);
                                        sb.Append("</p>");
                                        sb.Append(describeLink);
                                        sb.Append("<a href=\"file:////");
                                        sb.Append(sourcePath.Replace('\\', '/'));
                                        sb.Append("\">");
                                        sb.Append(sourcePath.Replace('\\', '/'));
                                        sb.Append("</a><p style=\"word-wrap: break-word;\">");
                                    }
                                }
                                else
                                {
                                    sb.Append(w);
                                    sb.Append("</p><p style=\"word-wrap: break-word;\">");
                                }
                              
                            }

                            sb.Append("</p></div>");

                            // Textdata
                            if (text!=string.Empty)
                            {
                                sb.Append("<div style=\"padding: 5px 10px; margin: 5px 10px;  border: 2px outset #D7EBF9; background: #FAFAFA; color:red;\"> TEXTDATA :");
                                sb.Append("<p style=\"color:black; word-wrap: break-word;\">");
                                sb.Append(text.Replace("\r", "").Replace("\n", ""));
                                sb.Append("</p></div>");
                            }

                            #region ERROR MESSAGE
                            ///////////////////////////////////////////////////ERROROR MESSAGE
                            if (a[i].ChildNodes[0].ChildNodes.Count > 2)
                            {
                                var erormess =
                                    a[i].ChildNodes[0].ChildNodes[2].ChildNodes[0].InnerText.Replace("\"", "")
                                                                                  .Replace("\'", "")
                                                                                  .Replace("\r", "");

                                    sbMessage.Append("<li style=\"border-bottom: 1px solid #D7EBF9;\"class=\"expandable\"><span class=\"testClass\" style=\"color:" + color + ";\">" + classname + "<img src=\"Images/" + imgStatus + ".png\" height=\"10\" width=\"10\" /></span>");
                                    sbMessage.Append("<ul style=\"display: none;\">");
                                    sbMessage.Append("<li><span class=\"testMethod\">" + _method.Name + "<img src=\"Images/" + imgStatus + ".png\" height=\"10\" width=\"10\" /></span>");
                                    sbMessage.Append("<div style=\"padding: 5px 10px; margin: 5px 10px;  border: 2px outset #D7EBF9; background: #FAFAFA; color:red;\"> MESSAGE :"); 
                                    sbMessage.Append("<p style=\"color:black; word-wrap: break-word;\">");

                                    sb.Append("<div style=\"padding: 5px 10px; margin: 5px 10px;  border: 2px outset #D7EBF9; background: #FAFAFA; color:red;\"> MESSAGE :");
                                sb.Append("<p style=\"color:black; word-wrap: break-word;\">");

                                arr = erormess.Split('\n');
                                foreach (var w in arr)
                                {
                                    sb.Append(w);
                                    sb.Append("</p><p style=\"color:black; word-wrap: break-word;\">");
                                    sbMessage.Append(w);
                                    sbMessage.Append("</p><p style=\"color:black;word-wrap: break-word;\">"); 
                                }
                                sb.Append("</p></div>");
                                sbMessage.Append("</p></div>");

                                sbMessage.Append("</li>");
                                sbMessage.Append("</ul>");
                                sbMessage.Append("</li>");


                                //STACKTRACE
                                if (a[i].ChildNodes[0].ChildNodes[2].ChildNodes.Count > 1)
                                {
                                    erormess =
                                    a[i].ChildNodes[0].ChildNodes[2].ChildNodes[1].InnerText.Replace("\"", "")
                                                                                  .Replace("\'", "")
                                                                                  .Replace("\r", "");

                                    sb.Append("<div style=\"padding: 5px 10px; margin: 5px 10px;  border: 2px outset #D7EBF9; background: #FAFAFA; color:red;\"> STACKTRACE: ");
                                    sb.Append("<p style=\"color:black; word-wrap: break-word;\">");

                                    arr = erormess.Split('\n');
                                    foreach (var w in arr)
                                    {
                                        if (w.Contains(@"\"))
                                        {
                                            string str = w.Replace('\\','/');
                                            sb.Append(str);
                                            sb.Append("</p><p style=\"color:black; word-wrap: break-word;\">");

                                        }
                                        else
                                        {
                                            sb.Append(w);
                                            sb.Append("</p><p style=\"color:black; word-wrap: break-word;\">");
                                        }
                                    }
                                    sb.Append("</p></div>");
                                }
                                
                            }

                            /////////////////////////////////////////
                            #endregion
                            sb.Append("</li>");

                            i++;
                                }
                        sb.Append("</ul>");
                        sb.Append("</li>");
                      
                    }
                    sb.Append("</ul>");
                    sb.Append("</li>");
                    sbMessage.Append("</ul>");
                    sbMessage.Append("</li>");
                }
                sb.Append("';");
                sbMessage.Append("';");
                string htmlTestHierarchy = string.Format("{0}{1}", sb, sbMessage);
                WriteFile("Logs.js", htmlTestHierarchy);
        }

        private static string GetDeleteDigit(string str)
        {
         // var del = Regex.Match(str, @"_*\d{1,4}(\.||-)\d{1,2}(\.||-)\d{2,6}_*\d*-*\d*-*\d*").Value;
            var del = Regex.Match(str, @"_*\d*\.\d*\.\d*_\d*").Value;
            if (del == string.Empty) return str;
            return str.Replace(del, string.Empty);
        }

        private static string ReplaceLastPath(string sourcePathReplase)
        {
            sourcePathReplase = sourcePathReplase.Replace(string.Format(@"\{0}", sourcePathReplase.Split('\\').Last()), string.Empty);
            return sourcePathReplase;
        }

        private static void GenerateTRXFile(string MsTestExePath, string TestContainerFilePath)
        {
            trxFilePath = Path.Combine(folderPath, FILENAME_TRX);
            string commandText = "\"" + MsTestExePath + "\" /testcontainer:\"" + TestContainerFilePath + "\" /resultsfile:\"" + trxFilePath + "\"";
            WriteFile("mstestrunner.bat", commandText);
            ExecuteBatchFile();
        }
        private static void ExecuteBatchFile()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = Path.Combine(folderPath, "mstestrunner.bat");
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        private static bool RecognizeParameters(string[] args)
        {
            if (args.Length >= 4 && args.Length <= 10)
            {
                int i = 0;
                while (i < args.Length)
                {
                    switch (args[i].ToLower())
                    {
                        case "/m":
                        case "/mstestexepath":
                        case "-m":
                        case "-mstestexepath":
                            if (args.Length > i)
                            {
                                MSTestExePathParam = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;
                        case "/tc":
                        case "/testcontainerfolderpath":
                        case "-tc":
                        case "-testcontainerfolderpath":
                            if (args.Length > i)
                            {
                                TestContainerFolderPathParam = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;
                        case "/t":
                        case "/trxfilepath":
                        case "-t":
                        case "-trxfilepath":
                            if (args.Length > i)
                            {
                                trxFilePath = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;
                        case "/d":
                        case "/destinationfolderpath":
                        case "-d":
                        case "-destinationfolderpath":
                            if (args.Length > i)
                            {
                                DestinationFolderParam = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;
                        /*case "/l":
                        case "/logfile":
                        case "-l":
                        case "-logfile":
                            if (args.Length > i)
                            {
                                LogFileParam = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;
                        case "/h":
                        case "/helpfile":
                        case "-h":
                        case "-helpfile":
                            if (args.Length > i)
                            {
                                HelpFileParam = args[i + 1];
                                i += 2;
                            }
                            else
                                return false;
                            break;*/
                        case "/?":
                        case "/help":
                        case "-?":
                        case "-help":
                            return false;
                        default:
                            Console.WriteLine("Error: Unrecognized parameter\n");
                            return false;
                    }
                }
                return true;
            }
            return false;
        }
        private static void DisplayCommandLineHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}.exe\n", Title));
            sb.Append(string.Format("[</M MSTestExePath>\n"));
            sb.Append(string.Format(" </TC TestContainerFolderPath>]    :optional if you set /T TRXFilePath\n"));
            sb.Append(string.Format("[</T TRXFilePath>]\n"));
            sb.Append(string.Format("</D DestinationFolder>             \n"));
            sb.Append(string.Format("</? Help>\n"));
            Console.Write(sb.ToString());
            Console.ReadKey();
        }
        private static bool CopyFilesWithSubFolders(string SourcePath, string DestinationPath, bool overwriteexisting, string Pattern = "*.*")
        {
            bool ret = true;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false) Directory.CreateDirectory(DestinationPath);
                    foreach (string fls in Directory.GetFiles(SourcePath, Pattern))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyFilesWithSubFolders(drs, DestinationPath + drinfo.Name, overwriteexisting, Pattern) == false) ret = false;
                    }
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }
        #endregion


        //a1qa report
        public static void CreateSuites(XmlDocument doc)
        {
            // header
            var header = new StringBuilder();
            header.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            header.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            header.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"\" lang=\"\">");
            header.AppendLine("<head>");

            //добавлено для выделения ссылки после нажатия. Описывает стиль ссылок менюшек.
            header.AppendLine("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.5.2/jquery.min.js\"></script>");
            header.AppendLine("<style>");
            header.AppendLine(".menu{width: 300px; height: 25; font-size: 16px;}");
            header.AppendLine(".menu li{list-style: none; float: left; margin-right: 1px; padding: 3px;}");
            header.AppendLine(".menu li:hover, .menu li.active {background-color: GreenYellow;}"); //Цвет обводки линков после нажатия
            header.AppendLine("</style>");

            header.AppendLine("<title>Automated Tests Results</title>");
            header.AppendLine("<meta http-equiv='Content-Type' content='text/html;charset=utf-8' />");
            header.AppendLine("</head>");
            header.AppendLine("<body style='margin-top: 0;'>");
            header.AppendLine("<div>");
            header.AppendLine("<p> <img src=\"https://dev.admin.wsdinsider.com/ADM/Content/images/logo_iris.png\"></img></p>");
            header.AppendLine("</div>");
            header.AppendLine("<table id=\"suites\">");
            header.AppendLine("<tr>");
            header.AppendLine("<td>");

            // footer
            var footer = new StringBuilder();
            footer.AppendLine("</td>");
            footer.AppendLine("</tr>");
            footer.AppendLine("</table>");
            footer.AppendLine("</body>");
            footer.AppendLine("</html>");

            // create log files
            var content = new StringBuilder();

            content.AppendLine("<ul class = menu>"); //added
            //const string testOutputTmp = "<a href=\"{0}.html\" {1} target=\"main\">{0}</a><br/>"; //было
            const string testOutputTmp = "<li><a href=\"{0}.html\" {1} target=\"main\">{0}</a></li>"; //added

            XmlNodeList list = doc.GetElementsByTagName("UnitTestResult");
            int testNumber = 0;
            foreach (XmlNode node in list)
            {
                testNumber++;
                //цвета тестов на вкладке Automated Tests
                string outcome = node.Attributes != null && node.Attributes["outcome"].Value == "Passed" ? "style=\"color:green\"" : "style=\"color:red\""; //оригинал

                // get error info
                var errorInfo = new StringBuilder();
                if (node.Attributes["outcome"].Value != "Passed")
                {
                    foreach (XmlNode outNodes in node.FirstChild.ChildNodes)
                    {
                        if (outNodes.Name == "ErrorInfo")
                        {
                            errorInfo.AppendLine("<table>");
                            //добавляя тег <p style=\"\"> мы меняем цвет сообщений об ошибках
                            errorInfo.AppendLine("<tr><td><p style=\"color:red\">" + outNodes.InnerText + "</p></td></tr>");
                            errorInfo.AppendLine("</table><br/>");
                        }
                    }
                }

                XmlNode nodeStdout = node.FirstChild.FirstChild;
                string output = GetLogsWithScreens(nodeStdout.InnerText);
                string testName = testNumber.ToString("D4") + "_" + Regex.Match(output, "TC_[0-9]+_[\\w]+").Groups[0];
                try
                {
                    File.WriteAllText("..\\..\\" + testName + ".html", Regex.Replace(errorInfo + output, "\\r\\n", "<br/>"), Encoding.UTF8);
                    content.AppendLine(string.Format(testOutputTmp, testName, outcome));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot save testdata: " + "..\\" + testName + ".html\r\n" + e.Message);
                }
            }

            content.AppendLine("</ul>"); //added
            
            //Это скрипт для добавления и снятия стилей с ссылок по нажатию        
            content.AppendLine("<script type=\"text/javascript\">");
            content.AppendLine("var make_button_active = function()");
            content.AppendLine("{");
            content.AppendLine("  var siblings =($(this).siblings());");
            content.AppendLine("  siblings.each(function (index)");
            content.AppendLine("    {");
            content.AppendLine("      $(this).removeClass('active');");
            content.AppendLine("    }");
            content.AppendLine("  )");
            content.AppendLine("  $(this).addClass('active');");
            content.AppendLine("}");
            content.AppendLine("$(document).ready(");
            content.AppendLine("  function()");
            content.AppendLine("  {");
            content.AppendLine("    $(\".menu li\").click(make_button_active);");
            content.AppendLine("  }");
            content.AppendLine(")");
            content.AppendLine("</script>");

            var suites = new StringBuilder();
            suites.Append(header);
            suites.Append(content);
            suites.Append(footer);

            File.WriteAllText("..\\..\\suites.html", suites.ToString(), Encoding.UTF8);
        }

        public static void CreateOverview(XmlDocument doc)
        {
            var header = new StringBuilder();
            header.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            header.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"");
            header.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            header.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"\" lang=\"\">");
            header.AppendLine("<head><title>Automated Test Results</title><body>");
            header.AppendLine("<head><title>To view test logs please click on the test name on the left navigation bar</title><body>");

            var footer = new StringBuilder();
            footer.AppendLine("</body>");
            footer.AppendLine("</html>");

            var content = new StringBuilder();
            content.AppendLine("<p><b>Enviroment:</b></p>");
            content.AppendLine("<table border= '1' cellpadding='5' style='background:#F5D0A9; color:black; border-collapse: collapse'>");
            content.AppendLine(string.Format("<tr><td>TestCodebase</td><td>{0}</td></tr>", testEnvironmentInfo.TestCodebase));
            content.AppendLine(string.Format("<tr><td>Timestamp</td><td>{0}</td></tr>", testEnvironmentInfo.Timestamp));
            content.AppendLine(string.Format("<tr><td>MachineName</td><td>{0}</td></tr>", testEnvironmentInfo.MachineName));
            content.AppendLine(string.Format("<tr><td>UserName</td><td>{0}</td></tr>", testEnvironmentInfo.UserName));
            content.AppendLine(string.Format("<tr><td>OriginalTRXFile</td><td>{0}</td></tr>", testEnvironmentInfo.OriginalTRXFile));
            content.AppendLine("</table>");

            content.AppendLine("<p><b>Test statistic table:</b></p>");

            content.AppendLine("<table border='1' cellpadding='2' style='background:#F5D0A9; color:black; border-collapse: collapse'>");
            content.AppendLine("<tr align='center'><td>Failed</td><td>Passed</td><td>Total</td></tr>");
            XmlNodeList list = doc.GetElementsByTagName("UnitTestResult");
            int passed = 0;
            foreach (XmlNode node in list)
            {
                if (node.Attributes["outcome"].Value == "Passed")
                {
                    passed++;
                }
            }

            content.AppendLine(string.Format("<tr align='center'><td>{0}</td><td>{1}</td><td>{2}</td></tr>", list.Count - passed, passed, list.Count));
            content.AppendLine("</table>");


            var overview = new StringBuilder();
            overview.Append(header);
            overview.Append(content);
            overview.Append(footer);

            File.WriteAllText("..\\..\\overview.html", overview.ToString(), Encoding.UTF8);

        }

        public static string GetLogsWithScreens(string output)
        {
            var sb = new StringBuilder();
            string[] arr = output.Split('\n');
            var text = string.Empty;
            foreach (var w in arr)
            {
                if (w.Contains(@"\"))
                {
                    // здесь регэкспом выдираем место где хранится фоточка
                    var sourcePath = Regex.Match(w, @"(\s\S:[\\].*)|([\\].*)").Value;

                    if (sourcePath.Contains(".jpg") || sourcePath.Contains(".png") ||
                        sourcePath.Contains(".jpeg") || sourcePath.Contains(".gif") || sourcePath.Contains(".avi") || sourcePath.Contains(".mp4"))
                    {
                        sourcePath = Regex.Replace(sourcePath, "'\\r", "");
                        string destPath = string.Format("{0}\\{1}", Path.Combine(DestinationFolderParam, "Screen"),
                                                        GetDeleteDigit(Regex.Replace(sourcePath.Split('\\').Last(), "\\r", "")));
                        try
                        {
                            if (File.Exists(sourcePath))
                            {
                                File.Copy(sourcePath, destPath, true);
                            }
                        }
                        catch (Exception e)
                        {
                            sourcePath = @"\Не найден сетевой путь";
                            Console.WriteLine(e.Message);
                        }
                        if (w.Contains(".avi") || w.Contains(".mp4"))
                        {
                            // здесь покоится отображение проигрывателя на странице
                            //sb.Append(w);
                            //sb.Append("</p><p style=\"word-wrap: break-word;\">");
                            //sb.Append("</p><div><object width=\"470\" height=\"353\"><param name=\"movie\" value=\"");
                            //sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                            //sb.Append("\"><param name=\"wmode\" value=\"window\"><param name=\"allowFullScreen\" value=\"true\"><param name=\"autostart\" value=\"0\"><embed src=\"");
                            //sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                            //sb.Append("\" type=\"application/x-shockwave-flash\" wmode=\"window\" allowfullscreen=\"true\" width=\"470\" height=\"353\"/></object></div><p>");

                            sb.Append("</p><div><a href=\"");
                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                            sb.Append("\">");
                            sb.Append(w.Replace('\\', '/'));
                            sb.Append("</a></div><p>");

                        }
                        else
                        {
                            sb.Append("</p><div><img  style=\"width:300;height:200\" src=\"");
                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                            sb.Append("\" alt=\"");
                            sb.Append(string.Format("Screen//{0}", GetDeleteDigit(sourcePath.Split('\\').Last())));
                            sb.Append("\" class=\"magnify\"/></div><p>");
                        }
                    }
                    else
                    {
                        var describeLink = w.Replace(sourcePath, string.Empty);
                        sb.Append("</p>");
                        sb.Append(describeLink);
                        sb.Append("<a href=\"file:////");
                        sb.Append(sourcePath.Replace('\\', '/'));
                        sb.Append("\">");
                        sb.Append(sourcePath.Replace('\\', '/'));
                        sb.Append("</a><p style=\"word-wrap: break-word;\">");
                    }
                }
                else
                {
                    sb.Append(w);
                    sb.Append("</p><p style=\"word-wrap: break-word;\">");
                }
            }
            return sb.ToString();
        }

        public static void SaveUnitTestResultsOutput(XmlDocument doc)
        {
            XmlNodeList list = doc.GetElementsByTagName("StdOut");
            var output = new StringBuilder();
            foreach (XmlNode node in list)
            {
                output.Append(Regex.Replace(node.InnerXml, "\\r\\n", "<br/>"));
            }
            File.WriteAllText("..\\..\\Output.htm", output.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Parsing HTML files for styles and images
        /// </summary>
        /// <param name="path">Path to folder</param>
        public static void ParseHTML(string path)
        {
            Console.WriteLine("Parsing HTML Files");

            string[] filePaths = Directory.GetFiles(@path, "*.html");
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (filePaths[i].Contains("_TC_"))
                {
                    AutoParser.ParseOneFile(filePaths[i], filePaths[i]);
                }
            }
        }
    }
}
