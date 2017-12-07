using System;
using System.IO;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// class provides methods for soft assertions
    /// sometimes we need verify some condition and proceed test in case if verification was failed
    /// this method allow make this
    /// after as the test was ended and has failed soft assertions test still be marked as failed with messages of the reasons
    /// class doesn't support running in several threads
    /// </summary>
    public class Checker
    {
        private readonly static Random Random = new Random(int.MaxValue);

        /// <summary>
        /// store for the reasons of failed soft assertions
        /// </summary>
        public static CheckMessList Messages = new CheckMessList();

        /// <summary>
        /// verify that two object are equal
        /// </summary>
        /// <param name="message">message if objects don't equals</param>
        /// <param name="actual">actual object</param>
        /// <param name="expected">expected object</param>
        public static bool CheckEquals(string message, Object actual, Object expected)
        {
            if (!actual.Equals(expected))
            {
                //SetErrorInfo($"<span style=\"background-color: grey; font-weight: bold\">" + message + "." + "\r\nObject Actual not equals object Expected", actual, expected);
                Logger.Instance.Info("<span style=\"background-color: red; font-weight: bold\">" + message +
                    "\r\n<span style=\"background-color: red;\">Expeceted: </span>" + expected + 
                    "\r\n" + "<span style=\"background-color: red; \">Actual: </span>" + actual);
                return false;
            }
            Logger.Instance.Info(message + "\r\nExpeceted: " + expected + "\r\n" + "Actual: " + actual);
            return true;
        }

        /// <summary>
        /// verify that actual string contains another(expected)
        /// </summary>
        /// <param name="message">message if verification was failed</param>
        /// <param name="actual">actual string</param>
        /// <param name="expected">expected string</param>
        public static bool CheckContains(string message, string actual, string expected)
        {
            if (BaseForm.TitleForm != null) message = string.Format("{0} :: {1}", BaseForm.TitleForm, message);
            if (!actual.Contains(expected))
            {
                //SetErrorInfo($"<span style=\"background-color: grey; font-weight: bold\">{message}.", actual, expected);
                Logger.Instance.Info("<span style=\"background-color: red; font-weight: bold\">" + message +
                    "\r\n<span style=\"background-color: red;\">Expeceted: </span>" + expected + 
                    "\r\n" + "<span style=\"background-color: red;\">Actual: </span>" + actual);
                return false;
            }
            Logger.Instance.Info(message);
            return true;
        }

        /// <summary>
        /// Marks test as failed with changing static HasWarn variable of the BaseTest to true
        /// also make screenshot and post log message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="actual">actual object</param>
        /// <param name="expected">expected object</param>
        private static void SetErrorInfo(string message, Object actual, Object expected)
        {
            Messages.Add($"< span style =\"background-color: grey; font-weight: bold\">Assertion failed:</span>\r\n"+
                "< span style =\"background-color: red; font-weight: bold\">Condition:</span> {message} \r\n " +
                "< span style =\"background-color: red; font-weight: bold\">Actual:</span> [{actual}] " +
                "< span style =\"background-color: red; font-weight: bold\">but Expected:</span> [{expected}]\r\n");
            string fileName = "Error_Screen_" + Random;//TODO: inspect in future Regex.Match(Regex.Split(message, ". Object")[0], @"[\w\s.]+").Groups[1];
            try
             {
                Browser.SaveScreenShot(fileName);
            }
            catch (Exception e)
            {
                string newPath = Path.Combine(Directory.GetCurrentDirectory(), Browser.ActiveDir);
                // Create the subfolder
                Directory.CreateDirectory(newPath);
                newPath = Path.Combine(newPath, fileName + ".jpg");
                Logger.Instance.Info("Cannot create screenshot in folder " + newPath + ": " + e.Message);
            }
        }
    }
}
