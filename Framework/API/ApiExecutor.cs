using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.API
{
    /// <summary>
    /// class for executing web api requests
    /// </summary>
    public class ApiExecutor
    {
        /// <summary>
        /// Executes webrequest by specified uri.
        /// After the execution logs request and response
        /// </summary>
        /// <param name="uri">specified uri</param>
        /// <returns>response, readen with StreamReader from the response's stream</returns>
        public string ExecuteRequest(string uri)
        {
            Logger.Instance.Info("Get response from: " + uri);
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();
            Logger.Instance.Info(string.Format("Request {0} was executed with Response Code {1}", uri, ((HttpWebResponse)response).StatusDescription));
            Stream dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);
                return reader.ReadToEnd();
            }
            throw new Exception("There was empty response from API by " + uri);
        }


        /// <summary>
        /// execute SOAP request to API
        /// </summary>
        /// <param name="url">SOAP api url</param>
        /// <param name="action">SOAP ACTION</param>
        /// <param name="soapXmlRequest">SOAP REQUEST xml</param>
        /// <returns>SOAP RESPONSE xml</returns>
        public string ExecuteSoapRequest(string url, string action, string soapXmlRequest)
        {
            Logger.Instance.Info("============== Execute SOAP REQUEST =================");
            Logger.Instance.Info("API: " + url);
            Logger.Instance.Info("SOAP ACTION: " + action);
            string fileNameRequest = "REQUEST_" + DateTime.Now.ToString("yyyyMMddHHmmsss", CultureInfo.InvariantCulture) + ".xml";
            File.Create(fileNameRequest).Close();
            File.WriteAllText(fileNameRequest, soapXmlRequest);
            Logger.Instance.Info("PATH TO REQUEST " + Path.GetFullPath(fileNameRequest));
            Logger.Instance.Info("============== ==================== =================");
            
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(soapXmlRequest);
            HttpWebRequest webRequest = CreateWebRequest(url, action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                string soapResult;
                using (var rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                Logger.Instance.Info("============== Execute SOAP RESPONSE =================");
                var formatted = GetFormattedXml(soapResult);
                string fileName = "RESPONSE_" + DateTime.Now.ToString("yyyyMMddHHmmsss", CultureInfo.InvariantCulture) + ".xml";
                File.Create(fileName).Close();
                File.WriteAllText(fileName, formatted);
                Logger.Instance.Info("PATH TO RESPONSE " + Path.GetFullPath(fileName));
                Logger.Instance.Info("============== ==================== =================");
                return soapResult;
            }
        }

        private string GetFormattedXml(string xml)
        {
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, Encoding.Unicode);
            var document = new XmlDocument();
            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String formattedXml = sReader.ReadToEnd();
                return formattedXml;
            }
            catch (XmlException)
            {
                return xml;
            }
        }

        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private XmlDocument CreateSoapEnvelope(string soapXmlRequest)
        {
            var soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(soapXmlRequest);
            return soapEnvelop;
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}
