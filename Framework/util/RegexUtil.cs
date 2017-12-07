using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// usefull regular expression
    /// </summary>
    public class RegexUtil
    {
        /// <summary>
        /// replace all groups with specified index in input
        /// </summary>
        /// <param name="input">text to replacing</param>
        /// <param name="pattern">regex pattern</param>
        /// <param name="group">number of group to replace</param>
        /// <param name="value">value that will be placed instead</param>
        /// <returns>text after replacing</returns>
        public static string ReplacePattern(string input, string pattern, int group, string value)
        {
            string inputString = input;
            foreach (Match match in Regex.Matches(inputString, pattern))
            {
                inputString = Regex.Replace(input, match.Groups[group].ToString(), value);
            }
            return inputString;
        }

        /// <summary>
        /// returns set of matched values
        /// </summary>
        /// <param name="input">text</param>
        /// <param name="pattern">pattern to match</param>
        /// <param name="group">group</param>
        /// <returns>set with matching</returns>
        public static HashSet<string> GetMatchedSet(string input, string pattern, int group)
        {
            var set = new HashSet<string>();
            foreach (Match m in Regex.Matches(input, pattern))
            {
                set.Add(m.Groups[group].ToString());
            }
            return set;
        }

        /// <summary>
        /// returns random string
        /// </summary>
        /// <param name="length">int</param>
        /// <returns>random string</returns>
        public static string GetRandomAlphaNumeric(int length)
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, length);
        }

        public static MatchCollection GetRegexItems(String Source, String RegexOpinionsText)
        {
            Regex regex = new Regex(RegexOpinionsText);
            return regex.Matches(Source);
        }

        public static Match GetRegexItem(String Source, String RegexOpinionsText)
        {
            Regex regex = new Regex(RegexOpinionsText);
            return regex.Match(Source);
        }
    }
}
