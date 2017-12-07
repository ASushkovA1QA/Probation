using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTestResultViewer.Consol
{
    public class AutoParser
    {
        /// <summary>
        /// Invokes all parsing method for html documents
        /// </summary>
        /// <param name="inputPath">Input File path</param>
        /// <param name="outputPath">Outpit File Path; may be the same as input path</param>
        public static void ParseOneFile(string inputPath, string outputPath)
        {
            StreamReader reader = new StreamReader(inputPath);
            string file = reader.ReadToEnd();
            reader.Close();

            file = ChangeTextColor(file);
            file = ParseImages(file);

            StreamWriter writer = new StreamWriter(outputPath, false, Encoding.Unicode);
            writer.Write(file);
            writer.Close();
        }

        /// <summary>
        /// Parses HTML file for "TEST PASSED" and "TEST FAILED" text and colors them.
        /// Adds background color for the entire document depending on results.
        /// </summary>
        /// <param name="htmlText">String with HTML-text</param>
        /// <returns>HTML-text with edited code</returns>
        private static string ChangeTextColor(string htmlText)
        {
            if (htmlText.Contains("TEST PASSED"))
            {
                //блок для покраски всего докумета при успешном тесте
                //открывающий тег
                htmlText = htmlText.Insert(0, "<div style = \"background: Aquamarine\">");
                //закрывающий тег
                htmlText = htmlText.Insert(htmlText.Length, "</div>");

                //блок для строки "TEST PASSED"
                int textStartIndex = htmlText.IndexOf("TEST PASSED");
                int textEndIndex = textStartIndex + 11; //11 потому что днина блока 11
                //Редактируем открывающий тег
                for (int i = textStartIndex; i > 0; i--)
                {
                    if (htmlText[i] == '<')
                    {
                        htmlText = htmlText.Insert(i + 2, " style = \"background: lime; font-weight: bold\"");
                        break;
                    }
                }
            }
            else if (htmlText.Contains("TEST FAILED"))
            {
                //блок для покраски всего докумета при зафейленном тесте
                //открывающий тег
                htmlText = htmlText.Insert(0, "<div style = \"background: Pink\">");
                //закрывающий тег
                htmlText = htmlText.Insert(htmlText.Length, "</div>");

                //блок для строки "TEST FAILED"
                int textStartIndex = htmlText.IndexOf("TEST FAILED");
                int textEndIndex = textStartIndex + 11; //11 потому что днина блока 11
                //Редактируем открывающий тег
                for (int i = textStartIndex; i > 0; i--)
                {
                    if (htmlText[i] == '<')
                    {
                        htmlText = htmlText.Insert(i + 2, " style = \"color: white; background: red; font-weight: bold\"");
                        break;
                    }
                }
            }
            return htmlText;
        }

        /// <summary>
        /// Finds all images in HTML and parses them. Then invokes CreateImageLink for each image
        /// </summary>
        /// <param name="htmlText">String with HTML-text</param>
        /// <returns>HTML-text with edited code</returns>
        private static string ParseImages(string htmlText)
        {
            string template = "class=\"magnify\"/>";
            List<string> imagesBlocks = new List<string>();

            if (htmlText.Contains(template))
            {
                int pos = htmlText.IndexOf(template) + 17; //17 потому что днина блока 17
                imagesBlocks.Add(htmlText.Substring(0, pos));
                imagesBlocks.Add(htmlText.Substring(pos, (htmlText.Length - pos)));
            }

            if (imagesBlocks.Count == 0)
                return htmlText;

        restart:
            int lastImageBlock = imagesBlocks.Count - 1;
            if (imagesBlocks.Count > 0 && imagesBlocks[lastImageBlock].Contains(template))
            {
                int pos = imagesBlocks[lastImageBlock].IndexOf(template) + 17;
                imagesBlocks.Add(imagesBlocks[lastImageBlock].Substring(pos, (imagesBlocks[lastImageBlock].Length - pos)));
                imagesBlocks[lastImageBlock] = imagesBlocks[lastImageBlock].Substring(0, pos);
                goto restart;
            }

            for (int i = 0; i < imagesBlocks.Count; i++)
            {
                imagesBlocks[i] = CreateImageLink(imagesBlocks[i]); //Add assignation
            }

            StringBuilder sb = new StringBuilder();
            foreach (string html in imagesBlocks)
            {
                sb.Append(html);
            }
            string result = sb.ToString();

            return result;
        }

        /// <summary>
        /// Parses HTML file for image and generates links for them
        /// </summary>
        /// <param name="htmlText">String with HTML-text</param>
        /// <returns>HTML-text with edited code</returns>
        private static string CreateImageLink(string htmlText)
        {
            string template = "div><img";
            if (htmlText.Contains(template))
            {
                if (htmlText.Contains("<img"))
                {
                    int srcStartPos = htmlText.IndexOf("src=\"") + 5;
                    int srcEndPos = htmlText.IndexOf("\"", srcStartPos);
                    int srcLenght = srcEndPos - srcStartPos;
                    string imageSrc = htmlText.Substring(srcStartPos, srcLenght);
                    string imageTag = ("<a target=\"_blank\" href=\"" + imageSrc + "\">");

                    //создаём открывающий тег
                    htmlText = htmlText.Insert(htmlText.IndexOf(template) + 4, imageTag);
                    //создаём закрывающий тег
                    for (int i = htmlText.IndexOf("\"><img") + 2; i < htmlText.Length; i++)
                    {
                        if (htmlText[i] == '>')
                        {
                            htmlText = htmlText.Insert(i + 1, "</a>");
                            break;
                        }
                    }
                }
            }
            return htmlText;
        }
    }
}
