// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vortice.Vulkan;

internal static class LibraryLoader
{
    static LibraryLoader()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Extension = ".dll";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            Extension = ".dylib";
        else
            Extension = ".so";
    }

    public static string Extension { get; }

    public static IntPtr LoadLocalLibrary(string libraryName)
    {
        if (!libraryName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
            libraryName += Extension;


        var osPlatform = GetOSPlatform();
        var architecture = GetArchitecture();

        var libraryPath = GetNativeAssemblyPath(osPlatform, architecture, libraryName);

        static string GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";

            throw new ArgumentException("Unsupported OS platform.");
        }

        static string GetArchitecture()
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86: return "x86";
                case Architecture.X64: return "x64";
                case Architecture.Arm: return "arm";
                case Architecture.Arm64: return "arm64";
            }

            throw new ArgumentException("Unsupported architecture.");
        }

        static string GetNativeAssemblyPath(string osPlatform, string architecture, string libraryName)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] paths = new[]
            {
                Path.Combine(assemblyLocation, libraryName),
                Path.Combine(assemblyLocation, "runtimes", osPlatform, "native", libraryName),
                Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "native", libraryName),
                Path.Combine(assemblyLocation, "native", $"{osPlatform}-{architecture}", libraryName),
            };

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return libraryName;
        }

        IntPtr handle;
#if !NET5_0_OR_GREATER
        handle = LoadPlatformLibrary(libraryPath);
#else
        handle = NativeLibrary.Load(libraryPath);
#endif

        if (handle == IntPtr.Zero)
            throw new DllNotFoundException($"Unable to load library '{libraryName}'.");

        return handle;
    }

#if !NET5_0_OR_GREATER
    private static IntPtr LoadPlatformLibrary(string libraryName)
    {
        if (string.IsNullOrEmpty(libraryName))
            throw new ArgumentNullException(nameof(libraryName));

        IntPtr handle;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            handle = Win32.LoadLibrary(libraryName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            handle = Linux.dlopen(libraryName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            handle = Mac.dlopen(libraryName);
        else
            throw new PlatformNotSupportedException($"Current platform is unknown, unable to load library '{libraryName}'.");

        return handle;
    }

    public static IntPtr GetSymbol(IntPtr library, string symbolName)
    {
        if (string.IsNullOrEmpty(symbolName))
            throw new ArgumentNullException(nameof(symbolName));

        IntPtr handle;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            handle = Win32.GetProcAddress(library, symbolName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            handle = Linux.dlsym(library, symbolName);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            handle = Mac.dlsym(library, symbolName);
        else
            throw new PlatformNotSupportedException($"Current platform is unknown, unable to load symbol '{symbolName}' from library {library}.");

        return handle;
    }


#pragma warning disable IDE1006 // Naming Styles
    private static class Mac
    {
        private const string SystemLibrary = "/usr/lib/libSystem.dylib";

        private const int RTLD_LAZY = 1;
        private const int RTLD_NOW = 2;

        public static IntPtr dlopen(string path, bool lazy = true) =>
            dlopen(path, lazy ? RTLD_LAZY : RTLD_NOW);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(SystemLibrary)]
        public static extern void dlclose(IntPtr handle);
    }

    private static class Linux
    {
        private const string SystemLibrary = "libdl.so";

        private const int RTLD_LAZY = 1;
        private const int RTLD_NOW = 2;

        public static IntPtr dlopen(string path, bool lazy = true) =>
            dlopen(path, lazy ? RTLD_LAZY : RTLD_NOW);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlopen(string path, int mode);

        [DllImport(SystemLibrary)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(SystemLibrary)]
        public static extern void dlclose(IntPtr handle);
    }

    private static class Win32
    {
        private const string SystemLibrary = "Kernel32.dll";

        [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport(SystemLibrary, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern void FreeLibrary(IntPtr hModule);
    }
#pragma warning restore IDE1006 // Naming Styles
#else
    public static IntPtr GetSymbol(IntPtr library, string name)
    {
        return NativeLibrary.GetExport(library, name);
    }
#endif
}
