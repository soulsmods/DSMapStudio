using ImGuiNET;
using StudioCore.Editor;
using StudioCore.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading.Tasks;
using SoapstoneLib;
using StudioCore.ParamEditor;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using System.Windows.Forms;

namespace StudioCore
{
    public class MapStudioNew
    {
        private static string _version = "1.04.1";
        private static string _programTitle = $"Dark Souls Map Studio version {_version}";

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

        private bool _msbEditorFocused = false;
        private MsbEditor.MsbEditorScreen _msbEditor;
        private bool _modelEditorFocused = false;
        private MsbEditor.ModelEditorScreen _modelEditor;
        private bool _paramEditorFocused = false;
        private ParamEditor.ParamEditorScreen _paramEditor;
        private bool _textEditorFocused = false;
        private TextEditor.TextEditorScreen _textEditor;

        private SoapstoneService _soapstoneService;

        public static RenderDoc RenderDocManager;

        private const bool UseRenderdoc = false;

        private AssetLocator _assetLocator;
        private Editor.ProjectSettings _projectSettings = null;

        private NewProjectOptions _newProjectOptions = new NewProjectOptions();

        private static bool _firstframe = true;
        public static bool FirstFrame = true;

        public MapStudioNew()
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
                WindowTitle = $"{_programTitle}",
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

            Scene.Renderer.Initialize(_gd);

            ImguiRenderer = new ImGuiRenderer(_gd, _gd.SwapchainFramebuffer.OutputDescription, CFG.Current.GFX_Display_Width,
                CFG.Current.GFX_Display_Height, ColorSpaceHandling.Legacy);
            MainWindowCommandList = factory.CreateCommandList();
            GuiCommandList = factory.CreateCommandList();

            _assetLocator = new AssetLocator();
            _msbEditor = new MsbEditor.MsbEditorScreen(_window, _gd, _assetLocator);
            _modelEditor = new MsbEditor.ModelEditorScreen(_window, _gd, _assetLocator);
            _paramEditor = new ParamEditor.ParamEditorScreen(_window, _gd);
            _textEditor = new TextEditor.TextEditorScreen(_window, _gd);
            _soapstoneService = new SoapstoneService(_version, _assetLocator, _msbEditor);

            Editor.AliasBank.SetAssetLocator(_assetLocator);
            ParamEditor.ParamBank.PrimaryBank.SetAssetLocator(_assetLocator);
            ParamEditor.ParamBank.VanillaBank.SetAssetLocator(_assetLocator);
            TextEditor.FMGBank.SetAssetLocator(_assetLocator);
            MsbEditor.MtdBank.LoadMtds(_assetLocator);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            SetupFonts();
            ImguiRenderer.OnSetupDone();

            var style = ImGui.GetStyle();
            style.TabBorderSize = 0;

            if (CFG.Current.LastProjectFile != null && CFG.Current.LastProjectFile != "")
            {
                if (File.Exists(CFG.Current.LastProjectFile))
                {
                    var project = Editor.ProjectSettings.Deserialize(CFG.Current.LastProjectFile);
                    AttemptLoadProject(project, CFG.Current.LastProjectFile, false);
                }
            }
        }

        /// <summary>
        /// Characters to load that FromSoft use, but aren't included in the ImGui Japanese glyph range.
        /// </summary>
        private char[] SpecialCharsJP = { '鉤' };

        private unsafe void SetupFonts()
        {
            var fonts = ImGui.GetIO().Fonts;
            var fileEn = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\RobotoMono-Light.ttf");
            var fontEn = File.ReadAllBytes(fileEn);
            var fileOther = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\NotoSansCJKtc-Light.otf");
            var fontOther = File.ReadAllBytes(fileOther);
            var fileIcon = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\forkawesome-webfont.ttf");
            var fontIcon = File.ReadAllBytes(fileIcon);
            //fonts.AddFontFromFileTTF($@"Assets\Fonts\NotoSansCJKtc-Medium.otf", 20.0f, null, fonts.GetGlyphRangesJapanese());
            fonts.Clear();

            var scale = CFG.Current.FontSizeScale;

            fixed (byte* p = fontEn)
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.GlyphMinAdvanceX = 5.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                fonts.AddFontFromMemoryTTF((IntPtr)p, fontEn.Length, 14.0f * scale, cfg, fonts.GetGlyphRangesDefault());
            }
            fixed (byte* p = fontOther)
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 7.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;

