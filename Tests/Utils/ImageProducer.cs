using System;
using System.Drawing.Imaging;
using System.IO;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Tests.Utils
{
    public class ImageProducer
    {
        public static String SetScreenshot()
        {
            Browser.GetScreenshot().SaveAsFile(Directory.GetCurrentDirectory() + ConfigReader.GetScreenshotName(), ImageFormat.Jpeg);
            return UploadImage2Cloud.Upload(ConfigReader.GetScreenshotName(), "Screenshot_date_" + DateTime.Now.ToString());
        }
    }
}
