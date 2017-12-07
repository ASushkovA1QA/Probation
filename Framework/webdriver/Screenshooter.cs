using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace WebdriverFramework.Framework.WebDriver
{
    public class Screenshooter
    {
        private static readonly Logger Log = Logger.Instance;
        public static ImageFormat ImageFormat
        {
            get
            {
                var strFormat = Configuration.ImageFormat;
                switch (strFormat.ToLowerInvariant())
                {
                    case "png":
                        return ImageFormat.Png;
                    case "jpg":
                        return ImageFormat.Jpeg;
                    case "bmp":
                        return ImageFormat.Bmp;
                    case "gif":
                        return ImageFormat.Gif;
                    case "wmf":
                        return ImageFormat.Wmf;
                    default:
                        return ImageFormat.Png;
                }
            }
        }

        public static string ImageFileFormat
        {
            get
            {
                var curFormat = ImageFormat;
                if (Equals(curFormat, ImageFormat.Jpeg))
                {
                    return ".jpg";
                }

                return "." + curFormat.ToString().ToLowerInvariant();
            }
        }

        public static BrowserFactory.BrowserType GetCurrentBrowserType()
        {
            var type = Environment.GetEnvironmentVariable("sbrowser", EnvironmentVariableTarget.User) ?? Configuration.Browser;
            BrowserFactory.BrowserType browserType;
            Enum.TryParse(type, true, out browserType);
            return browserType;
        }

        public static string SaveScreenShot(string pathToScreenFolder, Browser browser, int screenNumber)
        {
            if (!Directory.Exists(pathToScreenFolder))
            {
                Directory.CreateDirectory(pathToScreenFolder);
            }

            var fileName = Regex.Replace(screenNumber.ToString("D4"), "[\\W]+", string.Empty) + ImageFileFormat;
            var todayPath = GetTodayPath(pathToScreenFolder, fileName);
            try
            {
                var screenshot = Browser.GetScreenshot();
                screenshot.SaveAsFile(todayPath, ImageFormat);
                Log.Info(string.Format("Screenshot was saved '{0}'", todayPath));
            }
            catch (Exception e)
            {
                Log.Warn(string.Format("Не удалось сохранить скриншот {0} по причине:\r\n{1}", todayPath, e.Message));
            }

            return todayPath;
        }

        public static string GetTodayPath(string pathToScreenFolder, string fileName)
        {
            return Path.Combine(pathToScreenFolder, fileName);
        }


        public static string SaveScreenShot(string fileName, string pathToScreenFolder, Browser browser)
        {
            var today = DateTime.Now;
            var time = today.ToString("HH.mm.ss");
            if (!Directory.Exists(pathToScreenFolder))
            {
                Directory.CreateDirectory(pathToScreenFolder);
            }

            fileName = fileName + time + ImageFileFormat;
            fileName = fileName.Length < 25 ? fileName + "somelettershere" : fileName;
            fileName = Regex.Match(fileName.Substring(0, 15), @"[\w\s]+").Groups[0] + "_" + time + ImageFileFormat;
            var todayPath = GetTodayPath(pathToScreenFolder, fileName);
            try
            {
                Log.Info(string.Format("Current url: '{0}'", Browser.GetCurrentUrl()));
                var entereScreenshot = GetEntirePageScreenshot();
                entereScreenshot.Save(todayPath, ImageFormat);
                Log.Info(string.Format("Screenshot was saved '{0}'", todayPath));
            }
            catch (Exception e)
            {
                Log.Warn(string.Format("Не удалось сохранить скриншот {0} по причине:\r\n{1}", todayPath, e.Message));
            }

            return todayPath;
        }

        public static Bitmap GetEntirePageScreenshot()
        {
            Browser.ScrollToTopViaJs();
            System.Threading.Thread.Sleep(200);

            Bitmap stitchedImage = null;
            try
            {
                var totalWidth = Browser.GetTotalWidth();
                var totalHeight = Browser.GetTotalHeight();
                var viewportWidth1 = (long)Browser.GetDriverStatic().ExecuteScript("return document.body.clientWidth");
                var viewportHeight1 = (long)Browser.GetDriverStatic().ExecuteScript("return window.innerHeight");
                var viewportWidth = (int)viewportWidth1;
                var viewportHeight = (int)viewportHeight1;

                var rectangles = SplitPageIntoRectangles(totalHeight, viewportHeight, totalWidth, viewportWidth);

                stitchedImage = new Bitmap(totalWidth, totalHeight);
                var previous = Rectangle.Empty;
                foreach (var rectangle in rectangles)
                {
                    var screenshotImage = GetRectanglePartOfScreenshotWithScroll(previous, rectangle, viewportHeight);

                    var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                    using (var g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }

                    previous = rectangle;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format("Не удалось сохранить скриншот по причине:\r\n{0}", ex.Message));
            }

            return stitchedImage;
        }

        private static List<Rectangle> SplitPageIntoRectangles(int totalHeight, int viewportHeight, int totalWidth, int viewportWidth)
        {
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < totalHeight; i += viewportHeight)
            {
                var newHeight = viewportHeight;
                totalHeight += (int)(viewportHeight * 0.1);
                if (i + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - i;
                }

                for (var ii = 0; ii < totalWidth; ii += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    if (ii + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - ii;
                    }

                    var currRect = new Rectangle(ii, i, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }

            return rectangles;
        }

        private static Image GetRectanglePartOfScreenshotWithScroll(Rectangle previous, Rectangle rectangle, int viewportHeight)
        {
            if (previous != Rectangle.Empty)
            {
                var xDiff = rectangle.Right - previous.Right;
                var yDiff = rectangle.Bottom - previous.Bottom - (int)(viewportHeight * 0.1);

                ((IJavaScriptExecutor)Browser.GetDriverStatic()).ExecuteScript(string.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                System.Threading.Thread.Sleep(200);
            }

            var screenshot = ((ITakesScreenshot)Browser.GetDriverStatic()).GetScreenshot();
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }

            return screenshotImage;
        }
    }
}
