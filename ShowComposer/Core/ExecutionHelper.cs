using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FileExecutionHelper
{
    public class ExecutionHelper
    {
        public static void ExecuteCommand(string command, out string output, out string error)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            process.Close();
        }

        public static void RunCmdCommand(string command, out string output)
        {
            RunCmdCommand(command, out output, false, 866);
        }

        public static void RunCmdCommand(string command, out string output, bool useOutput, int chcp)
        {

            output = null;

            Process pr = new Process();

            pr.StartInfo.FileName = @"cmd.exe";
            pr.StartInfo.UseShellExecute = false;
            pr.StartInfo.RedirectStandardInput = true; // перенаправление вывода
            pr.StartInfo.RedirectStandardOutput = true; // перенаправление вывода
            pr.StartInfo.CreateNoWindow = true; // Не показывать окно cmd

            pr.Start();

            //if (useOutput == true)
                pr.StandardInput.WriteLine("chcp " + chcp); // Установить utf8 для вывода

            var dfgdkl = command.Replace('\\', ' ');

            pr.StandardInput.WriteLine(command);

            if (useOutput == true)
                output = pr.StandardOutput.ReadToEnd();

            pr.StandardInput.WriteLine("exit");
            pr.WaitForExit();
            pr.Close();
            pr.Dispose();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }
    }
}
