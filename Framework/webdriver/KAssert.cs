using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebdriverFramework.Framework.util;

namespace WebdriverFramework.Framework.WebDriver
{
    /// <summary>
    /// wrapper over MSTest assert
    /// allow logging results of the assertions
    /// </summary>
    public class KAssert
    {
        private const string False = "FALSE >> ";
        private static readonly Logger Log = Logger.Instance;
        
        /// <summary>
        /// may be called when we need to fail current test immediately
        /// </summary>
        /// <param name="message"></param>
        public static void Fail(string message)
        {
            Checker.Messages.Add(message);
            Assert.Fail(message + Log.LastExecutedStepMessage);
        }

        /// <summary>
        /// assert true 
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="condition"> bool var</param>
        public static void AssertTruePosMsg(string posMessage, bool condition)
        {
            if (!condition)
            {
                Checker.Messages.Add(False + posMessage);
                Assert.IsTrue(condition, False + posMessage + Log.LastExecutedStepMessage);
            }
            else
            {
                Log.Info(posMessage);
            }


        }
        /// <summary>
        /// Assert True from ignored message
        /// </summary>
        /// <param name="posMessage"> message </param>
        /// <param name="condition"> bool var </param>
        public static void AssertTruePosMsgTempIgnoring(string posMessage, bool condition)
        {
            try
            {
                Assert.IsTrue(condition, False + posMessage + Log.LastExecutedStepMessage);
            }
            catch (AssertFailedException ex)
            {
                Log.Warn("############################## IGNORED #################################");
                Log.Warn(ex.Message);
                Log.Warn("############################## IGNORED #################################");
            }
            Log.Info(posMessage);
        }

        /// <summary>
        /// assert false 
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="condition"> bool var</param>
        internal static void AssertFalsePosMsg(string posMessage, bool condition)
        {
            if (condition)
            {
                Checker.Messages.Add(False + posMessage);
                Assert.IsFalse(condition, False + posMessage + Log.LastExecutedStepMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }
        /// <summary>
        /// assert true 
        /// </summary>
        /// <param name="message"> message</param>
        /// <param name="condition"> bool var</param>
        internal static void AssertTrue(string message, bool condition)
        {
            if (!condition)
            {
                Checker.Messages.Add(message);
                Assert.IsTrue(condition, message + Log.LastExecutedStepMessage);
            }
        }
        /// <summary>
        /// assert Equals 
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="expected"> expected bool result</param>
        /// <param name="actual"> actual bool result</param>
        public static void AssertEqualsPosMsg(string posMessage, bool expected, bool actual)
        {
            if (expected != actual)
            {
                Checker.Messages.Add(False + posMessage);
                Assert.AreEqual(expected, actual, False + posMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }
        /// <summary>
        /// assert Equals for files that realized IComparable interface
        /// </summary>
        /// <param name="posMessage"> Positive result message</param>
        /// <param name="expected"> expected object result</param>
        /// <param name="actual"> actual object result</param>
        public static void AssertEqualsPosMsg(string posMessage, object expected, object actual)
        {
            if (!expected.Equals(actual))
            {
                Checker.Messages.Add(False + posMessage);
                Assert.AreEqual(expected, actual, False + posMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }

        /// <summary>
        /// assert Equals 
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="expected">expected bool result</param>
        /// <param name="actual">actual bool result</param>
        public static void AssertEquals(string posMessage, bool expected, bool actual)
        {
            if (expected != actual)
            {
                Checker.Messages.Add(False + posMessage);
                Assert.AreEqual(expected, actual, False + posMessage + Log.LastExecutedStepMessage);
            }
        }

        /// <summary>
        /// assert Equals 
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="expected"> expected bool result</param>
        /// <param name="actual"> actual bool result</param>
        public static void AssertEqualsPosMsg(string posMessage, string expected, string actual)
        {
            if (expected != actual)
            {
                Checker.Messages.Add(False + posMessage);
                Assert.AreEqual(expected, actual, False + posMessage + Log.LastExecutedStepMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }
        /// <summary>
        /// assert contains value
        /// </summary>
        /// <param name="posMessage"> mesage</param>
        /// <param name="expected">expected string</param>
        /// <param name="actual"> actual string </param>
        public static void AssertContainsPosMsg(string posMessage, string expected, string actual)
        {
            if (!actual.Contains(expected))
            {
                Checker.Messages.Add(False + posMessage);
                Assert.IsTrue(actual.Contains(expected), False + posMessage + Log.LastExecutedStepMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }
        /// <summary>
        /// Assert Not Equals
        /// </summary>
        /// <param name="posMessage"> message</param>
        /// <param name="expected">expected string </param>
        /// <param name="actual">actual string</param>
        public static void AssertNotEqualsPosMsg(string posMessage, string expected, string actual)
        {
            if (expected.Equals(actual))
            {
                Checker.Messages.Add(False + posMessage);
                Assert.AreNotEqual(expected, actual, False + posMessage + Log.LastExecutedStepMessage);
            }
            else
            {
                Log.Info(posMessage);
            }
        }
        /// <summary>
        /// assert true
        /// </summary>
        /// <param name="value"> bool value</param>
        public static void AssertTrue(bool value)
        {
            if (!value)
            {
                Checker.Messages.Add(Log.LastExecutedStepMessage);
                Assert.IsTrue(value, Log.LastExecutedStepMessage);
            }
        }
        /// <summary>
        /// Assert True
        /// </summary>
        /// <param name="value">bool value</param>
        /// <param name="condition"> condition msg if not true </param>
        public static void AssertTrue(bool value, string condition)
        {
            if (!value)
            {   
                Checker.Messages.Add(condition);
                Assert.IsTrue(value, "FAIL:\r\n Expected condition: " + condition + 
                    "\r\n Expected: TRUE" + 
                    "\r\n But was: FALSE");
            }
            Log.Info(condition);
        }
    }
}
