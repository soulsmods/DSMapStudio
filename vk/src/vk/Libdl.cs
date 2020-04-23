using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    internal static class Libdl
    {
        [DllImport("libdl")]
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libdl")]
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport("libdl")]
        public static extern int dlclose(IntPtr handle);

        [DllImport("libdl")]
        public static extern string dlerror();

        public const int RTLD_NOW = 0x002;
    }
}
