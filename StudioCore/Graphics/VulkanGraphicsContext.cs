using System.Collections.Generic;
using System.Diagnostics;
using StudioCore.Editor;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Vortice.Vulkan;

namespace StudioCore.Graphics;

public class VulkanGraphicsContext : IGraphicsContext
{
    public static RenderDoc RenderDocManager;
    private const bool UseRenderdoc = false;
    
    private VulkanImGuiRenderer _imGuiRenderer;
    public IImguiRenderer ImguiRenderer => _imGuiRenderer;
    
    private Sdl2Window _window;
    public Sdl2Window Window => _window;
    private GraphicsDevice _gd;
    public GraphicsDevice Device => _gd;
    private CommandList MainWindowCommandList;
    
    // Window framebuffer
    private Texture MainWindowColorTexture;
    private TextureView MainWindowResolvedColorView;
    private Framebuffer MainWindowFramebuffer;
    private ResourceSet MainWindowResourceSet;
    
    private bool _windowResized = true;
    private bool _windowMoved = true;
    private bool _colorSrgb = false;

    public VulkanGraphicsContext(string title)
    {
        if (UseRenderdoc)
        {
            RenderDoc.Load(out RenderDocManager);
            RenderDocManager.OverlayEnabled = false;
        }

        WindowCreateInfo windowCI = new WindowCreateInfo
        {
            X = CFG.Current.GFX_Display_X,
            Y = CFG.Current.GFX_Display_Y,
            WindowWidth = CFG.Current.GFX_Display_Width,
            WindowHeight = CFG.Current.GFX_Display_Height,
            WindowInitialState = WindowState.Maximized,
            WindowTitle = $"{title}",
        };
        GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, VkFormat.D32Sfloat, true, true, true, _colorSrgb);

#if DEBUG
        gdOptions.Debug = true;
#endif

        VeldridStartup.CreateWindowAndGraphicsDevice(
            windowCI,
            gdOptions,
            out _window,
            out _gd);
        _window.Resized += () => _windowResized = true;
        _window.Moved += (p) => _windowMoved = true;

        Sdl2Native.SDL_Init(SDLInitFlags.GameController);

        Scene.Renderer.Initialize(_gd);
        
        var factory = _gd.ResourceFactory;
        _imGuiRenderer = new VulkanImGuiRenderer(_gd, _gd.SwapchainFramebuffer.OutputDescription, CFG.Current.GFX_Display_Width,
            CFG.Current.GFX_Display_Height, ColorSpaceHandling.Legacy);
        MainWindowCommandList = factory.CreateCommandList();
    }
    
    private void RecreateWindowFramebuffers(CommandList cl)
    {
        MainWindowColorTexture?.Dispose();
        MainWindowFramebuffer?.Dispose();
        MainWindowResourceSet?.Dispose();

        var factory = _gd.ResourceFactory;
        _gd.GetPixelFormatSupport(
            VkFormat.R8G8B8A8Unorm,
            VkImageType.Image2D,
            VkImageUsageFlags.ColorAttachment,
            VkImageTiling.Optimal,
            out PixelFormatProperties properties);

        TextureDescription mainColorDesc = TextureDescription.Texture2D(
            _gd.SwapchainFramebuffer.Width,
            _gd.SwapchainFramebuffer.Height,
            1,
            1,
            VkFormat.R8G8B8A8Unorm,
            VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.Sampled,
            VkImageCreateFlags.None,
            VkImageTiling.Optimal,
            VkSampleCountFlags.Count1);
        MainWindowColorTexture = factory.CreateTexture(ref mainColorDesc);
        MainWindowFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(null, MainWindowColorTexture));
    }

    public void Draw(List<EditorScreen> editors, EditorScreen focusedEditor)
    {
        Debug.Assert(_window.Exists);
        int width = _window.Width;
        int height = _window.Height;
        int x = _window.X;
        int y = _window.Y;

        if (_windowResized)
        {
            _windowResized = false;

            CFG.Current.GFX_Display_Width = width;
            CFG.Current.GFX_Display_Height = height;

            _gd.ResizeMainWindow((uint)width, (uint)height);
            CommandList cl = _gd.ResourceFactory.CreateCommandList();
            cl.Begin();
            RecreateWindowFramebuffers(cl);
            _imGuiRenderer.WindowResized(width, height);
            foreach (var editor in editors)
            {
                editor.EditorResized(_window, _gd);
            }

            cl.End();
            _gd.SubmitCommands(cl);
            cl.Dispose();
        }

        if (_windowMoved)
        {
            _windowMoved = false;
            CFG.Current.GFX_Display_X = x;
            CFG.Current.GFX_Display_Y = y;
        }

        MainWindowCommandList.Begin();
        MainWindowCommandList.SetFramebuffer(_gd.SwapchainFramebuffer);
        MainWindowCommandList.ClearColorTarget(0, new RgbaFloat(0.176f, 0.176f, 0.188f, 1.0f));
        MainWindowCommandList.ClearDepthStencil(0.0f);
        MainWindowCommandList.SetFullViewport(0);

        focusedEditor.Draw(_gd, MainWindowCommandList);
        var fence = Scene.Renderer.Frame(MainWindowCommandList, false);
        MainWindowCommandList.SetFullViewport(0);
        MainWindowCommandList.SetFullScissorRects();
        _imGuiRenderer.Render(_gd, MainWindowCommandList);
        MainWindowCommandList.End();
        _gd.SubmitCommands(MainWindowCommandList, fence);
        Scene.Renderer.SubmitPostDrawCommandLists();

        _gd.SwapBuffers();
    }

    public void Dispose()
    {
        _imGuiRenderer?.Dispose();
        MainWindowCommandList?.Dispose();
        MainWindowColorTexture?.Dispose();
        MainWindowResolvedColorView?.Dispose();
        MainWindowFramebuffer?.Dispose();
        MainWindowResourceSet?.Dispose();
        _gd?.Dispose();
    }
}