using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StudioCore;
using System.Windows.Forms;
using System.Security.Permissions;
using Veldrid.Sdl2;

namespace DSMapStudio
{
    public static class Program
    {
        public static string[] ARGS;

        private static string _version = Application.ProductVersion;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);

            //SDL_version version;
            //Sdl2Native.SDL_GetVersion(&version);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            var mapStudio = new MapStudioNew();
#if !DEBUG
            try
            {
                mapStudio.Run();
            }
            catch
            {
                mapStudio.AttemptSaveOnCrash();
                mapStudio.CrashShutdown();
                // Throw to trigger CrashHandler
                throw;
            }
#else
            mapStudio.Run();
#endif
        }

        static List<string> LogExceptions(Exception ex)
        {
            List<string> log = new();
            do
            {
                if (ex is AggregateException ae)
                {
                    ex = ae.Flatten();
                }
                log.Add(ex.Message);
                log.Add(ex.StackTrace);
                ex = ex.InnerException;
                log.Add("----------------------\n");
            }
            while (ex != null);
            log.RemoveAt(log.Count - 1);
            return log;
        }


        static string CrashLogPath = $"{Directory.GetCurrentDirectory()}\\Crash Logs";
        static void ExportCrashLog(List<string> exceptionInfo)
        {
            var time = $"{DateTime.Now:yyyy-M-dd--HH-mm-ss}";
            exceptionInfo.Insert(0, $"DSMapStudio Version {_version}");
            Directory.CreateDirectory($"{CrashLogPath}");
            var crashLogPath = $"{CrashLogPath}\\Log {time}.txt";
            File.WriteAllLines(crashLogPath, exceptionInfo);

            if (exceptionInfo.Count > 10)
                MessageBox.Show($"DSMapStudio has run into an issue.\nCrash log has been generated at \"{crashLogPath}\".",
                    $"DSMapStudio Unhandled Error - {_version}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show($"DSMapStudio has run into an issue.\nCrash log has been generated at \"{crashLogPath}\".\n\nCrash Log:\n{string.Join("\n", exceptionInfo)}",
                    $"DSMapStudio Unhandled Error - {_version}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("Crash caught : " + e.Message);
            Console.WriteLine("Stack Trace : " + e.StackTrace);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);

            List<string> log = LogExceptions(e);
            ExportCrashLog(log);
        }
    }
}
