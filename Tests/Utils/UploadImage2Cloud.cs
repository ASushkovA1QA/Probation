using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebdriverFramework.Framework.WebDriver;
using System.IO;

namespace WebdriverFramework.Tests.Utils
{
    public  class UploadImage2Cloud
    {
        public static String Upload(string imagePath, string imageCloudName)
        {
            var cloudinary = new Cloudinary(
              new Account(
                    ConfigReader.GetCloudName(),
                    ConfigReader.GetCloudApiKey(),
                    ConfigReader.GetCloudApiSecret()));

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Directory.GetCurrentDirectory() + imagePath),
                PublicId = imageCloudName,
                //Tags = "special, for_homepage"
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult.Uri.ToString();
        }
    }
}
