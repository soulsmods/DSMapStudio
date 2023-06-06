using ImGuiNET;
using Octokit;
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
using StudioCore.MsbEditor;
using System.Drawing;
using Vortice.Vulkan;

namespace StudioCore
{
    public class MapStudioNew
    {
        private static string _version = System.Windows.Forms.Application.ProductVersion;
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
        private VkSampleCountFlags? _newSampleCount;

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
        private SettingsMenu _settingsMenu = new();

        private static bool _firstframe = true;
        public static bool FirstFrame = true;
        
        // ImGui Debug windows
        private bool _showImGuiDemoWindow = false;
        private bool _showImGuiMetricsWindow = false;
        private bool _showImGuiDebugLogWindow = false;
        private bool _showImGuiStackToolWindow = false;

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
            //Sdl2ControllerTracker.CreateDefault(out _controllerTracker);

            var factory = _gd.ResourceFactory;
            TextureSamplerResourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
               new ResourceLayoutElementDescription("SourceTexture", VkDescriptorType.SampledImage, VkShaderStageFlags.Fragment),
               new ResourceLayoutElementDescription("SourceSampler", VkDescriptorType.Sampler, VkShaderStageFlags.Fragment)));

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

            _settingsMenu.MsbEditor = _msbEditor;
            _settingsMenu.ModelEditor = _modelEditor;
            _settingsMenu.ParamEditor = _paramEditor;
            _settingsMenu.TextEditor = _textEditor;

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
                    var settings = Editor.ProjectSettings.Deserialize(CFG.Current.LastProjectFile);
                    if (settings == null)
                    {
                        CFG.Current.LastProjectFile = "";
                        CFG.Save();
                    }
                    else
                    {
                        AttemptLoadProject(settings, CFG.Current.LastProjectFile, false);
                    }
                }
                else
                {
                    MessageBox.Show($"Project.json at \"{CFG.Current.LastProjectFile}\" does not exist.", "Project Load Error", MessageBoxButtons.OK);
                    CFG.Current.LastProjectFile = "";
                    CFG.Save();
                }
            }
        }

        /// <summary>
        /// Characters to load that FromSoft use, but aren't included in the ImGui Japanese glyph range.
        /// </summary>
        private readonly char[] SpecialCharsJP = { '鉤', '梟', '倅', '…', '飴', '護', '戮', 'ā', 'ī', 'ū', 'ē', 'ō', 'Ā', 'Ē', 'Ī', 'Ō', 'Ū' };

        private unsafe void SetupFonts()
        {
            var fonts = ImGui.GetIO().Fonts;
            var fileEn = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\RobotoMono-Light.ttf");
            var fontEn = File.ReadAllBytes(fileEn);
            var fontEnNative = ImGui.MemAlloc((uint)fontEn.Length);
            Marshal.Copy(fontEn, 0, fontEnNative, fontEn.Length);
            var fileOther = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\NotoSansCJKtc-Light.otf");
            var fontOther = File.ReadAllBytes(fileOther);
            var fontOtherNative = ImGui.MemAlloc((uint)fontOther.Length);
            Marshal.Copy(fontOther, 0, fontOtherNative, fontOther.Length);
            var fileIcon = Path.Combine(AppContext.BaseDirectory, $@"Assets\Fonts\forkawesome-webfont.ttf");
            var fontIcon = File.ReadAllBytes(fileIcon);
            var fontIconNative = ImGui.MemAlloc((uint)fontIcon.Length);
            Marshal.Copy(fontIcon, 0, fontIconNative, fontIcon.Length);
            fonts.Clear();

            float scale = ImGuiRenderer.GetUIScale();

            // English fonts
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.GlyphMinAdvanceX = 5.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                fonts.AddFontFromMemoryTTF(fontEnNative, fontIcon.Length, 14.0f * scale, cfg, fonts.GetGlyphRangesDefault());
            }

            // Other language fonts
            {
                var ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                var cfg = new ImFontConfigPtr(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 7.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;

                var glyphRanges = new ImFontGlyphRangesBuilderPtr(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
                glyphRanges.AddRanges(fonts.GetGlyphRangesJapanese());
                Array.ForEach(SpecialCharsJP, c => glyphRanges.AddChar(c));

                if (CFG.Current.FontChinese)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesChineseFull());
                if (CFG.Current.FontKorean)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesKorean());
                if (CFG.Current.FontThai)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesThai());
                if (CFG.Current.FontVietnamese)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesVietnamese());
                if (CFG.Current.FontCyrillic)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesCyrillic());

                glyphRanges.BuildRanges(out ImVector glyphRange);
                fonts.AddFontFromMemoryTTF(fontOtherNative, fontOther.Length, 16.0f * scale, cfg, glyphRange.Data);
                glyphRanges.Destroy();
            }

            // Icon fonts
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
                    var f = fonts.AddFontFromMemoryTTF(fontIconNative, fontIcon.Length, 16.0f * scale, cfg, (IntPtr)r);
                }
            }

            ImguiRenderer.RecreateFontDeviceTexture();
        }

        public void SetupCSharpDefaults()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private bool programUpdateAvailable = false;
        private string releaseUrl = "";
        private async Task CheckProgramUpdate()
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("DSMapStudio"));
            try
            {
                Release release = await gitHubClient.Repository.Release.GetLatest("soulsmods", "DSMapStudio");
                bool isVer = false;
                string verstring = "";
                foreach (char c in release.TagName)
                {
                    if (char.IsDigit(c) || (isVer && c == '.'))
                    {
                        verstring += c;
                        isVer = true;
                    }
                    else
                    {
                        isVer = false;
                    }
                }
                if (Version.Parse(verstring) > Version.Parse(_version))
                {
                    // Update available
                    programUpdateAvailable = true;
                    releaseUrl = release.HtmlUrl;
                }
            }
            catch(Exception e)
            {
#if DEBUG
                TaskManager.warningList.TryAdd("ProgramUpdateCheckFail", $"Failed to check for program updates ({e.Message})");
#endif
            }
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

            if (CFG.Current.EnableCheckProgramUpdate)
            {
                CheckProgramUpdate();
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

        private void ChangeProjectSettings(Editor.ProjectSettings newsettings, string moddir, NewProjectOptions options)
        {
            _projectSettings = newsettings;
            _assetLocator.SetFromProjectSettings(newsettings, moddir);
            _settingsMenu.ProjSettings = _projectSettings;

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
            float scale = ImGuiRenderer.GetUIScale();
            var style = ImGui.GetStyle();

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
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 16.0f * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(100f, 100f) * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, style.FramePadding * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, style.CellPadding * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, style.IndentSpacing * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, style.ItemSpacing * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, style.ItemInnerSpacing * scale);
        }

        public void UnapplyStyle()
        {
            ImGui.PopStyleColor(27);
            ImGui.PopStyleVar(10);
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

        private bool GameNotUnpackedWarning(GameType gameType)
        {
            if (gameType is GameType.DarkSoulsPTDE or GameType.DarkSoulsIISOTFS)
            {
                MessageBox.Show($@"The files for {gameType} do not appear to be unpacked. Please use UDSFM for DS1:PTDE and UXM for DS2 to unpack the files.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None);
                return false;
            }
            else
            {
                TaskManager.warningList.TryAdd($"GameNotUnpacked{gameType}", $"The files for {gameType} do not appear to be fully unpacked. Functionality will be limited.\nPlease use UXM to unpack the files.");
                return true;
            }
        }

        private bool AttemptLoadProject(Editor.ProjectSettings settings, string filename, bool updateRecents = true, NewProjectOptions options = null)
        {
            bool success = true;
            try
            {
                // Check if game exe exists
                if (!Directory.Exists(settings.GameRoot))
                {
                    success = false;
                    System.Windows.Forms.MessageBox.Show($@"Could not find game data directory for {settings.GameType}. Please select the game executable.", "Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.None);

                    var rbrowseDlg = new System.Windows.Forms.OpenFileDialog()
                    {
                        Filter = AssetLocator.GameExecutableFilter,
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
                        if (!GameNotUnpackedWarning(settings.GameType))
                            return false;
                    }
                    if ((settings.GameType == GameType.Sekiro || settings.GameType == GameType.EldenRing) && !File.Exists(Path.Join(Path.GetFullPath("."), "oo2core_6_win64.dll")))
                    {
                        if (!File.Exists(Path.Join(settings.GameRoot, "oo2core_6_win64.dll")))
                        {
                            MessageBox.Show($"Could not find file \"oo2core_6_win64.dll\" in \"{settings.GameRoot}\", which should be included by default.\n\nTry reinstalling the game.", "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.None);
                            return false;
                        }
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
            }
            catch
            {
                // Error loading project, clear recent project to let the user launch the program next time without issue.
                CFG.Current.LastProjectFile = "";
                CFG.Save();
                throw;
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

        private bool _standardProjectUIOpened = true;
        private void NewProject_NameGUI()
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
        }

        private void NewProject_ProjectDirectoryGUI()
        {
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
        }

        private void NewProject_GameTypeComboGUI()
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text($@"Game Type:         ");
            ImGui.SameLine();
            string[] games = Enum.GetNames(typeof(GameType));
            int gameIndex = Array.IndexOf(games, _newProjectOptions.settings.GameType.ToString());
            if (ImGui.Combo("##GameTypeCombo", ref gameIndex, games, games.Length))
            {
                _newProjectOptions.settings.GameType = Enum.Parse<GameType>(games[gameIndex]);
            }
        }

        private void Update(float deltaseconds)
        {
            var ctx = Tracy.TracyCZoneN(1, "Imgui");

            float scale = ImGuiRenderer.GetUIScale();

            if (_settingsMenu.FontRebuildRequest)
            {
                ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, SetupFonts);
                _settingsMenu.FontRebuildRequest = false;
            }
            else
            {
                ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, null);
            }

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
                            if (settings != null)
                            {
                                AttemptLoadProject(settings, browseDlg.FileName);
                            }
                        }
                    }
                    if (ImGui.BeginMenu("Recent Projects", Editor.TaskManager.GetLiveThreads().Count == 0 && CFG.Current.RecentProjects.Count > 0))
                    {
                        CFG.RecentProject recent = null;
                        int id = 0;
                        foreach (var p in CFG.Current.RecentProjects.ToArray())
                        {
                            if (ImGui.MenuItem($@"{p.GameType}: {p.Name}##{id}"))
                            {
                                if (File.Exists(p.ProjectFile))
                                {
                                    var settings = Editor.ProjectSettings.Deserialize(p.ProjectFile);
                                    if (settings != null)
                                    {
                                        if (AttemptLoadProject(settings, p.ProjectFile, false))
                                        {
                                            recent = p;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show($"Project.json at \"{p.ProjectFile}\" does not exist.\nRemoving project from recent projects list.", "Project Load Error", MessageBoxButtons.OK);
                                    CFG.Current.RecentProjects.Remove(p);
                                    CFG.Save();
                                }
                            }
                            if (ImGui.BeginPopupContextItem())
                            {
                                if (ImGui.Selectable("Remove from list"))
                                {
                                    CFG.Current.RecentProjects.Remove(p);
                                    CFG.Save();
                                }
                                ImGui.EndPopup();
                            }
                            id++;
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
                    
                    if (ImGui.MenuItem("Editor Settings"))
                    {
                        _settingsMenu.MenuOpenState = true;
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
                                   "Nordgaren\n" +
                                   "ivi\n" +
                                   "Vawser\n\n" +
                                   "Special Thanks:\n" +
                                   "TKGP\n" +
                                   "Meowmaritus\n" +
                                   "Radai\n" +
                                   "Moonlight Ruin\n" +
                                   "Evan (HalfGrownHollow)");
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

                    if (ImGui.BeginMenu("ImGui Debug"))
                    {
                        if (ImGui.MenuItem("Demo"))
                        {
                            _showImGuiDemoWindow = true;
                        }
                        if (ImGui.MenuItem("Metrics"))
                        {
                            _showImGuiMetricsWindow = true;
                        }
                        if (ImGui.MenuItem("Debug Log"))
                        {
                            _showImGuiDebugLogWindow = true;
                        }
                        if (ImGui.MenuItem("Stack Tool"))
                        {
                            _showImGuiStackToolWindow = true;
                        }
                        ImGui.EndMenu();
                    }
                }

                if (programUpdateAvailable)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
                    if (ImGui.Button("Update Available"))
                    {
                        Process myProcess = new();
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = releaseUrl;
                        myProcess.Start();
                    }
                    ImGui.PopStyleColor();
                }

                if (TaskManager.GetLiveThreads().Count > 0 && ImGui.BeginMenu("Tasks"))
                {
                    foreach (String task in TaskManager.GetLiveThreads())
                    {
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

                ImGui.EndMainMenuBar();
            }

            SettingsGUI();

            ImGui.PopStyleVar();
            Tracy.TracyCZoneEnd(ctx);

            bool open = true;
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 7.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f) * scale);

            // ImGui Debug windows
            if (_showImGuiDemoWindow)
                ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
            if (_showImGuiMetricsWindow)
                ImGui.ShowMetricsWindow(ref _showImGuiMetricsWindow);
            if (_showImGuiDebugLogWindow)
                ImGui.ShowDebugLogWindow(ref _showImGuiDebugLogWindow);
            if (_showImGuiStackToolWindow)
                ImGui.ShowStackToolWindow(ref _showImGuiStackToolWindow);

            // New project modal
            if (newProject)
            {
                _newProjectOptions.settings = new Editor.ProjectSettings();
                _newProjectOptions.directory = "";
                ImGui.OpenPopup("New Project");
            }
            if (ImGui.BeginPopupModal("New Project", ref open, ImGuiWindowFlags.AlwaysAutoResize))
            {
                //
                ImGui.BeginTabBar("NewProjectTabBar");
                if (ImGui.BeginTabItem("Standard"))
                {
                    if (!_standardProjectUIOpened)
                        _newProjectOptions.settings.GameType = GameType.Undefined;
                    _standardProjectUIOpened = true;

                    NewProject_NameGUI();
                    NewProject_ProjectDirectoryGUI();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Game Executable:   ");
                    ImGui.SameLine();
                    Utils.ImGuiGenericHelpPopup("?", "##Help_GameExecutable",
                        "The location of the game's .EXE or EBOOT.BIN file.\nThe folder with the executable will be used to obtain unpacked game data.");
                    ImGui.SameLine();
                    var gname = _newProjectOptions.settings.GameRoot;
                    if (ImGui.InputText("##gdir", ref gname, 255))
                    {
                        if (File.Exists(gname))
                            _newProjectOptions.settings.GameRoot = Path.GetDirectoryName(gname);
                        else
                            _newProjectOptions.settings.GameRoot = gname;
                        _newProjectOptions.settings.GameType = _assetLocator.GetGameTypeForExePath(gname);

                        if (_newProjectOptions.settings.GameType == GameType.Bloodborne)
                        {
                            _newProjectOptions.settings.GameRoot = _newProjectOptions.settings.GameRoot + @"\dvdroot_ps4";
                        }
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($@"{ForkAwesome.FileO}##fd2"))
                    {
                        var browseDlg = new System.Windows.Forms.OpenFileDialog()
                        {
                            Filter = AssetLocator.GameExecutableFilter,
                            ValidateNames = true,
                            CheckFileExists = true,
                            CheckPathExists = true,
                        };

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _newProjectOptions.settings.GameRoot = Path.GetDirectoryName(browseDlg.FileName);
                            _newProjectOptions.settings.GameType = _assetLocator.GetGameTypeForExePath(browseDlg.FileName);

                            if (_newProjectOptions.settings.GameType == GameType.Bloodborne)
                            {
                                _newProjectOptions.settings.GameRoot = _newProjectOptions.settings.GameRoot + @"\dvdroot_ps4";
                            }
                        }
                    }
                    ImGui.Text($@"Detected Game:      {_newProjectOptions.settings.GameType}");

                    ImGui.EndTabItem();
                }
                else
                {
                    _standardProjectUIOpened = false;
                }
                
                if (ImGui.BeginTabItem("Advanced"))
                {
                    NewProject_NameGUI();
                    NewProject_ProjectDirectoryGUI();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Game Directory:    ");
                    ImGui.SameLine();
                    Utils.ImGuiGenericHelpPopup("?", "##Help_GameDirectory",
                        "The location of game files.\nTypically, this should be the location of the game executable.");
                    ImGui.SameLine();
                    var gname = _newProjectOptions.settings.GameRoot;
                    if (ImGui.InputText("##gdir", ref gname, 255))
                    {
                        _newProjectOptions.settings.GameRoot = gname;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($@"{ForkAwesome.FileO}##fd2"))
                    {
                        var browseDlg = new System.Windows.Forms.FolderBrowserDialog();

                        if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _newProjectOptions.settings.GameRoot = browseDlg.SelectedPath;
                        }
                    }
                    NewProject_GameTypeComboGUI();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
                //

                ImGui.Separator();
                if (_newProjectOptions.settings.GameType is GameType.DarkSoulsIISOTFS or GameType.DarkSoulsIII)
                {
                    ImGui.NewLine();
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
                }
                else if (FeatureFlags.EnablePartialParam && _newProjectOptions.settings.GameType == GameType.EldenRing)
                {
                    ImGui.NewLine();
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
                }
                ImGui.NewLine();

                ImGui.AlignTextToFramePadding();
                ImGui.Text($@"Import row names:  ");
                ImGui.SameLine();
                Utils.ImGuiGenericHelpPopup("?", "##Help_ImportRowNames",
                    "Default: ON\nImports and applies row names from lists stored in Assets folder.\nRow names can be imported at any time in the param editor's Edit menu.");
                ImGui.SameLine();
                ImGui.Checkbox("##loadDefaultNames", ref _newProjectOptions.loadDefaultNames);
                if (_newProjectOptions.settings.UseLooseParams == false
                    && _newProjectOptions.loadDefaultNames == true
                    && _newProjectOptions.settings.GameType == GameType.DarkSoulsIISOTFS)
                {
                    ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.4f, 1.0f), "Warning: Saving row names onto non-loose params will crash the game. It is highly recommended you use loose params with Dark Souls 2.");
                }
                ImGui.NewLine();

                if (_newProjectOptions.settings.GameType == GameType.Undefined)
                    ImGui.BeginDisabled();
                if (ImGui.Button("Create", new Vector2(120, 0) * scale))
                {
                    bool validated = true;
                    if (_newProjectOptions.settings.GameRoot == null || !Directory.Exists(_newProjectOptions.settings.GameRoot))
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
                        var message = System.Windows.Forms.MessageBox.Show("Your selected project directory already contains a project.json. Would you like to replace it?", "Error",
                                         System.Windows.Forms.MessageBoxButtons.YesNo,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        if (message == DialogResult.No)
                            validated = false;
                    }
                    if (validated && _newProjectOptions.settings.GameRoot == _newProjectOptions.directory)
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

                    var gameroot = _newProjectOptions.settings.GameRoot;
                    if (!_assetLocator.CheckFilesExpanded(gameroot, _newProjectOptions.settings.GameType))
                    {
                        if (!GameNotUnpackedWarning(_newProjectOptions.settings.GameType))
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
                if (_newProjectOptions.settings.GameType == GameType.Undefined)
                    ImGui.EndDisabled();

                ImGui.SameLine();
                if (ImGui.Button("Cancel", new Vector2(120, 0) * scale))
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
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4) * scale);
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

        public void SettingsGUI()
        {
                _settingsMenu.Display();
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
