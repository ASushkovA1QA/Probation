using System;
using System.Collections.Generic;
namespace WebdriverFramework.Framework
{
    /// <summary>
    /// class for storing object in runtime. It is used for _store test date between tests
    /// </summary>
    /// <typeparam name="T">Type of stored objects</typeparam>
    public static class Store<T>
    {
        private static readonly Dictionary<int , T> _store = new Dictionary<int, T>();

        /// <summary>
        /// Store object in the dictionary
        /// </summary>
        /// <param name="testid">testid is key in dictionary. In generally should be equals test case ID</param>
        /// <param name="storedObject">object, for example Order</param>
        public static void StoreObject(int testid, T storedObject)
        {
            _store.Add(testid, storedObject);
        }

        /// <summary>
        /// returns object from dictionary by testid
        /// </summary>
        /// <param name="testid">testid is key in dictionary. In generally should be equals test case ID</param>
        /// <returns></returns>
        public static T GetFromStore(int testid)
        {
            try
            {
                return _store[testid];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(typeof(T) +  " was no created in test case with ID " + testid);
            }
        }
    }
}
