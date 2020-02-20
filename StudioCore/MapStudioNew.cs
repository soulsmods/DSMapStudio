using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Drawing;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using Veldrid.Sdl2;
using Veldrid;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StudioCore
{
    public class MapStudioNew
    {
        private Sdl2Window _window;
        private GraphicsDevice _gd;
        private CommandList MainWindowCommandList;
        private CommandList GuiCommandList;

        private bool _windowResized = true;
        private bool _windowMoved = true;
        private bool _colorSrgb = false;

        private static double _desiredFrameLengthSeconds = 1.0 / 20.0f;
        private static bool _limitFrameRate = true;
        //private static FrameTimeAverager _fta = new FrameTimeAverager(0.666);

        private event Action<int, int> _resizeHandled;

        private int _msaaOption = 0;
        private TextureSampleCount? _newSampleCount;

        // Window framebuffer
        private ResourceLayout TextureSamplerResourceLayout;
        private Texture MainWindowColorTexture;
        private TextureView MainWindowResolvedColorView;
        private Framebuffer MainWindowFramebuffer;
        private ResourceSet MainWindowResourceSet;

        private ImGuiRenderer ImguiRenderer;

        private MsbEditor.MsbEditorScreen MSBEditor;
        private MsbEditor.ParamEditorScreen ParamEditor;
        private MsbEditor.TextEditorScreen TextEditor;

        private bool MSBEditorActive = false;

        public static RenderDoc RenderDocManager;

        private const bool UseRenderdoc = true;

        unsafe public MapStudioNew()
        {
            CFG.AttemptLoadOrDefault();

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
                WindowTitle = "Dark Souls Map Studio",
            };
            GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, PixelFormat.R32_Float, true, ResourceBindingModel.Improved, true, true, _colorSrgb);

#if DEBUG
            gdOptions.Debug = true;
#endif

            VeldridStartup.CreateWindowAndGraphicsDevice(
               windowCI,
               gdOptions,
               //VeldridStartup.GetPlatformDefaultBackend(),
               //GraphicsBackend.Metal,
               GraphicsBackend.Vulkan,
               
               //GraphicsBackend.Direct3D11,
               //GraphicsBackend.OpenGL,
               //GraphicsBackend.OpenGLES,
               out _window,
               out _gd);
            _window.Resized += () => _windowResized = true;
            _window.Moved += (p) => _windowMoved = true;

            Sdl2Native.SDL_Init(SDLInitFlags.GameController);
            //Sdl2ControllerTracker.CreateDefault(out _controllerTracker);

            var factory = _gd.ResourceFactory;
            TextureSamplerResourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
               new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
               new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            ImguiRenderer = new ImGuiRenderer(_gd, _gd.SwapchainFramebuffer.OutputDescription, CFG.Current.GFX_Display_Width,
                CFG.Current.GFX_Display_Height, ColorSpaceHandling.Legacy);
            MainWindowCommandList = factory.CreateCommandList();
            GuiCommandList = factory.CreateCommandList();

            Scene.Renderer.Initialize(_gd);
            MSBEditor = new MsbEditor.MsbEditorScreen(_window, _gd);
            ParamEditor = new MsbEditor.ParamEditorScreen(_window, _gd);
            TextEditor = new MsbEditor.TextEditorScreen(_window, _gd);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            var fonts = ImGui.GetIO().Fonts;
            var fileJp = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\NotoSansCJKtc-Light.otf");
            var fontJp = File.ReadAllBytes(fileJp);
            var fileEn = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\RobotoMono-Light.ttf");
            var fontEn = File.ReadAllBytes(fileEn);
            var fileIcon = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\forkawesome-webfont.ttf");
            var fontIcon = File.ReadAllBytes(fileIcon);
            //fonts.AddFontFromFileTTF($@"Assets\Fonts\NotoSansCJKtc-Medium.otf", 20.0f, null, fonts.GetGlyphRangesJapanese());
            fonts.Clear();
            fixed (byte* p = fontEn)
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.GlyphMinAdvanceX = 5.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                var f = fonts.AddFontFromMemoryTTF((IntPtr)p, fontEn.Length, 14.0f, cfg, fonts.GetGlyphRangesDefault());
            }
            fixed (byte* p = fontJp)
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 7.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                var f = fonts.AddFontFromMemoryTTF((IntPtr)p, fontJp.Length, 16.0f, cfg, fonts.GetGlyphRangesJapanese());
            }
            fixed (byte* p = fontIcon)
            {
                ushort[] ranges = { ForkAwesome.IconMin, ForkAwesome.IconMax, 0 };
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 12.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                ImFontGlyphRangesBuilder b = new ImFontGlyphRangesBuilder();

                fixed (ushort* r = ranges)
                {
                    var f = fonts.AddFontFromMemoryTTF((IntPtr)p, fontIcon.Length, 16.0f, cfg, (IntPtr)r);
                }
            }
            fonts.Build();
            ImguiRenderer.RecreateFontDeviceTexture();
        }

        public void Run()
        {
            long previousFrameTicks = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_window.Exists)
            {
                bool focused = _window.Focused;
                if (!focused)
                {
                    _desiredFrameLengthSeconds = 1.0 / 20.0f;
                }
                else
                {
                    _desiredFrameLengthSeconds = 1.0 / 60.0f;
                }
                long currentFrameTicks = sw.ElapsedTicks;
                double deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

                while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds)
                {
                    currentFrameTicks = sw.ElapsedTicks;
                    deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
                    System.Threading.Thread.Sleep(focused ? 0 : 1);
                }

                previousFrameTicks = currentFrameTicks;

                InputSnapshot snapshot = null;
                Sdl2Events.ProcessEvents();
                snapshot = _window.PumpEvents();
                InputTracker.UpdateFrameInput(snapshot, _window);
                Update((float)deltaSeconds);
                if (!_window.Exists)
                {
                    break;
                }

                if (_window.Focused)
                {
                    Draw();
                }
            }

            //DestroyAllObjects();
            Resource.ResourceManager.Shutdown();
            _gd.Dispose();
            CFG.Save();

            System.Windows.Forms.Application.Exit();
        }

        private void Update(float deltaseconds)
        {
            ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot);
            //ImGui.

            var command = EditorCommandQueue.GetNextCommand();
            string[] commandsplit = null;
            if (command != null)
            {
                commandsplit = command.Split($@"/");
            }

            var vp = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(vp.Pos);
            ImGui.SetNextWindowSize(vp.Size);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            flags |= ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.MenuBar;
            flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            flags |= ImGuiWindowFlags.NoBackground;
            ImGui.Begin("DockSpace_W", flags);
            var dsid = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.NoSplit);
            ImGui.PopStyleVar(1);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            if (ImGui.Begin("Map Editor"))
            {
                ImGui.PopStyleVar(1);
                MSBEditor.OnGUI();
                ImGui.End();
                MSBEditorActive = true;
                MSBEditor.Update(deltaseconds);
            }
            else
            {
                ImGui.PopStyleVar(1);
                MSBEditorActive = false;
            }

            string[] paramcmds = null;
            if (commandsplit != null && commandsplit[0] == "param")
            {
                paramcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            if (ImGui.Begin("Param Editor"))
            {
                ParamEditor.OnGUI(paramcmds);
                ImGui.End();
            }

            string[] textcmds = null;
            if (commandsplit != null && commandsplit[0] == "text")
            {
                textcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            if (ImGui.Begin("Text Editor"))
            {
                TextEditor.OnGUI(textcmds);
                ImGui.End();
            }

            ImGui.PopStyleVar(2);
        }

        private void RecreateWindowFramebuffers(CommandList cl)
        {
            MainWindowColorTexture?.Dispose();
            MainWindowFramebuffer?.Dispose();
            MainWindowResourceSet?.Dispose();

            var factory = _gd.ResourceFactory;
            _gd.GetPixelFormatSupport(
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureType.Texture2D,
                TextureUsage.RenderTarget,
                out PixelFormatProperties properties);

            TextureDescription mainColorDesc = TextureDescription.Texture2D(
                _gd.SwapchainFramebuffer.Width,
                _gd.SwapchainFramebuffer.Height,
                1,
                1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.RenderTarget | TextureUsage.Sampled,
                TextureSampleCount.Count1);
            MainWindowColorTexture = factory.CreateTexture(ref mainColorDesc);
            MainWindowFramebuffer = factory.CreateFramebuffer(new FramebufferDescription(null, MainWindowColorTexture));
            //MainWindowResourceSet = factory.CreateResourceSet(new ResourceSetDescription(TextureSamplerResourceLayout, MainWindowResolvedColorView, _gd.PointSampler));
        }

        private void Draw()
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
                //_scene.Camera.WindowResized(width, height);
                _resizeHandled?.Invoke(width, height);
                CommandList cl = _gd.ResourceFactory.CreateCommandList();
                cl.Begin();
                //_sc.RecreateWindowSizedResources(_gd, cl);
                RecreateWindowFramebuffers(cl);
                ImguiRenderer.WindowResized(width, height);
                MSBEditor.EditorResized(_window, _gd);
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

            if (_newSampleCount != null)
            {
                //_sc.MainSceneSampleCount = _newSampleCount.Value;
                _newSampleCount = null;
                //DestroyAllObjects();
                //CreateAllObjects();
            }

            //_frameCommands.Begin();

            //CommonMaterials.FlushAll(_frameCommands);

            //_scene.RenderAllStages(_gd, _frameCommands, _sc);

            //CommandList cl2 = _gd.ResourceFactory.CreateCommandList();
            MainWindowCommandList.Begin();
            //cl2.SetFramebuffer(_gd.SwapchainFramebuffer);
            MainWindowCommandList.SetFramebuffer(_gd.SwapchainFramebuffer);
            MainWindowCommandList.ClearColorTarget(0, new RgbaFloat(0.082f, 0.082f, 0.084f, 1.0f));
            float depthClear = _gd.IsDepthRangeZeroToOne ? 1f : 0f;
            MainWindowCommandList.ClearDepthStencil(depthClear);
            MainWindowCommandList.SetFullViewport(0);
            MainWindowCommandList.End();
            _gd.SubmitCommands(MainWindowCommandList);
            if (MSBEditorActive)
            {
                MSBEditor.Draw(_gd, MainWindowCommandList);
            }
            Scene.Renderer.Frame();
            GuiCommandList.Begin();
            GuiCommandList.SetFramebuffer(_gd.SwapchainFramebuffer);
            ImguiRenderer.Render(_gd, GuiCommandList);
            GuiCommandList.End();
            _gd.SubmitCommands(GuiCommandList);
            //Scene.SceneRenderPipeline.TestUpdateView(_gd, MainWindowCommandList, TestWorldView.CameraTransform.CameraViewMatrix);

            _gd.SwapBuffers();
        }
    }
}
