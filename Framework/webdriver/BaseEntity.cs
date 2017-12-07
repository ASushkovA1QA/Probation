using System;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// base entity, contains instances browser and log
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Browser singletone instance
        /// </summary>
        protected static Browser Browser;
        
        /// <summary>
        /// Logger singletone instance
        /// </summary>
        protected static Logger Log = Logger.Instance;

        /// <summary>
        /// abstract method that must be overrided in the inherited classes
        /// allow mark log message with specified prefix
        /// for example:
        /// if logging executed from the form class in the log we can see:
        /// FORM ... message
        /// when FORM is result taht returns overrided method
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected abstract String FormatLogMsg(string message);

        /// <summary>
        /// debug in log
        /// </summary>
        /// <param name="message">message for log</param>
        protected void Debug(String message)
        {
            Log.Debug(FormatLogMsg(message));
        }

        /// <summary>
        /// info in log
        /// </summary>
        /// <param name="message">message for log</param>
        protected void Info(String message)
        {
            Log.Info(FormatLogMsg(message));
        }

        /// <summary>
        /// warn in log
        /// </summary>
        /// <param name="message">message for log</param>
        protected void Warn(String message)
        {
            Log.Warn(FormatLogMsg(message));
        }

        /// <summary>
        /// error in log
        /// </summary>
        /// <param name="message">message for log</param>
        protected void Error(String message)
        {
            Log.Error(FormatLogMsg(message));
        }

        /// <summary>
        /// fatal in log
        /// </summary>
        /// <param name="message">message for log</param>
        protected void Fatal(String message)
        {
            Log.Fail(FormatLogMsg(message));
        }

        /// <summary>
        /// log step in log
        /// </summary>
        /// <param name="step">number of the step</param>
        /// <param name="message">message for log</param>
        protected void LogStep(int step, String message)
        {
            Log.LogStep(step, message);
        }

        /// <summary>
        /// log step information starts from NOT IMPLEMENTED
        /// </summary>
        /// <param name="step">step number</param>
        /// <param name="message">step description</param>
        protected void LogNotImplementedStep(int step, String message)
        {
            LogStep(step, "NOT IMPLEMENTTED::" + message);
        }

        /// <summary>
        /// log step without messsage in log
        /// </summary>
        /// <param name="step">number of the step</param>
        protected void LogStep(int step)
        {
            Log.LogStep(step);
        }

        /// <summary>
        /// log step range of steps with action message
        /// </summary>
        /// <param name="step">step</param>
        /// <param name="toStep">toStep</param>
        /// <param name="message">message</param>
        protected void LogStep(int step, int toStep, string message)
        {
            Log.LogStep(step, toStep, message);
        }
    }
}
