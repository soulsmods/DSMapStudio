using System.Collections.Generic;
using System.Diagnostics;
using StudioCore.Editor;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Silk.NET.OpenGL;
using Silk.NET.SDL;

namespace StudioCore.Graphics;

public unsafe class OpenGLCompatGraphicsContext : IGraphicsContext
{
    private OpenGLImGuiRenderer _imGuiRenderer;
    public IImguiRenderer ImguiRenderer => _imGuiRenderer;
    
    private Sdl2Window _window;
    public Sdl2Window Window => _window;
    public GraphicsDevice Device => null;

    private Sdl SDL;
    private SdlContext _context;
    private GL GL;
    private bool _windowResized = true;
    private bool _windowMoved = true;
    private bool _colorSrgb = false;

    public OpenGLCompatGraphicsContext()
    {
    }

    public void Initialize()
    {
        WindowCreateInfo windowCI = new WindowCreateInfo
        {
            X = CFG.Current.GFX_Display_X,
            Y = CFG.Current.GFX_Display_Y,
            WindowWidth = CFG.Current.GFX_Display_Width,
            WindowHeight = CFG.Current.GFX_Display_Height,
            WindowInitialState = WindowState.Maximized,
        };

        SdlProvider.InitFlags = Sdl.InitVideo;
        SDL = SdlProvider.SDL.Value;

        SDL.GLSetAttribute(GLattr.DepthSize, 24);
        SDL.GLSetAttribute(GLattr.StencilSize, 8);
        SDL.GLSetAttribute(GLattr.RedSize, 8);
        SDL.GLSetAttribute(GLattr.GreenSize, 8);
        SDL.GLSetAttribute(GLattr.BlueSize, 8);
        SDL.GLSetAttribute(GLattr.AlphaSize, 8);
        SDL.GLSetAttribute(GLattr.Doublebuffer, 1);
        SDL.GLSetAttribute(GLattr.Multisamplebuffers, 0);
        SDL.GLSetAttribute(GLattr.Multisamplesamples, 0);
        SDL.GLSetAttribute(GLattr.ContextMajorVersion, 3);
        SDL.GLSetAttribute(GLattr.ContextMinorVersion, 3);
        SDL.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Core);
        SDL.GLSetAttribute(GLattr.ContextFlags, (int)ContextFlagMask.ForwardCompatibleBit);
        SDL.GLSetAttribute(GLattr.ShareWithCurrentContext, 0);
        
        _window = VeldridStartup.CreateWindow(windowCI);
        _window.Resized += () => _windowResized = true;
        _window.Moved += (p) => _windowMoved = true;

        _context = new SdlContext(SDL, _window.SdlWindowHandle);
        _context.Create(
            (GLattr.ContextMajorVersion, 3),
            (GLattr.ContextMinorVersion, 3),
            (GLattr.ContextProfileMask, (int)GLprofile.Core),
            (GLattr.ContextFlags, (int)ContextFlagMask.ForwardCompatibleBit),
            (GLattr.ShareWithCurrentContext, 0)
        );
        _context.MakeCurrent();
        _context.SwapInterval(1);
        GL = new GL(_context);

        _imGuiRenderer = new OpenGLImGuiRenderer(GL, CFG.Current.GFX_Display_Width, CFG.Current.GFX_Display_Height,
            ColorSpaceHandling.Legacy);
    }
    
    private void RecreateWindowFramebuffers()
    {
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
            
            RecreateWindowFramebuffers();
            
            _imGuiRenderer.WindowResized(width, height);
            foreach (var editor in editors)
            {
                editor.EditorResized(_window, null);
            }
        }

        if (_windowMoved)
        {
            _windowMoved = false;
            CFG.Current.GFX_Display_X = x;
            CFG.Current.GFX_Display_Y = y;
        }
        
        GL.Viewport(0, 0, (uint)width, (uint)height);
        GL.ClearColor(0.176f, 0.176f, 0.188f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        _imGuiRenderer.Render();
        _context.SwapBuffers();
    }

    public void Dispose()
    {
        //_imGuiRenderer?.Dispose();
    }
}