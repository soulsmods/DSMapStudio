﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using StudioCore;
using System.Security.Permissions;
using Microsoft.DotNet.PlatformAbstractions;
using StudioCore.Graphics;
using StudioCore.Platform;
using Veldrid.Sdl2;

namespace DSMapStudio
{
    public static class Program
    {
        private static string _version = "undefined";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CrashHandler;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException());
            _version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? "undefined";
            var mapStudio = new MapStudioNew(new VulkanGraphicsContext(), _version);
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
                PlatformUtils.Instance.MessageBox($"DSMapStudio has run into an issue.\nCrash log has been generated at \"{crashLogPath}\".",
                    $"DSMapStudio Unhandled Error - {_version}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                PlatformUtils.Instance.MessageBox($"DSMapStudio has run into an issue.\nCrash log has been generated at \"{crashLogPath}\".\n\nCrash Log:\n{string.Join("\n", exceptionInfo)}",
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
