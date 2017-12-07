namespace WebdriverFramework.Tests.Utils
{
    public class Enums
    {
        public enum status
        {
            PASSED,
            BLOCKED,
            RETEST,
            FAILED
        }

        public static string GetTestStatus(status statusEnum)
        {
            switch (statusEnum)
            {
                case status.PASSED:
                    return "1";
                case status.BLOCKED:
                    return "2";
                case status.RETEST:
                    return "4";
                case status.FAILED:
                    return "5";
                default: return null;
            }
        }
    }
}
