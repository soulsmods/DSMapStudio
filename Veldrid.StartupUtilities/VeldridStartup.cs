using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Sdl2;

namespace Veldrid.StartupUtilities
{
    public static class VeldridStartup
    {
        public static void CreateWindowAndGraphicsDevice(
            WindowCreateInfo windowCI,
            out Sdl2Window window,
            out GraphicsDevice gd)
            => CreateWindowAndGraphicsDevice(
                windowCI,
                new GraphicsDeviceOptions(),
                out window,
                out gd);

        public static void CreateWindowAndGraphicsDevice(
            WindowCreateInfo windowCI,
            GraphicsDeviceOptions deviceOptions,
            out Sdl2Window window,
            out GraphicsDevice gd)
        {
            Sdl2Native.SDL_Init(SDLInitFlags.Video);

            window = CreateWindow(ref windowCI);
            gd = CreateGraphicsDevice(window, deviceOptions);
        }


        public static Sdl2Window CreateWindow(WindowCreateInfo windowCI) => CreateWindow(ref windowCI);

        public static Sdl2Window CreateWindow(ref WindowCreateInfo windowCI)
        {
            SDL_WindowFlags flags = SDL_WindowFlags.OpenGL | SDL_WindowFlags.Resizable
                    | GetWindowFlags(windowCI.WindowInitialState);
            if (windowCI.WindowInitialState != WindowState.Hidden)
            {
                flags |= SDL_WindowFlags.Shown;
            }
            Sdl2Window window = new Sdl2Window(
                windowCI.WindowTitle,
                windowCI.X,
                windowCI.Y,
                windowCI.WindowWidth,
                windowCI.WindowHeight,
                flags,
                false);

            return window;
        }

        private static SDL_WindowFlags GetWindowFlags(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    return 0;
                case WindowState.FullScreen:
                    return SDL_WindowFlags.Fullscreen;
                case WindowState.Maximized:
                    return SDL_WindowFlags.Maximized;
                case WindowState.Minimized:
                    return SDL_WindowFlags.Minimized;
                case WindowState.BorderlessFullScreen:
                    return SDL_WindowFlags.FullScreenDesktop;
                case WindowState.Hidden:
                    return SDL_WindowFlags.Hidden;
                default:
                    throw new VeldridException("Invalid WindowState: " + state);
            }
        }

        public static GraphicsDevice CreateGraphicsDevice(Sdl2Window window)
            => CreateGraphicsDevice(window, new GraphicsDeviceOptions());

        public static GraphicsDevice CreateGraphicsDevice(
            Sdl2Window window,
            GraphicsDeviceOptions options)
        {
            return CreateVulkanGraphicsDevice(options, window);
        }

        public static unsafe SwapchainSource GetSwapchainSource(Sdl2Window window)
        {
            IntPtr sdlHandle = window.SdlWindowHandle;
            SDL_SysWMinfo sysWmInfo;
            Sdl2Native.SDL_GetVersion(&sysWmInfo.version);
            Sdl2Native.SDL_GetWMWindowInfo(sdlHandle, &sysWmInfo);
            switch (sysWmInfo.subsystem)
            {
                case SysWMType.Windows:
                    Win32WindowInfo w32Info = Unsafe.Read<Win32WindowInfo>(&sysWmInfo.info);
                    return SwapchainSource.CreateWin32(w32Info.Sdl2Window, w32Info.hinstance);
                case SysWMType.X11:
                    X11WindowInfo x11Info = Unsafe.Read<X11WindowInfo>(&sysWmInfo.info);
                    return SwapchainSource.CreateXlib(
                        x11Info.display,
                        x11Info.Sdl2Window);
                case SysWMType.Wayland:
                    WaylandWindowInfo wlInfo = Unsafe.Read<WaylandWindowInfo>(&sysWmInfo.info);
                    return SwapchainSource.CreateWayland(wlInfo.display, wlInfo.surface);
                case SysWMType.Cocoa:
                    CocoaWindowInfo cocoaInfo = Unsafe.Read<CocoaWindowInfo>(&sysWmInfo.info);
                    IntPtr nsWindow = cocoaInfo.Window;
                    return SwapchainSource.CreateNSWindow(nsWindow);
                default:
                    throw new PlatformNotSupportedException("Cannot create a SwapchainSource for " + sysWmInfo.subsystem + ".");
            }
        }

#if !EXCLUDE_VULKAN_BACKEND
        public static unsafe GraphicsDevice CreateVulkanGraphicsDevice(GraphicsDeviceOptions options, Sdl2Window window)
            => CreateVulkanGraphicsDevice(options, window, false);
        public static unsafe GraphicsDevice CreateVulkanGraphicsDevice(
            GraphicsDeviceOptions options,
            Sdl2Window window,
            bool colorSrgb)
        {
            SwapchainDescription scDesc = new SwapchainDescription(
                GetSwapchainSource(window),
                (uint)window.Width,
                (uint)window.Height,
                options.SwapchainDepthFormat,
                options.SyncToVerticalBlank,
                colorSrgb);
            VulkanDeviceOptions vkopts = new VulkanDeviceOptions();
            vkopts.InstanceExtensions = new string[] { "VK_KHR_get_physical_device_properties2" };
            vkopts.DeviceExtensions = new string[] { "VK_KHR_maintenance3", "VK_EXT_descriptor_indexing" };
            GraphicsDevice gd = GraphicsDevice.CreateVulkan(options, scDesc, vkopts);

            return gd;
        }

        private static unsafe VkSurfaceSource GetSurfaceSource(SDL_SysWMinfo sysWmInfo)
        {
            switch (sysWmInfo.subsystem)
            {
                case SysWMType.Windows:
                    Win32WindowInfo w32Info = Unsafe.Read<Win32WindowInfo>(&sysWmInfo.info);
                    return VkSurfaceSource.CreateWin32(w32Info.hinstance, w32Info.Sdl2Window);
                default:
                    throw new PlatformNotSupportedException("Cannot create a Vulkan surface for " + sysWmInfo.subsystem + ".");
            }
        }
#endif
    }
}
