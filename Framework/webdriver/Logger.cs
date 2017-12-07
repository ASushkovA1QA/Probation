using System;
using System.Collections;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// class for log
    /// </summary>
    public class Logger
    {
        private readonly ILog _log;
        private static Logger _instance;
        /// <summary>
        /// delimeter betweeen inline log messages
        /// </summary>
        public static readonly string LogDelimiter = "::";
        private int _lastExecutedStep;
        public static bool IsPrecondition { get; } = false;
        public string PreconditionMessage { get; private set; } = "";
        private const string TemplatePreconditionMessage = " in precondition id {0}";

        /// <summary>
        /// constructor
        /// </summary>
        private Logger()
        {
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger("Console"); }

        /// <summary>
        /// instance log
        /// </summary>
        public static Logger Instance => _instance ?? (_instance = new Logger());

        /// <summary>
        /// Store last executed step description
        /// </summary>
        public string LastExecutedStepMessage { get; set; } = "";

        /// <summary>
        /// fail in log
        /// </summary>
        /// <param name="message">message for log</param>
        public void Fail(string message)
        {
            _log.Fatal(message);
            KAssert.Fail(message);
        }

        /// <summary>
        /// info in log
        /// </summary>
        /// <param name="message">message for log</param>
        public void Info(string message)
        {
            _log.Info(message);
        }

        /// <summary>
        /// api version in log
        /// </summary>
        /// <param name="versionApi"></param>
        public void VersionApi(string versionApi)
        {
            _log.Info($"<span style =\"background-color: deepskyblue; font-weight: bold\">VERSION API: {versionApi}");
        }

        /// <summary>
        /// precondition for start
        /// </summary>
        /// <param name="title">name of the step</param>
        public void PreconditionStart(string title)
        {
            SetPreconditionMessage();
            var formattedName = $"===== CommandLineExecutor: '{title}' =====";
            var delims = "";
            var nChars = formattedName.Length;
            for (var i = 0; i < nChars; i++)
            {
                delims += "+";
            }
            Info(delims);
            Info(formattedName);
            Info(delims);
        }

        /// <summary>
        /// precondition for failed
        /// </summary>
        /// <param name="title">name of the step</param>
        public void PreconditionFailed(string title)
        {
            
            var formattedName = $"===== CommandLineExecutor Failed: '{title}' =====";
            var delims = "";
            var nChars = formattedName.Length;
            for (var i = 0; i < nChars; i++)
            {
                delims += "#";
            }
            Info(delims);
            Info(formattedName);
            Info(delims);
        }

        /// <summary>
        /// precondition for passed
        /// </summary>
        /// <param name="title">name of the step</param>
        public void PreconditionPassed(string title)
        {
            var formattedName = $"===== CommandLineExecutor Passed: '{title}' =====";
            var delims = "";
            var nChars = formattedName.Length;
            for (var i = 0; i < nChars; i++)
            {
                delims += "+";
            }
            Info(delims);
            Info(formattedName);
            Info(delims);
        }

        /// <summary>
        /// name of the test
        /// </summary>
        /// <param name="title">name of the test</param>
        public void TestName(string title)
        {
            var formattedName = $"========== Test Case: '{title}' ==========";
            var delims = "";
            var nChars = formattedName.Length;
            for (var i = 0; i < nChars; i++)
            {
                delims += "-";
            }
            Info(delims);
            Info(formattedName);
            Info(delims);
        }

        /// <summary>
        /// simple log range of steps
        /// </summary>
        /// <param name="step">step</param>
        /// <param name="toStep">toStep</param>
        public void LogStep(int step, int toStep)
        {
            StoreLastStepInfo(step);
            Info($"<font style='color: #ffffff; background-color: grey'>------------------------------------------[ Steps {step} - {toStep}]----------------------------------------</font>");
            //Info($"<span style=\"background-color: deepskyblue; font-weight: bold\"> ------------------------------------------[ Steps {step} - {toStep}]----------------------------------------");
        }


        /// <summary>
        /// simple log step
        /// </summary>
        /// <param name="step"></param>
        public void LogStep(int step)
        {
            StoreLastStepInfo(step);
            Info($"<font style='color: #ffffff; background-color: grey'>------------------------------------------[ Step {step} ]----------------------------------------</font>");
            //Info($"<span style=\"background-color: deepskyblue; font-weight: bold\"> ----------------------------------------[ Step {step} ]<span>");
        }

        /// <summary>
        /// log step with action message from MTM
        /// </summary>
        /// <param name="step">step</param>
        /// <param name="message">message</param>
        public void LogStep(int step, string message)
        {
            StoreLastStepInfo(step);
            Info($"<font style='color: #ffffff; background-color: grey'>------------------------------------------[ Step {step} ]: {message}</font>");
            //Info($"<span style=\"background-color: deepskyblue; font-weight: bold\">----------------------------------------[ Step {step} ]: [{message}]</span>");
        }

        private void StoreLastStepInfo(int step)
        {
            _lastExecutedStep = step;
            LastExecutedStepMessage = $" [step #'{_lastExecutedStep}']";
        }

        /// <summary> log step with action message from MTM
        /// </summary>
        /// <param name="step">step</param>
        /// <param name="toStep">toStep</param>
        /// <param name="message">message</param>
        public void LogStep(int step, int toStep, string message)
        {
            LogStep(step, toStep);
            Info(message);
            Info("----------------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// log if test is passed
        /// </summary>
        /// <param name="title">name test</param>
        public void Passed(string title)
        {
            var formattedName = $"******** Test Case: '{title}' Passed! ********";
            var delims = "";
            var nChars = formattedName.Length;
            for (var i = 0; i < nChars; i++)
            {
                delims += "*";
            }
            Info(delims);
            Info(formattedName);
            Info(delims);
        }

        /// <summary>
        /// debug
        /// </summary>
        /// <param name="format">format</param>
        public void Debug(string format)
        {
           _log.Debug(format);
        }

        /// <summary>
        /// warn
        /// </summary>
        /// <param name="formatLogMsg">format for message</param>
        public void Warn(string formatLogMsg)
        {
            _log.Warn(formatLogMsg);
        }
        
        /// <summary>
        /// error
        /// </summary>
        /// <param name="formatLogMsg">format for message</param>
        public void Error(string formatLogMsg)
        {
            _log.Error(formatLogMsg);
        }

        /// <summary>
        /// get TC id
        /// </summary>
        /// <param name="intput">intput</param>
        /// <returns>intput</returns>
        private static string GetTcId(string intput)
        {
            string step = null;
            try
            {
                var match = Regex.Match(intput, @"TC_(\d+)", RegexOptions.IgnoreCase); //
                if (match.Success)
                {
                    step = match.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                intput += ex;
            }
            return step ?? intput;
        }

        /// <summary>
        /// Set precondition message
        /// </summary>
        public void SetPreconditionMessage()
        {
            PreconditionMessage = string.Format(TemplatePreconditionMessage, GetTcId((String)_deepCounter.Peek()));
        }

        private readonly Stack _deepCounter = new Stack();
    }
}
