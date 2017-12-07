using System.Collections.ObjectModel;
using OpenQA.Selenium;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.Util
{
    public class CookieManager
    {
        private static readonly Logger Log = Logger.Instance;

        public static void DeleteAllCookie()
        {
            Browser.Instance.GetDriver().Manage().Cookies.DeleteAllCookies();
            Browser.Refresh();
        }

        public ReadOnlyCollection<Cookie> GetAllCookies()
        {
            return Browser.Instance.GetDriver().Manage().Cookies.AllCookies;
        }

        public static void CheckBrowserCookies(string keywords, bool shouldExists)
        {
            var keyArray = keywords.Split(',');
            Log.Info("checking browser's cookies for " + keywords);
            var cookies = Browser.Instance.GetDriver().Manage().Cookies.AllCookies;
            var exist = false;
            foreach (var cookie in cookies)
            {
                Log.Info(cookie.ToString());
                if (cookie.ToString().Contains(keyArray[0]))
                {
                    exist = true;
                }
            }

            var result = exist;
            KAssert.AssertEqualsPosMsg("Cookies are valid", shouldExists, result);
        }
    }
}
