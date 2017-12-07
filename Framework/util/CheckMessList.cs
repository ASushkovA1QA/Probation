using System;
using System.Collections.Generic;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// list of the messages. simple wrapped over List
    /// </summary>
    public class CheckMessList : List<String>
    {
        /// <summary>
        /// Initilize list of the messages and set HasWarn var of the BaseTest to false
        /// </summary>
        public CheckMessList()
        {
            BaseTest.HasWarn = false;
        }

        /// <summary>
        /// add message to List and set HasWarn to true
        /// </summary>
        /// <param name="message">message with reason of the warn</param>
        public new void Add(String message)
        {
            BaseTest.HasWarn = true;
            base.Add(message);
        }
    }
}
