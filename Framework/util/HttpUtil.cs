using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.Util
{
    /// <summary>
    /// Для логгирования
    /// </summary>
    public class HttpUtil
    {
        private static readonly Logger Log = Logger.Instance;
        public static string PostRequest(string url, NameValueCollection postParameters)
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            var postData = string.Empty;
            foreach (var key in postParameters.Keys)
            {
                postData += "&" + key + "=" + HttpUtility.UrlEncode(postParameters[key.ToString()]);
            }

            if (postData.Length > 0)
            {
                postData = postData.Substring(1);
            }

            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.Date = DateTime.Now;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (var requestWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
            {
                requestWriter.Write(postData);
            }

            var myHttpWebResponse = GetResponse(myHttpWebRequest);
            return myHttpWebResponse;
        }

        public static string GetResponse(HttpWebRequest request)
        {
            request.Timeout = 60000;
            try
            {
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return string.Empty;
                }

                using (var responseStream = response.GetResponseStream())
                {
                    using (var readerStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return readerStream.ReadToEnd();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Log.Warn(string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message));
                return null;
            }
            catch (WebException ex)
            {
                Log.Warn(string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message));
                Log.Warn(string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.StackTrace));
                return null;
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message));
                Log.Warn(string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.StackTrace));
                return null;
            }
        }
    }
}
