using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// provide useful methods to executing some commands in the cmd.exe
    /// </summary>
    public static class CommandLineExecutor
    {
        private const int Timeout = 500;


        /// <summary>
        /// Execute command in windows command-line
        /// </summary>
        /// <param name="fileName">path to executed file</param>
        /// <param name="arguments">command to execute</param>
        /// <returns>output of execution</returns>
        public static string ExecuteCommand(string fileName, string arguments)
        {
            return ExecuteCommand(fileName, arguments, true);
        }

        /// <summary>
        /// Execute command in windows command-line
        /// method will return null if waitForExit = false
        /// </summary>
        /// <param name="fileName">path to executed file</param>
        /// <param name="arguments">command to execute</param>
        /// <param name="waitForExit">true if there is nessessary in waiting for proccess is exit</param>
        /// <returns>output of execution</returns>
        public static string ExecuteCommand(string fileName, string arguments, bool waitForExit)
        {
            Logger.Instance.Info("executing command: " + Regex.Match(fileName, "[\\w.]+$").Groups[0] + " " + arguments);
            var startInfo = new ProcessStartInfo {FileName = fileName};
            if (arguments.Length != 0)
            {
                startInfo.Arguments = arguments;
            }
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            var cmd = new Process {StartInfo = startInfo};
            try
            {
                cmd.Start();
            }
            catch (Win32Exception)
            {
                throw new Exception(String.Format("File {0} was not found in runtime", fileName));
            }
            if (waitForExit)
            {
                Browser.Sleep(Timeout);
                string output = cmd.StandardOutput.ReadToEnd();
                Logger.Instance.Info(output);
                cmd.WaitForExit();
                return output;
            }
            return null;
        }


    }
}
