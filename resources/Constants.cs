using System.IO;

namespace WebdriverFramework.resources
{
    public class Constants
    {
        // configuration params
        public static readonly string SuccessMessage = "Succesfully Logged In";        
        public static readonly string TestConfiguration = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), @"../resources", "testconfiguration.xml"));
        public static readonly string InvalidValue = "InvalidValue";        
    }
}
