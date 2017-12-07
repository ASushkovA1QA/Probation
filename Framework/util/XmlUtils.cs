using System.Xml;

namespace WebdriverFramework.Framework.Util
{
    public class XmlUtils
    {
        public static string GetNodeText(string value, string xpathExpression)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(value);
            var titleNodes = xmlDoc.SelectNodes(xpathExpression);
            return titleNodes.Item(0).InnerText;
        }
    }
}