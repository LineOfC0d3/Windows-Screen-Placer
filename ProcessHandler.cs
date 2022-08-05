using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Windows_Screen_Placer
{
    internal static class ProcessHandler
    {
        public static Process HandledProcess { get; private set; }

        public static void Start(string exePath, bool forceAdmin = false)
        {
            Start(exePath, "");
        }

        public static void Start(string exePath, string args)
        {
            HandledProcess = new Process();
            HandledProcess.StartInfo.FileName = exePath;
            HandledProcess.StartInfo.Arguments = args;
            HandledProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            try
            {
                ExecuteNormally();
            }
            catch (Win32Exception)
            {
                ExecuteAsAdmin();
            }
            HandledProcess.WaitForInputIdle();
            Debug.WriteLine(HandledProcess.Id);
        }

        private static void ExecuteNormally()
        {
            HandledProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            HandledProcess.Start();
        }

        private static void ExecuteAsAdmin()
        {
            HandledProcess.StartInfo.UseShellExecute = true;
            HandledProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            HandledProcess.StartInfo.Verb = "runas";
            HandledProcess.Start();
        }
    }
}