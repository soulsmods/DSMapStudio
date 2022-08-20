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
        static async Task Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashHandler);

            //SDL_version version;
            //Sdl2Native.SDL_GetVersion(&version);

            #if !DEBUG
            try
            {
                var mapstudio = new StudioCore.MapStudioNew();
                await mapstudio.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show((e.Message + "\n" + e.StackTrace).Replace("\0", "\\0"), "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw e;
            }
            #else
            new StudioCore.MapStudioNew().Run();
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