                var glyphJP = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
                glyphJP.AddRanges(fonts.GetGlyphRangesJapanese());
                Array.ForEach(SpecialCharsJP, c => glyphJP.AddChar(c));
                glyphJP.BuildRanges(out ImVector glyphRangeJP);
                fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 16.0f * scale, cfg, glyphRangeJP.Data);
                glyphJP.Destroy();

                if (CFG.Current.FontChinese)
                    fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 16.0f * scale, cfg, fonts.GetGlyphRangesChineseFull());
                if (CFG.Current.FontKorean)
                    fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 16.0f * scale, cfg, fonts.GetGlyphRangesKorean());
                if (CFG.Current.FontThai)
                    fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 16.0f * scale, cfg, fonts.GetGlyphRangesThai());
                if (CFG.Current.FontVietnamese)
                    fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 16.0f * scale, cfg, fonts.GetGlyphRangesVietnamese());
                cfg.GlyphMinAdvanceX = 5.0f;
                if (CFG.Current.FontCyrillic)
                    fonts.AddFontFromMemoryTTF((IntPtr)p, fontOther.Length, 18.0f * scale, cfg, fonts.GetGlyphRangesCyrillic());
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
                    var f = fonts.AddFontFromMemoryTTF((IntPtr)p, fontIcon.Length, 16.0f * scale, cfg, (IntPtr)r);
                }
            }
            fonts.Build();
            ImguiRenderer.RecreateFontDeviceTexture();
        }

        public void SetupCSharpDefaults()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public void ManageImGuiConfigBackups()
        {
            if (!File.Exists("imgui.ini"))
            {
                if (File.Exists("imgui.ini.backup"))
                    File.Copy("imgui.ini.backup", "imgui.ini");
            }
            else if (!File.Exists("imgui.ini.backup"))
            {
                if (File.Exists("imgui.ini"))
                    File.Copy("imgui.ini", "imgui.ini.backup");
            }
        }

        public void Run()
        {
            SetupCSharpDefaults();
            ManageImGuiConfigBackups();

            if (CFG.Current.EnableSoapstone)
            {
                SoapstoneServer.RunAsync(KnownServer.DSMapStudio, _soapstoneService);
            }
            /*Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    GC.Collect();
                }
            });*/

            // Flush geometry megabuffers for editor geometry
            //Renderer.GeometryBufferAllocator.FlushStaging();

            long previousFrameTicks = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Tracy.Startup();
            while (_window.Exists)
            {
                // Make sure any awaited UI thread work has a chance to complete
                //await Task.Yield();

                Tracy.TracyCFrameMark();

                // Limit frame rate when window isn't focused unless we are profiling
                bool focused = Tracy.EnableTracy ? true : _window.Focused;
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

                var ctx = Tracy.TracyCZoneNC(1, "Sleep", 0xFF0000FF);
                while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds)
                {
                    currentFrameTicks = sw.ElapsedTicks;
                    deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
                    System.Threading.Thread.Sleep(focused ? 0 : 1);
                }
                Tracy.TracyCZoneEnd(ctx);

                previousFrameTicks = currentFrameTicks;

                ctx = Tracy.TracyCZoneNC(1, "Update", 0xFF00FF00);
                InputSnapshot snapshot = null;
                Sdl2Events.ProcessEvents();
                snapshot = _window.PumpEvents();
                InputTracker.UpdateFrameInput(snapshot, _window);
                Update((float)deltaSeconds);
                Tracy.TracyCZoneEnd(ctx);
                if (!_window.Exists)
                {
                    break;
                }

                if (true)//_window.Focused)
                {
                    ctx = Tracy.TracyCZoneNC(1, "Draw", 0xFFFF0000);
                    Draw();
                    Tracy.TracyCZoneEnd(ctx);
                }
                else
                {
                    // Flush the background queues
                    Renderer.Frame(null, true);
                }
            }

            //DestroyAllObjects();
            Tracy.Shutdown();
            Resource.ResourceManager.Shutdown();
            _gd.Dispose();
            CFG.Save();

            System.Windows.Forms.Application.Exit();
        }

        // Try to shutdown things gracefully on a crash
        public void CrashShutdown()
        {
            Tracy.Shutdown();
            Resource.ResourceManager.Shutdown();
            _gd.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        private string CrashLogPath = $"{Directory.GetCurrentDirectory()}\\Crash Logs";
        public void ExportCrashLog(List<string> exceptionInfo)
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

        private void ChangeProjectSettings(Editor.ProjectSettings newsettings, string moddir, NewProjectOptions options)
        {
            _projectSettings = newsettings;
            _assetLocator.SetFromProjectSettings(newsettings, moddir);

            Editor.AliasBank.ReloadAliases();
            ParamEditor.ParamBank.ReloadParams(newsettings, options);
            MsbEditor.MtdBank.ReloadMtds();
            _msbEditor.ReloadUniverse();
            _modelEditor.ReloadAssetBrowser();
            
            //Resources loaded here should be moved to databanks
            _msbEditor.OnProjectChanged(_projectSettings);
            _modelEditor.OnProjectChanged(_projectSettings);
            _textEditor.OnProjectChanged(_projectSettings);
            _paramEditor.OnProjectChanged(_projectSettings);
        }

        public void ApplyStyle()
        {
            // Colors
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            //ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(0.106f, 0.106f, 0.110f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.200f, 0.200f, 0.216f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.200f, 0.200f, 0.216f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.MenuBarBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarBg, new Vector4(0.243f, 0.243f, 0.249f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab, new Vector4(0.408f, 0.408f, 0.408f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered, new Vector4(0.635f, 0.635f, 0.635f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabActive, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrab, new Vector4(0.635f, 0.635f, 0.635f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.200f, 0.600f, 1.000f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.000f, 0.478f, 0.800f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.161f, 0.550f, 0.939f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Tab, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.110f, 0.592f, 0.918f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabActive, new Vector4(0.200f, 0.600f, 1.000f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabUnfocused, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.TabUnfocusedActive, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));

            // Sizes
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 16.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(100f,100f));
        }

        public void UnapplyStyle()
        {
            ImGui.PopStyleColor(27);
            ImGui.PopStyleVar(5);
        }

        private void DumpFlverLayouts()
        {
            var browseDlg = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Text file (*.txt) |*.TXT",
                ValidateNames = true,
            };

            if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var file = new StreamWriter(browseDlg.FileName))
                {
                    foreach (var mat in Resource.FlverResource.MaterialLayouts)
                    {
                        file.WriteLine(mat.Key + ":");
                        foreach (var member in mat.Value)
                        {
                            file.WriteLine($@"{member.Index}: {member.Type.ToString()}: {member.Semantic.ToString()}");
                        }
                        file.WriteLine();
                    }
                }
            }
        }

        private bool AttemptLoadProject(Editor.ProjectSettings settings, string filename, bool updateRecents=true, NewProjectOptions options=null)
        {
            bool success = true;

            // Check if game exe exists
            if (!Directory.Exists(settings.GameRoot))
            {
                success = false;
                System.Windows.Forms.MessageBox.Show($@"Could not find game data directory for {settings.GameType}. Please select the game executable.", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.None);

                var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                {
                    Filter = AssetLocator.GameExecutatbleFilter,
                    ValidateNames = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    //ShowReadOnly = true,
                };

                var gametype = GameType.Undefined;
                while (gametype != settings.GameType)
                {
                    if (rbrowseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        settings.GameRoot = rbrowseDlg.FileName;
                        gametype = _assetLocator.GetGameTypeForExePath(settings.GameRoot);
                        if (gametype != settings.GameType)
                        {
                            System.Windows.Forms.MessageBox.Show($@"Selected executable was not for {settings.GameType}. Please select the correct game executable.", "Error",
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.None);
                        }
                        else
                        {
                            success = true;
                            settings.GameRoot = Path.GetDirectoryName(settings.GameRoot);
                            if (settings.GameType == GameType.Bloodborne)
                            {
                                settings.GameRoot = settings.GameRoot + @"\dvdroot_ps4";
                            }
                            settings.Serialize(filename);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (success)
            {
                if (!_assetLocator.CheckFilesExpanded(settings.GameRoot, settings.GameType))
                {
                    System.Windows.Forms.MessageBox.Show($@"The files for {settings.GameType} do not appear to be unpacked. Please use UDSFM for DS1:PTDE and UXM for the rest of the games to unpack the files.", "Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.None);
                    return false;
                }
                if ((settings.GameType == GameType.Sekiro || settings.GameType == GameType.EldenRing) && !File.Exists(Path.Join(Path.GetFullPath("."), "oo2core_6_win64.dll")))
                {
                    //Technically we're not checking it exists, but the same can be said for many things we assume from CheckFilesExpanded
                    File.Copy(Path.Join(settings.GameRoot, "oo2core_6_win64.dll"), Path.Join(Path.GetFullPath("."), "oo2core_6_win64.dll"));
                }
                _projectSettings = settings;
                ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(filename), options);
                CFG.Current.LastProjectFile = filename;
                _window.Title = $"{_programTitle}  -  {_projectSettings.ProjectName}";

                if (updateRecents)
                {
                    var recent = new CFG.RecentProject();
                    recent.Name = _projectSettings.ProjectName;
                    recent.GameType = _projectSettings.GameType;
                    recent.ProjectFile = filename;
                    CFG.Current.RecentProjects.Insert(0, recent);
                    if (CFG.Current.RecentProjects.Count > CFG.MAX_RECENT_PROJECTS)
                    {
                        CFG.Current.RecentProjects.RemoveAt(CFG.Current.RecentProjects.Count - 1);
                    }
                }
            }
            return success;
        }

        //Unhappy with this being here
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _user32_ShowWindow(IntPtr hWnd, int nCmdShow);

        // Saves modded files to a recovery directory in the mod folder on crash
        public void AttemptSaveOnCrash()
        {
            bool success = _assetLocator.CreateRecoveryProject();
            if (success)
            {
                _msbEditor.SaveAll();
                _modelEditor.SaveAll();
                _paramEditor.SaveAll();
                _textEditor.SaveAll();
                System.Windows.Forms.MessageBox.Show(
                    $@"Your project was successfully saved to {_assetLocator.GameModDirectory} for manual recovery. " +
                    "You must manually replace your projects with these recovery files should you wish to restore them. " +
                    "Given the program has crashed, these files may be corrupt and you should backup your last good saved " +
                    "files before attempting to use these.",
                    "Saved recovery",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        private void SaveFocusedEditor()
        {
            if (_projectSettings != null && _projectSettings.ProjectName != null)
            {
                _projectSettings.Serialize(CFG.Current.LastProjectFile); //Danger zone assuming on lastProjectFile
                if (_msbEditorFocused)
                {
                    _msbEditor.Save();
                }
                if (_modelEditorFocused)
                {
                    _modelEditor.Save();
                }
                if (_paramEditorFocused)
                {
                    _paramEditor.Save();
                }
                if (_textEditorFocused)
                {
                    _textEditor.Save();
                }
            }
        }

        private KeyBind _currentKeyBind;
        private void Update(float deltaseconds)
        {
            var ctx = Tracy.TracyCZoneN(1, "Imgui");
            ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot);
            Tracy.TracyCZoneEnd(ctx);
            List<string> tasks = Editor.TaskManager.GetLiveThreads();
            Editor.TaskManager.ThrowTaskExceptions();

            string[] commandsplit = EditorCommandQueue.GetNextCommand();
            if (commandsplit != null && commandsplit[0] == "windowFocus")
            {
                //this is a hack, cannot grab focus except for when un-minimising
                _user32_ShowWindow(_window.Handle, 6);
                _user32_ShowWindow(_window.Handle, 9);
            }

            ctx = Tracy.TracyCZoneN(1, "Style");
            //ImGui.BeginFrame(); // Imguizmo begin frame
            ApplyStyle();
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
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            if (ImGui.Begin("DockSpace_W", flags))
            {
                //Console.WriteLine("hi");
            }
            var dsid = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.NoSplit);
            ImGui.PopStyleVar(1);
            ImGui.End();
            ImGui.PopStyleColor(1);
            Tracy.TracyCZoneEnd(ctx);

            ctx = Tracy.TracyCZoneN(1, "Menu");
            bool newProject = false;
            bool keyBindGUI = false;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0.0f);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Enable Texturing (alpha)", "", CFG.Current.EnableTexturing))
                    {
                        CFG.Current.EnableTexturing = !CFG.Current.EnableTexturing;
                    }
                    if (ImGui.MenuItem("New Project", "", false, Editor.TaskManager.GetLiveThreads().Count == 0))
                    {
                        newProject = true;
                    }
                    if (ImGui.MenuItem("Open Project", "", false, Editor.TaskManager.GetLiveThreads().Count == 0))
                    {
                        var browseDlg = new System.Windows.Forms.OpenFileDialog()
                        {
                            Filter = AssetLocator.JsonFilter,
                            ValidateNames = true,
                            CheckFileExists = true,
                            CheckPathExists = true,
                        };

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var settings = Editor.ProjectSettings.Deserialize(browseDlg.FileName);
                            AttemptLoadProject(settings, browseDlg.FileName);
                        }
                    }
                    if (ImGui.BeginMenu("Recent Projects", Editor.TaskManager.GetLiveThreads().Count == 0 && CFG.Current.RecentProjects.Count > 0))
                    {
                        CFG.RecentProject recent = null;
                        foreach (var p in CFG.Current.RecentProjects)
                        {
                            if (ImGui.MenuItem($@"{p.GameType.ToString()}:{p.Name}"))
                            {
                                if (File.Exists(p.ProjectFile))
                                {
                                    var settings = Editor.ProjectSettings.Deserialize(p.ProjectFile);
                                    if (AttemptLoadProject(settings, p.ProjectFile, false))
                                    {
                                        recent = p;
                                    }
                                }
                            }
                        }
                        if (recent != null)
                        {
                            CFG.Current.RecentProjects.Remove(recent);
                            CFG.Current.RecentProjects.Insert(0, recent);
                            CFG.Current.LastProjectFile = recent.ProjectFile;
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Open in Explorer", Editor.TaskManager.GetLiveThreads().Count == 0 && CFG.Current.RecentProjects.Count > 0))
                    {
                        if (ImGui.MenuItem("Open Project Folder", "", false, Editor.TaskManager.GetLiveThreads().Count == 0))
                        {
                            string projectPath = _assetLocator.GameModDirectory;
                            Process.Start("explorer.exe", projectPath);
                        }
                        if (ImGui.MenuItem("Open Game Folder", "", false, Editor.TaskManager.GetLiveThreads().Count == 0))
                        {
                            var gamePath = _assetLocator.GameRootDirectory;
                            Process.Start("explorer.exe", gamePath);
                        }
                        if (ImGui.MenuItem("Open Config Folder", "", false, Editor.TaskManager.GetLiveThreads().Count == 0))
                        {
                            var configPath = CFG.GetConfigFolderPath();
                            Process.Start("explorer.exe", configPath);
                        }
                        ImGui.EndMenu();
                    }

                    string focusType = "";
                    if (_msbEditorFocused)
                    {
                        focusType = "Maps";
                    }
                    else if (_modelEditorFocused)
                    {
                        focusType = "Models";
                    }
                    else if (_paramEditorFocused)
                    {
                        focusType = "Params";
                    }
                    else if (_textEditorFocused)
                    {
                        focusType = "Text";
                    }

                    if (ImGui.MenuItem($"Save {focusType}", KeyBindings.Current.Core_SaveCurrentEditor.HintText))
                    {
                        SaveFocusedEditor();
                    }
                    if (ImGui.MenuItem("Save All", KeyBindings.Current.Core_SaveAllEditors.HintText))
                    {
                        _msbEditor.SaveAll();
                        _modelEditor.SaveAll();
                        _paramEditor.SaveAll();
                        _textEditor.SaveAll();
                    }
                    if (Resource.FlverResource.CaptureMaterialLayouts && ImGui.MenuItem("Dump Flver Layouts (Debug)", ""))
                    {
                        DumpFlverLayouts();
                    }
                    ImGui.EndMenu();
                }
                if (_msbEditorFocused)
                {
                    _msbEditor.DrawEditorMenu();
                }
                else if (_modelEditorFocused)
                {
                    _modelEditor.DrawEditorMenu();
                }
                else if (_paramEditorFocused)
                {
                    _paramEditor.DrawEditorMenu();
                }
                else if (_textEditorFocused)
                {
                    _textEditor.DrawEditorMenu();
                }
                if (ImGui.BeginMenu("Settings"))
                {
                    if (_projectSettings == null || _projectSettings.ProjectName == null)
                    {
                        ImGui.MenuItem("Project Settings: (No project)", false);
                    }
                    else
                    {
                        if (ImGui.BeginMenu($@"Project Settings: {_projectSettings.ProjectName}", Editor.TaskManager.GetLiveThreads().Count == 0))
                        {
                            bool useLoose = _projectSettings.UseLooseParams;
                            if ((_projectSettings.GameType is GameType.DarkSoulsIISOTFS or GameType.DarkSoulsIII)
                                && ImGui.Checkbox("Use Loose Params", ref useLoose))
                            {
                                _projectSettings.UseLooseParams = useLoose;
                            }

                            bool usepartial = _projectSettings.PartialParams;
                            if ((FeatureFlags.EnablePartialParam || usepartial) &&
                                _projectSettings.GameType == GameType.EldenRing && ImGui.Checkbox("Partial Params", ref usepartial))
                            {
                                _projectSettings.PartialParams = usepartial;
                            }
                            ImGui.EndMenu();
                        }
                    }

                    if (ImGui.Selectable("Key Bindings"))
                    {
                        keyBindGUI = true;
                    }
                    if (ImGui.BeginMenu("Fonts"))
                    {
                        ImGui.Text("Please restart program for font changes to take effect.");
                        ImGui.Separator();

                        if (ImGui.SliderFloat("Font Scale", ref CFG.Current.FontSizeScale, 0.5f, 4.0f))
                        {
                            CFG.Current.FontSizeScale = (float)Math.Round(CFG.Current.FontSizeScale, 1);
                        }
                        if (ImGui.BeginMenu("Additional Language Fonts"))
                        {
                            ImGui.Text("Additional fonts take more VRAM and increase startup time.");
                            ImGui.Separator();
                            if (ImGui.MenuItem("Chinese", "", CFG.Current.FontChinese))
                            {
                                CFG.Current.FontChinese = !CFG.Current.FontChinese;
                            }
                            if (ImGui.MenuItem("Korean", "", CFG.Current.FontKorean))
                            {
                                CFG.Current.FontKorean = !CFG.Current.FontKorean;
                            }
                            if (ImGui.MenuItem("Thai", "", CFG.Current.FontThai))
                            {
                                CFG.Current.FontThai = !CFG.Current.FontThai;
                            }
                            if (ImGui.MenuItem("Vietnamese", "", CFG.Current.FontVietnamese))
                            {
                                CFG.Current.FontVietnamese = !CFG.Current.FontVietnamese;
                            }
                            if (ImGui.MenuItem("Cyrillic", "", CFG.Current.FontCyrillic))
                            {
                                CFG.Current.FontCyrillic = !CFG.Current.FontCyrillic;
                            }
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Map Editor"))
                    {
                        ImGui.Checkbox("Exclude loaded maps from search filter", ref CFG.Current.Map_AlwaysListLoadedMaps);
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Viewport Settings"))
                    {
                        if (ImGui.Button("Reset"))
                        {
                            CFG.Current.GFX_Camera_FOV = CFG.Default.GFX_Camera_FOV;

                            _msbEditor.Viewport.FarClip = CFG.Default.GFX_RenderDistance_Max;
                            CFG.Current.GFX_RenderDistance_Max = _msbEditor.Viewport.FarClip;

                            _msbEditor.Viewport._worldView.CameraMoveSpeed_Slow = CFG.Default.GFX_Camera_MoveSpeed_Slow;
                            CFG.Current.GFX_Camera_MoveSpeed_Slow = _msbEditor.Viewport._worldView.CameraMoveSpeed_Slow;

                            _msbEditor.Viewport._worldView.CameraMoveSpeed_Normal = CFG.Default.GFX_Camera_MoveSpeed_Normal;
                            CFG.Current.GFX_Camera_MoveSpeed_Normal = _msbEditor.Viewport._worldView.CameraMoveSpeed_Normal;

                            _msbEditor.Viewport._worldView.CameraMoveSpeed_Fast = CFG.Default.GFX_Camera_MoveSpeed_Fast;
                            CFG.Current.GFX_Camera_MoveSpeed_Fast = _msbEditor.Viewport._worldView.CameraMoveSpeed_Fast;
                        }
                        float cam_fov = CFG.Current.GFX_Camera_FOV;
                        if (ImGui.SliderFloat("Camera FOV", ref cam_fov, 40.0f, 140.0f))
                        {
                            CFG.Current.GFX_Camera_FOV = cam_fov;
                        }
                        if (ImGui.SliderFloat("Map Max Render Distance", ref _msbEditor.Viewport.FarClip, 10.0f, 500000.0f))
                        {
                            CFG.Current.GFX_RenderDistance_Max = _msbEditor.Viewport.FarClip;
                        }
                        if (ImGui.SliderFloat("Map Camera Speed (Slow)", ref _msbEditor.Viewport._worldView.CameraMoveSpeed_Slow, 0.1f, 999.0f))
                        {
                            CFG.Current.GFX_Camera_MoveSpeed_Slow = _msbEditor.Viewport._worldView.CameraMoveSpeed_Slow;
                        }
                        if (ImGui.SliderFloat("Map Camera Speed (Normal)", ref _msbEditor.Viewport._worldView.CameraMoveSpeed_Normal, 0.1f, 999.0f))
                        {
                            CFG.Current.GFX_Camera_MoveSpeed_Normal = _msbEditor.Viewport._worldView.CameraMoveSpeed_Normal;
                        }
                        if (ImGui.SliderFloat("Map Camera Speed (Fast)", ref _msbEditor.Viewport._worldView.CameraMoveSpeed_Fast, 0.1f, 999.0f))
                        {
                            CFG.Current.GFX_Camera_MoveSpeed_Fast = _msbEditor.Viewport._worldView.CameraMoveSpeed_Fast;
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Soapstone Server"))
                    {
                        string running = SoapstoneServer.GetRunningPort() is int port ? $"running on port {port}" : "not running";
                        ImGui.Text($"The server is {running}.\nIt is not accessible over the network, only to other programs on this computer.\nPlease restart the program for changes to take effect.");
                        ImGui.Separator();
                        if (ImGui.MenuItem("Enable Cross-Editor Features", "", CFG.Current.EnableSoapstone))
                        {
                            CFG.Current.EnableSoapstone = !CFG.Current.EnableSoapstone;
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem("Enable Texturing (alpha)", "", CFG.Current.EnableTexturing))
                    {
                        CFG.Current.EnableTexturing = !CFG.Current.EnableTexturing;
                    }
                    if (ImGui.MenuItem("Show Original FMG Names", "", CFG.Current.FMG_ShowOriginalNames))
                    {
                        CFG.Current.FMG_ShowOriginalNames = !CFG.Current.FMG_ShowOriginalNames;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.BeginMenu("About"))
                    {
                        ImGui.Text("Original Author:\n" +
                                   "Katalash\n\n" +
                                   "Core Development Team:\n" +
                                   "Katalash\n" +
                                   "Philiquaz\n" +
                                   "King bore haha (george)\n\n" +
                                   "Additional Contributors:\n" +
                                   "Thefifthmatt\n" +
                                   "Shadowth117\n" +
                                   "Nordgaren\n\n" +
                                   "Special Thanks:\n" +
                                   "TKGP\n" +
                                   "Meowmaritus\n" +
                                   "Vawser\n" +
                                   "Radai\n" +
                                   "Moonlight Ruin");
                        ImGui.EndMenu();
                    }
                    
                    if (ImGui.BeginMenu("How to use"))
                    {
                        ImGui.Text("Usage of many features is assisted through the symbol (?).\nIn many cases, right clicking items will provide further information and options.");
                        ImGui.EndMenu();
                    }
                    
                    if (ImGui.BeginMenu("Camera Controls"))
                    {
                        ImGui.Text("Holding click on the viewport will enable camera controls.\nUse WASD to navigate.\nUse right click to rotate the camera.\nHold Shift to temporarily speed up and Ctrl to temporarily slow down.\nScroll the mouse wheel to adjust overall speed.");
                        ImGui.EndMenu();
                    }

                    if (ImGui.MenuItem("Modding Wiki"))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "http://soulsmodding.wikidot.com/",
                            UseShellExecute = true
                        });
                    }
                    
                    if (ImGui.MenuItem("Map ID Reference"))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "http://soulsmodding.wikidot.com/reference:map-list",
                            UseShellExecute = true
                        });
                    }
                    
                    if (ImGui.MenuItem("DSMapStudio Discord"))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://discord.gg/CKDBCUFhB3",
                            UseShellExecute = true
                        });
                    }
                    
                    if (ImGui.MenuItem("FromSoftware Modding Discord"))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://discord.gg/mT2JJjx",
                            UseShellExecute = true
                        });
                    }
                    /*
                    if (ImGui.BeginMenu("Edits aren't sticking!"))
                    {
                        ImGui.Text("The mechanism that is used to detect if a field has been changed can stop existing before registering a change.\nThis occurs when switching param, row or using tab between fields.\nI hope to have this fixed soon, however it is a complicated issue.\nTo ensure a change sticks, simply click off the field you are editing.");
                        ImGui.EndMenu();
                    }
                    */
                    ImGui.EndMenu();
                }
                if (FeatureFlags.TestMenu)
                {
                    if (ImGui.BeginMenu("Tests"))
                    {
                        if (ImGui.MenuItem("Crash me (will actually crash)"))
                        {
                            var badArray = new int[2];
                            var crash = badArray[5];
                        }
                        if (ImGui.MenuItem("MSBE read/write test"))
                        {
                            Tests.MSBReadWrite.Run(_assetLocator);
                        }
                        if (ImGui.MenuItem("BTL read/write test"))
                        {
                            Tests.BTLReadWrite.Run(_assetLocator);
                        }
                        ImGui.EndMenu();
                    }

                }
                if (TaskManager.GetLiveThreads().Count > 0 && ImGui.BeginMenu("Tasks"))
                {
                    foreach (String task in TaskManager.GetLiveThreads()) {
                        ImGui.Text(task);
                    }
                    ImGui.EndMenu();
                }
                if (TaskManager.warningList.Count > 0)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0f, 0f, 1.0f));
                    if (ImGui.BeginMenu("!! WARNINGS !!"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        ImGui.Text("Click warnings to remove them from list");
                        if (ImGui.Button("Remove All Warnings"))
                            TaskManager.warningList.Clear();

                        ImGui.Separator();
                        foreach (var task in TaskManager.warningList)
                        {
                            if (ImGui.Selectable(task.Value, false, ImGuiSelectableFlags.DontClosePopups))
                            {
                                TaskManager.warningList.TryRemove(task);
                            }
                        }
                        ImGui.PopStyleColor();
                        ImGui.EndMenu();
                    }
                    ImGui.PopStyleColor();
                }
                /*
                //Recently completed task
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(.1f, 1f, .4f, 1.0f));
                ImGui.Spacing();
                ImGui.Text($"{TaskManager.lastCompletedActionString}");
                ImGui.PopStyleColor();
                */

                ImGui.EndMainMenuBar();
            }
            ImGui.PopStyleVar();
            Tracy.TracyCZoneEnd(ctx);

            bool open = true;
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 7.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);

            if (keyBindGUI)
                ImGui.OpenPopup("Key Bind Settings");
            if (ImGui.BeginPopupModal("Key Bind Settings", ref open))
            {
                if (ImGui.IsAnyItemActive())
                {
                    _currentKeyBind = null;
                }
                ImGui.Columns(2);
                foreach (var bind in KeyBindings.Current.GetType().GetFields())
                {
                    var bindVal = (KeyBind)bind.GetValue(KeyBindings.Current);
                    ImGui.Text(bind.Name);
                    ImGui.NextColumn();
                    var keyText = bindVal.HintText;
                    if (keyText == "")
                        keyText = "[None]";
                    if (_currentKeyBind == bindVal)
                    {
                        ImGui.Button("Press Key <Esc - Clear>");
                        if (InputTracker.GetKeyDown(Key.Escape))
                        {
                            bind.SetValue(KeyBindings.Current, new KeyBind());
                            _currentKeyBind = null;
                        }
                        else
                        {
                            var newkey = InputTracker.GetNewKeyBind();
                            if (newkey != null)
                            {
                                bind.SetValue(KeyBindings.Current, newkey);
                                _currentKeyBind = null;
                            }
                        }
                    }
                    else if (ImGui.Button($"{keyText}##{bind.Name}"))
                    {
                        _currentKeyBind = bindVal;
                    }
                    ImGui.NextColumn();
                }
                ImGui.Separator();
                if (ImGui.Button("Restore Defaults"))
                {
                    KeyBindings.ResetKeyBinds();
                }
                ImGui.EndPopup();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f));

            // New project modal
            if (newProject)
            {
                _newProjectOptions.settings = new Editor.ProjectSettings();
                _newProjectOptions.directory = "";
                ImGui.OpenPopup("New Project");
            }
            if (ImGui.BeginPopupModal("New Project", ref open, ImGuiWindowFlags.AlwaysAutoResize)) // The Grey overlay is apparently an imgui bug (that has been fixed in updated builds; in some forks at least).
            {
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Project Name:      ");
                ImGui.SameLine();
                Utils.ImGuiGenericHelpPopup("?", "##Help_ProjectName",
                    "Project's display name. Only affects visuals within DSMS.");
                ImGui.SameLine();
                var pname = _newProjectOptions.settings.ProjectName;
                if (ImGui.InputText("##pname", ref pname, 255))
                {
                    _newProjectOptions.settings.ProjectName = pname;
                }

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Project Directory: ");
                ImGui.SameLine();
                Utils.ImGuiGenericHelpPopup("?", "##Help_ProjectDirectory",
                    "The location mod files will be saved.\nTypically, this should be Mod Engine's Mod folder.");
                ImGui.SameLine();
                ImGui.InputText("##pdir", ref _newProjectOptions.directory, 255);
                ImGui.SameLine();
                if (ImGui.Button($@"{ForkAwesome.FileO}"))
                {
                    var browseDlg = new System.Windows.Forms.FolderBrowserDialog();

                    if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _newProjectOptions.directory = browseDlg.SelectedPath;
                    }
                }

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Game Executable:   ");
                ImGui.SameLine();
                Utils.ImGuiGenericHelpPopup("?", "##Help_GameExecutable",
                    "The location of the game's .EXE file.\nThe folder with the .EXE will be used to obtain unpacked game data.");
                ImGui.SameLine();
                var gname = _newProjectOptions.settings.GameRoot;
                if (ImGui.InputText("##gdir", ref gname, 255))
                {
                    _newProjectOptions.settings.GameRoot = gname;
                    _newProjectOptions.settings.GameType = _assetLocator.GetGameTypeForExePath(_newProjectOptions.settings.GameRoot);
                }
                ImGui.SameLine();
                ImGui.PushID("fd2");
                if (ImGui.Button($@"{ForkAwesome.FileO}"))
                {
                    var browseDlg = new System.Windows.Forms.OpenFileDialog()
                    {
                        Filter = AssetLocator.GameExecutatbleFilter,
                        ValidateNames = true,
                        CheckFileExists = true,
                        CheckPathExists = true,
                        //ShowReadOnly = true,
                    };

                    if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _newProjectOptions.settings.GameRoot = browseDlg.FileName;
                        _newProjectOptions.settings.GameType = _assetLocator.GetGameTypeForExePath(_newProjectOptions.settings.GameRoot);
                    }
                }
                ImGui.PopID();
                ImGui.Text($@"Detected Game:      {_newProjectOptions.settings.GameType.ToString()}");

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();
                if (_newProjectOptions.settings.GameType is GameType.DarkSoulsIISOTFS or GameType.DarkSoulsIII)
                {
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($@"Loose Params:      ");
                    ImGui.SameLine();
                    Utils.ImGuiGenericHelpPopup("?", "##Help_LooseParams",
                        "Default: OFF\n" +
                        "DS2: Save and Load parameters as individual .param files instead of regulation.\n" +
                        "DS3: Save and Load parameters as decrypted .parambnd instead of regulation.");
                    ImGui.SameLine();
                    var looseparams = _newProjectOptions.settings.UseLooseParams;
                    if (ImGui.Checkbox("##looseparams", ref looseparams))
                    {
                        _newProjectOptions.settings.UseLooseParams = looseparams;
                    }
                    ImGui.NewLine();
                }
                else if (FeatureFlags.EnablePartialParam && _newProjectOptions.settings.GameType == GameType.EldenRing)
                {
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($@"Save partial regulation:  ");
                    ImGui.SameLine();
                    Utils.ImGuiGenericHelpPopup("TODO (disbababled)", "##Help_PartialParam",
                        "TODO: why does this setting exist separately from loose params?");
                    ImGui.SameLine();
                    var partialReg = _newProjectOptions.settings.PartialParams;
                    if (ImGui.Checkbox("##partialparams", ref partialReg))
                    {
                        _newProjectOptions.settings.PartialParams = partialReg;
                    }
                    ImGui.SameLine();
                    ImGui.TextUnformatted("Warning: partial params require merging before use in game.\nRow names on unchanged rows will be forgotten between saves");
                    ImGui.NewLine();
                }
                ImGui.AlignTextToFramePadding();
                ImGui.Text($@"Import row names:  ");
                ImGui.SameLine();
                Utils.ImGuiGenericHelpPopup("?", "##Help_ImportRowNames",
                    "Default: ON\nImports and applies row names from lists stored in Assets folder.\nRow names can be imported at any time in the param editor's Edit menu.");
                ImGui.SameLine();
                ImGui.Checkbox("##loadDefaultNames", ref _newProjectOptions.loadDefaultNames);
                ImGui.NewLine();

                if (ImGui.Button("Create", new Vector2(120, 0)))
                {
                    bool validated = true;
                    if (_newProjectOptions.settings.GameRoot == null || !File.Exists(_newProjectOptions.settings.GameRoot))
                    {
                        System.Windows.Forms.MessageBox.Show("Your game executable path does not exist. Please select a valid executable.", "Error",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && _newProjectOptions.settings.GameType == GameType.Undefined)
                    {
                        System.Windows.Forms.MessageBox.Show("Your game executable is not a valid supported game.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && (_newProjectOptions.directory == null || !Directory.Exists(_newProjectOptions.directory)))
                    {
                        System.Windows.Forms.MessageBox.Show("Your selected project directory is not valid.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && File.Exists($@"{_newProjectOptions.directory}\project.json"))
                    {
                        System.Windows.Forms.MessageBox.Show("Your selected project directory is already a project.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && (Path.GetDirectoryName(_newProjectOptions.settings.GameRoot)).Equals(_newProjectOptions.directory))
                    {
                        var message = System.Windows.Forms.MessageBox.Show(
                            "Project Directory is the same as Game Directory, which allows game files to be overwritten directly.\n\n" +
                            "It's highly recommended you use the Mod Engine mod folder as your project folder instead (if possible).\n\n" +
                            "Continue and create project anyway?", "Caution",
                                         System.Windows.Forms.MessageBoxButtons.OKCancel,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        if (message != System.Windows.Forms.DialogResult.OK)
                            validated = false;
                    }
                    if (validated && (_newProjectOptions.settings.ProjectName == null || _newProjectOptions.settings.ProjectName == ""))
                    {
                        System.Windows.Forms.MessageBox.Show("You must specify a project name.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }

                    string gameroot = Path.GetDirectoryName(_newProjectOptions.settings.GameRoot);
                    if (_newProjectOptions.settings.GameType == GameType.Bloodborne)
                    {
                        gameroot = gameroot + @"\dvdroot_ps4";
                    }
                    if (!_assetLocator.CheckFilesExpanded(gameroot, _newProjectOptions.settings.GameType))
                    {
                        System.Windows.Forms.MessageBox.Show($@"The files for {_newProjectOptions.settings.GameType} do not appear to be unpacked. Please use UDSFM for DS1:PTDE and UXM for the rest of the games to unpack the files.", "Error",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }

                    if (validated)
                    {
                        _newProjectOptions.settings.GameRoot = gameroot;
                        _newProjectOptions.settings.Serialize($@"{_newProjectOptions.directory}\project.json");
                        AttemptLoadProject(_newProjectOptions.settings, $@"{_newProjectOptions.directory}\project.json", true, _newProjectOptions);

                        ImGui.CloseCurrentPopup();
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel", new Vector2(120, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
            ImGui.PopStyleVar(3);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            if (FirstFrame)
            {
                ImGui.SetNextWindowFocus();
            }
            string[] mapcmds = null;
            if (commandsplit != null && commandsplit[0] == "map")
            {
                mapcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            ctx = Tracy.TracyCZoneN(1, "Editor");
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            if (ImGui.Begin("Map Editor"))
            {
                ImGui.PopStyleColor(1);
                ImGui.PopStyleVar(1);
                _msbEditor.OnGUI(mapcmds);
                ImGui.End();
                _msbEditorFocused = true;
                _msbEditor.Update(deltaseconds);
            }
            else
            {
                ImGui.PopStyleColor(1);
                ImGui.PopStyleVar(1);
                _msbEditorFocused = false;
                ImGui.End();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            if (ImGui.Begin("Model Editor"))
            {
                ImGui.PopStyleColor(1);
                ImGui.PopStyleVar(1);
                _modelEditor.OnGUI();
                _modelEditorFocused = true;
                _modelEditor.Update(deltaseconds);
            }
            else
            {
                ImGui.PopStyleColor(1);
                ImGui.PopStyleVar(1);
                _modelEditorFocused = false;
            }
            ImGui.End();

            string[] paramcmds = null;
            if (commandsplit != null && commandsplit[0] == "param")
            {
                paramcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            if (ImGui.Begin("Param Editor"))
            {
                _paramEditor.OnGUI(paramcmds);
                _paramEditorFocused = true;
            }
            else
            {
                _paramEditorFocused = false;
            }
            ImGui.End();

            // Global shortcut keys
            if (!_msbEditor.Viewport.ViewportSelected)
            {
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_SaveCurrentEditor))
                    SaveFocusedEditor();
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_SaveAllEditors))
                {
                    _msbEditor.SaveAll();
                    _modelEditor.SaveAll();
                    _paramEditor.SaveAll();
                    _textEditor.SaveAll();
                }
            }

            string[] textcmds = null;
            if (commandsplit != null && commandsplit[0] == "text")
            {
                textcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4));
            if (ImGui.Begin("Text Editor"))
            {
                _textEditor.OnGUI(textcmds);
                _textEditorFocused = true;
            }
            else
            {
                _textEditorFocused = false;
            }
            ImGui.End();
            ImGui.PopStyleVar();

            ImGui.PopStyleVar(2);
            UnapplyStyle();
            Tracy.TracyCZoneEnd(ctx);

            ctx = Tracy.TracyCZoneN(1, "Resource");
            Resource.ResourceManager.UpdateTasks();
            Tracy.TracyCZoneEnd(ctx);

            if (!_firstframe)
            {
                FirstFrame = false;
            }
            _firstframe = false;
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
                _msbEditor.EditorResized(_window, _gd);
                _modelEditor.EditorResized(_window, _gd);
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
            MainWindowCommandList.ClearColorTarget(0, new RgbaFloat(0.176f, 0.176f, 0.188f, 1.0f));
            float depthClear = _gd.IsDepthRangeZeroToOne ? 1f : 0f;
            MainWindowCommandList.ClearDepthStencil(0.0f);
            MainWindowCommandList.SetFullViewport(0);
            //MainWindowCommandList.End();
            //_gd.SubmitCommands(MainWindowCommandList);
            //_gd.WaitForIdle();
            if (_msbEditorFocused)
            {
                _msbEditor.Draw(_gd, MainWindowCommandList);
            }
            if (_modelEditorFocused)
            {
                _modelEditor.Draw(_gd, MainWindowCommandList);
            }
            var fence = Scene.Renderer.Frame(MainWindowCommandList, false);
            //GuiCommandList.Begin();
            //GuiCommandList.SetFramebuffer(_gd.SwapchainFramebuffer);
            MainWindowCommandList.SetFullViewport(0);
            MainWindowCommandList.SetFullScissorRects();
            ImguiRenderer.Render(_gd, MainWindowCommandList);
            //GuiCommandList.End();
            MainWindowCommandList.End();
            _gd.SubmitCommands(MainWindowCommandList, fence);
            Scene.Renderer.SubmitPostDrawCommandLists();
            //Scene.SceneRenderPipeline.TestUpdateView(_gd, MainWindowCommandList, TestWorldView.CameraTransform.CameraViewMatrix);

            _gd.SwapBuffers();
        }
    }
}
