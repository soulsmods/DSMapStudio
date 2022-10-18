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
        //public static MapStudio MainInstance;

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
            catch (AggregateException e)
            {
                var stackTraceOrigin = e.StackTrace;
                e = e.Flatten();
                if (e.InnerExceptions.Count > 1)
                {
                    List<string> log = new();
                    foreach (var inner in e.InnerExceptions)
                    {
                        log.Add(inner.Message);
                        if (inner.StackTrace != null)
                            log.Add(inner.StackTrace);
                        else
                            log.Add(stackTraceOrigin);
                        log.Add("\n----------------\n");
                    }
                    log.RemoveAt(log.Count - 1);
                    mapStudio.ExportCrashLog(log);
                }
                else
                {
                    if (e.StackTrace != null)
                        mapStudio.ExportCrashLog((e.InnerExceptions[0].Message + "\n" + e.InnerExceptions[0].StackTrace).Replace("\0", "\\0"));
                    else
                        mapStudio.ExportCrashLog((e.InnerExceptions[0].Message + "\n" + stackTraceOrigin).Replace("\0", "\\0"));
                }
                mapStudio.AttemptSaveOnCrash();
                mapStudio.CrashShutdown();
                throw;
            }
            catch (Exception e)
            {
                mapStudio.ExportCrashLog((e.Message + "\n" + e.StackTrace).Replace("\0", "\\0"));
                mapStudio.AttemptSaveOnCrash();
                mapStudio.CrashShutdown();
                throw;
            }
#else
            mapStudio.Run();
#endif
        }

        static void CrashHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("Crash caught : " + e.Message);
            Console.WriteLine("Stack Trace : " + e.StackTrace);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }
    }
}
