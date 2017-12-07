using System;
using System.Diagnostics;
using System.Management;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    class ProcessUtils
    {
        /// <summary>
        /// returns collection of children processes
        /// </summary>
        /// <param name="pid">pid of parent process</param>
        /// <returns>collection of children processes</returns>
        public static ManagementObjectCollection GetChildren(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            return searcher.Get();
        }

        /// <summary>
        /// returns collection of children processes
        /// </summary>
        /// <param name="moc">parent process as ManagementObject</param>
        /// <returns>collection of children processes</returns>
        public static ManagementObjectCollection GetChildren(ManagementObjectCollection moc)
        {
            ManagementObjectCollection chromes = null;
            if (moc.Count == 1)
            {
                foreach (var mo in moc)
                {
                    var processId = (uint)mo["PROCESSID"];
                    var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processId);
                    chromes = searcher.Get();
                }
            }
            return chromes;
        }


        /// <summary>
        /// kill collection of processes
        /// </summary>
        /// <param name="processes">collection of processes</param>
        public static void KillProcesses(ManagementObjectCollection processes)
        {
            foreach (var process in processes)
            {
                try
                {
                    Process proc = Process.GetProcessById(Convert.ToInt32(process["ProcessID"]));
                    proc.Kill();
                }
                catch (ArgumentException e)
                {
                    Logger.Instance.Info(e.StackTrace);
                }

            }
        }

        #region deprecated method
        /// <summary>
        /// Kills parent process and all its children processes
        /// </summary>
        /// <param name="pid">Parent process PID</param>.
        [Obsolete("Method currently calles endless recursion, please use GetChildren() and KillProcesses() instead")]
        public static void KillProcessAndChildren(int pid)
        {
            ManagementObjectCollection moc = GetChildren(pid);
            foreach (ManagementObject mo in moc)
            {
                //Recursion should kill children processes at any depth
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException e)
            {
                Logger.Instance.Info(e.StackTrace);
            }
        }
        #endregion
    }
}
