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
using System.Threading.Tasks;
using Gtk;
using SoapstoneLib;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using StudioCore.Graphics;
using StudioCore.Platform;
using Vortice.Vulkan;
using Application = Gtk.Application;

namespace StudioCore
{
    public class MapStudioNew
    {
        private readonly string _version;
        private readonly string _programTitle;

        private static double _desiredFrameLengthSeconds = 1.0 / 20.0f;
        private static bool _limitFrameRate = true;

        private IGraphicsContext _context;

        private List<EditorScreen> _editors;
        private EditorScreen _focusedEditor;

        private SoapstoneService _soapstoneService;

        private AssetLocator _assetLocator;
        private Editor.ProjectSettings _projectSettings = null;

        private NewProjectOptions _newProjectOptions = new NewProjectOptions();
        private SettingsMenu _settingsMenu = new();

        private static bool _initialLoadComplete = false;
        private static bool _firstframe = true;
        public static bool FirstFrame = true;
        
        // ImGui Debug windows
        private bool _showImGuiDemoWindow = false;
        private bool _showImGuiMetricsWindow = false;
        private bool _showImGuiDebugLogWindow = false;
        private bool _showImGuiStackToolWindow = false;

        public unsafe MapStudioNew(IGraphicsContext context, string version)
        {
            _version = version;
            _programTitle = $"Dark Souls Map Studio version {_version}";
            
            // Hack to make sure dialogs work before the main window is created
            PlatformUtils.InitializeWindows(null);
            CFG.AttemptLoadOrDefault();
            Application.Init();
            
            _context = context;
            _context.Initialize();
            _context.Window.Title = _programTitle;
            PlatformUtils.InitializeWindows(context.Window.SdlWindowHandle);

            _assetLocator = new AssetLocator();
            var msbEditor = new MsbEditor.MsbEditorScreen(_context.Window, _context.Device, _assetLocator);
            var modelEditor = new MsbEditor.ModelEditorScreen(_context.Window, _context.Device, _assetLocator);
            var paramEditor = new ParamEditor.ParamEditorScreen(_context.Window, _context.Device, _assetLocator);
            var textEditor = new TextEditor.TextEditorScreen(_context.Window, _context.Device, _assetLocator);
            _editors = new List<EditorScreen>()
            {
                msbEditor, modelEditor, paramEditor, textEditor
            };
            _focusedEditor = msbEditor;

            _soapstoneService = new SoapstoneService(_version, _assetLocator, msbEditor);

            _settingsMenu.MsbEditor = msbEditor;
            _settingsMenu.ModelEditor = modelEditor;
            _settingsMenu.ParamEditor = paramEditor;
            _settingsMenu.TextEditor = textEditor;

            Editor.AliasBank.SetAssetLocator(_assetLocator);
            ParamEditor.ParamBank.PrimaryBank.SetAssetLocator(_assetLocator);
            ParamEditor.ParamBank.VanillaBank.SetAssetLocator(_assetLocator);
            TextEditor.FMGBank.SetAssetLocator(_assetLocator);
            MsbEditor.MtdBank.LoadMtds(_assetLocator);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            SetupFonts();
            _context.ImguiRenderer.OnSetupDone();

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
                    PlatformUtils.Instance.MessageBox($"Project.json at \"{CFG.Current.LastProjectFile}\" does not exist.", "Project Load Error", MessageBoxButtons.OK);
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

            float scale = GetUIScale();

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

            _context.ImguiRenderer.RecreateFontDeviceTexture();
        }

        public void SetupCSharpDefaults()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private bool _programUpdateAvailable = false;
        private string _releaseUrl = "";
        private void CheckProgramUpdate()
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("DSMapStudio"));
            Release release = gitHubClient.Repository.Release.GetLatest("soulsmods", "DSMapStudio").Result;
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
                _programUpdateAvailable = true;
                _releaseUrl = release.HtmlUrl;
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
                TaskManager.Run(new("Initialize Soapstone Server", false, false, true, () => SoapstoneServer.RunAsync(KnownServer.DSMapStudio, _soapstoneService)));
            }

            if (CFG.Current.EnableCheckProgramUpdate)
            {
                TaskManager.Run(new("Check Program Updates", false, false, true, () => CheckProgramUpdate()));
            }

            long previousFrameTicks = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Tracy.Startup();
            while (_context.Window.Exists)
            {
                Tracy.TracyCFrameMark();

                // Limit frame rate when window isn't focused unless we are profiling
                bool focused = Tracy.EnableTracy ? true : _context.Window.Focused;
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
                snapshot = _context.Window.PumpEvents();
                InputTracker.UpdateFrameInput(snapshot, _context.Window);
                Update((float)deltaSeconds);
                Tracy.TracyCZoneEnd(ctx);
                if (!_context.Window.Exists)
                {
                    break;
                }

                if (true)//_window.Focused)
                {
                    ctx = Tracy.TracyCZoneNC(1, "Draw", 0xFFFF0000);
                    _context.Draw(_editors, _focusedEditor);
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
            _context.Dispose();
            CFG.Save();

            Application.Quit();
        }

        // Try to shutdown things gracefully on a crash
        public void CrashShutdown()
        {
            Tracy.Shutdown();
            Resource.ResourceManager.Shutdown();
            _context.Dispose();
            Application.Quit();
        }

        private void ChangeProjectSettings(Editor.ProjectSettings newsettings, string moddir, NewProjectOptions options)
        {
            _projectSettings = newsettings;
            _assetLocator.SetFromProjectSettings(newsettings, moddir);
            _settingsMenu.ProjSettings = _projectSettings;

            Editor.AliasBank.ReloadAliases();
            ParamEditor.ParamBank.ReloadParams(newsettings, options);
            MsbEditor.MtdBank.ReloadMtds();

            foreach (var editor in _editors)
            {
                editor.OnProjectChanged(_projectSettings);
            }
        }

        public void ApplyStyle()
        {
            float scale = GetUIScale();
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
            using FileChooserNative fileChooser = new FileChooserNative("Save Flver layout dump",
                null, FileChooserAction.Save, "Save", "Cancel");
            fileChooser.AddFilter(_assetLocator.TxtFilter);
            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                using (var file = new StreamWriter(fileChooser.Filename))
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
                TaskLogs.AddLog($"The files for {gameType} do not appear to be unpacked. Please use UDSFM for DS1:PTDE and UXM for DS2 to unpack game files",
                    Microsoft.Extensions.Logging.LogLevel.Error,
                    TaskLogs.LogPriority.High);
                return false;
            }
            else
            {
                TaskLogs.AddLog($"The files for {gameType} do not appear to be fully unpacked. Functionality will be limited. Please use UXM selective unpacker to unpack game files",
                    Microsoft.Extensions.Logging.LogLevel.Warning);
                return true;
            }
        }

        private bool AttemptLoadProject(Editor.ProjectSettings settings, string filename, bool updateRecents = true, NewProjectOptions options = null)
        {
            bool success = true;
            // Check if game exe exists
            if (!Directory.Exists(settings.GameRoot))
            {
                success = false;
                PlatformUtils.Instance.MessageBox($@"Could not find game data directory for {settings.GameType}. Please select the game executable.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.None);

                using FileChooserNative fileChooser = new FileChooserNative($"Select executable for {settings.GameType}...",
                    null, FileChooserAction.Open, "Open", "Cancel");
                fileChooser.AddFilter(_assetLocator.GameExecutableFilter);
                fileChooser.AddFilter(_assetLocator.AllFilesFilter);
                var gametype = GameType.Undefined;
                while (gametype != settings.GameType)
                {
                    if (fileChooser.Run() == (int)ResponseType.Accept)
                    {
                        settings.GameRoot = fileChooser.Filename;
                        gametype = _assetLocator.GetGameTypeForExePath(settings.GameRoot);
                        if (gametype != settings.GameType)
                        {
                            PlatformUtils.Instance.MessageBox($@"Selected executable was not for {settings.GameType}. Please select the correct game executable.", "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.None);
                        }
                        else
                        {
                            success = true;
                            settings.GameRoot = Path.GetDirectoryName(settings.GameRoot);
                            if (settings.GameType == GameType.Bloodborne)
                            {
                                settings.GameRoot += @"\dvdroot_ps4";
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
                        PlatformUtils.Instance.MessageBox($"Could not find file \"oo2core_6_win64.dll\" in \"{settings.GameRoot}\", which should be included by default.\n\nTry reinstalling the game.", "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);
                        return false;
                    }
                    File.Copy(Path.Join(settings.GameRoot, "oo2core_6_win64.dll"), Path.Join(Path.GetFullPath("."), "oo2core_6_win64.dll"));
                }
                _projectSettings = settings;
                ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(filename), options);
                CFG.Current.LastProjectFile = filename;
                _context.Window.Title = $"{_programTitle}  -  {_projectSettings.ProjectName}";

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

        public void SaveAll()
        {
            foreach (var editor in _editors)
            {
                editor.SaveAll();
            }
        }

        // Saves modded files to a recovery directory in the mod folder on crash
        public void AttemptSaveOnCrash()
        {

            if (!_initialLoadComplete)
            {
                // Program crashed on initial load, clear recent project to let the user launch the program next time without issue.
                try
                {
                    CFG.Current.LastProjectFile = "";
                    CFG.Save();
                }
                catch(Exception e)
                {
                    PlatformUtils.Instance.MessageBox($"Unable to save config during crash recovery.\n" +
                        $"If you continue to crash on startup, delete config in AppData\\Local\\DSMapStudio\n\n" +
                        $"{e.Message} {e.StackTrace}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            bool success = _assetLocator.CreateRecoveryProject();
            if (success)
            {
                SaveAll();
                PlatformUtils.Instance.MessageBox(
                    $"Your project was successfully saved to {_assetLocator.GameModDirectory} for manual recovery.\n" +
                    "You must manually replace your projects with these recovery files should you wish to restore them.\n" +
                    "Given the program has crashed, these files may be corrupt and you should backup your last good saved\n" +
                    "files before attempting to use these.",
                    "Saved recovery",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void SaveFocusedEditor()
        {
            if (_projectSettings != null && _projectSettings.ProjectName != null)
            {
                // Danger zone assuming on lastProjectFile
                _projectSettings.Serialize(CFG.Current.LastProjectFile);
                _focusedEditor.Save();
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
                using FileChooserNative fileChooser = new FileChooserNative($"Select project directory...",
                    null, FileChooserAction.SelectFolder, "Open", "Cancel");
                if (fileChooser.Run() == (int)ResponseType.Accept)
                {
                    _newProjectOptions.directory = fileChooser.Filename;
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

        private unsafe void Update(float deltaseconds)
        {
            var ctx = Tracy.TracyCZoneN(1, "Imgui");

            float scale = MapStudioNew.GetUIScale();

            if (_settingsMenu.FontRebuildRequest)
            {
                _context.ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, SetupFonts);
                _settingsMenu.FontRebuildRequest = false;
            }
            else
            {
                _context.ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, null);
            }

            Tracy.TracyCZoneEnd(ctx);
            List<string> tasks = Editor.TaskManager.GetLiveThreads();
            Editor.TaskManager.ThrowTaskExceptions();

            string[] commandsplit = EditorCommandQueue.GetNextCommand();
            if (commandsplit != null && commandsplit[0] == "windowFocus")
            {
                //this is a hack, cannot grab focus except for when un-minimising
                _user32_ShowWindow(_context.Window.Handle, 6);
                _user32_ShowWindow(_context.Window.Handle, 9);
            }

            ctx = Tracy.TracyCZoneN(1, "Style");
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
                        using FileChooserNative fileChooser = new FileChooserNative("Choose the project json file",
                            null, FileChooserAction.Open, "Open", "Cancel");
                        fileChooser.AddFilter(_assetLocator.ProjectJsonFilter);
                        if (fileChooser.Run() == (int)ResponseType.Accept)
                        {
                            var settings = ProjectSettings.Deserialize(fileChooser.Filename);
                            if (settings != null)
                            {
                                AttemptLoadProject(settings, fileChooser.Filename);
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
                                    PlatformUtils.Instance.MessageBox($"Project.json at \"{p.ProjectFile}\" does not exist.\nRemoving project from recent projects list.", "Project Load Error", MessageBoxButtons.OK);
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

                    if (ImGui.MenuItem($"Save {_focusedEditor.SaveType}", KeyBindings.Current.Core_SaveCurrentEditor.HintText))
                    {
                        SaveFocusedEditor();
                    }
                    if (ImGui.MenuItem("Save All", KeyBindings.Current.Core_SaveAllEditors.HintText))
                    {
                        SaveAll();
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

                _focusedEditor.DrawEditorMenu();

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
                                   "Evan (HalfGrownHollow)\n" +
                                   "MyMaidisKitchenAid");
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

                if (_programUpdateAvailable)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
                    if (ImGui.Button("Update Available"))
                    {
                        Process myProcess = new();
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = _releaseUrl;
                        myProcess.Start();
                    }
                    ImGui.PopStyleColor();
                }

                if (ImGui.BeginMenu("Tasks", TaskManager.GetLiveThreads().Count > 0))
                {
                    foreach (String task in TaskManager.GetLiveThreads())
                    {
                        ImGui.Text(task);
                    }
                    ImGui.EndMenu();
                }

                TaskLogs.Display();

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
                            _newProjectOptions.settings.GameRoot += @"\dvdroot_ps4";
                        }
                    }
                    ImGui.SameLine();
                    if (ImGui.Button($@"{ForkAwesome.FileO}##fd2"))
                    {
                        using FileChooserNative fileChooser = new FileChooserNative($"Select executable for the game you want to mod...",
                            null, FileChooserAction.Open, "Open", "Cancel");
                        fileChooser.AddFilter(_assetLocator.GameExecutableFilter);
                        fileChooser.AddFilter(_assetLocator.AllFilesFilter);
                        if (fileChooser.Run() == (int)ResponseType.Accept)
                        {
                            _newProjectOptions.settings.GameRoot = Path.GetDirectoryName(fileChooser.Filename);
                            _newProjectOptions.settings.GameType = _assetLocator.GetGameTypeForExePath(fileChooser.Filename);

                            if (_newProjectOptions.settings.GameType == GameType.Bloodborne)
                            {
                                _newProjectOptions.settings.GameRoot += @"\dvdroot_ps4";
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
                        using FileChooserNative fileChooser = new FileChooserNative($"Select project directory...",
                            null, FileChooserAction.SelectFolder, "Open", "Cancel");

                        if (fileChooser.Run() == (int)ResponseType.Accept)
                        {
                            _newProjectOptions.settings.GameRoot = fileChooser.Filename;
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
                ImGui.NewLine();

                if (_newProjectOptions.settings.GameType == GameType.Undefined)
                    ImGui.BeginDisabled();
                if (ImGui.Button("Create", new Vector2(120, 0) * scale))
                {
                    bool validated = true;
                    if (_newProjectOptions.settings.GameRoot == null || !Directory.Exists(_newProjectOptions.settings.GameRoot))
                    {
                        PlatformUtils.Instance.MessageBox("Your game executable path does not exist. Please select a valid executable.", "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && _newProjectOptions.settings.GameType == GameType.Undefined)
                    {
                        PlatformUtils.Instance.MessageBox("Your game executable is not a valid supported game.", "Error",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && (_newProjectOptions.directory == null || !Directory.Exists(_newProjectOptions.directory)))
                    {
                        PlatformUtils.Instance.MessageBox("Your selected project directory is not valid.", "Error",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && File.Exists($@"{_newProjectOptions.directory}\project.json"))
                    {
                        var message = PlatformUtils.Instance.MessageBox("Your selected project directory already contains a project.json. Would you like to replace it?", "Error",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.None);
                        if (message == DialogResult.No)
                            validated = false;
                    }
                    if (validated && _newProjectOptions.settings.GameRoot == _newProjectOptions.directory)
                    {
                        var message = PlatformUtils.Instance.MessageBox(
                            "Project Directory is the same as Game Directory, which allows game files to be overwritten directly.\n\n" +
                            "It's highly recommended you use the Mod Engine mod folder as your project folder instead (if possible).\n\n" +
                            "Continue and create project anyway?", "Caution",
                                         MessageBoxButtons.OKCancel,
                                         MessageBoxIcon.None);
                        if (message != DialogResult.OK)
                            validated = false;
                    }
                    if (validated && (_newProjectOptions.settings.ProjectName == null || _newProjectOptions.settings.ProjectName == ""))
                    {
                        PlatformUtils.Instance.MessageBox("You must specify a project name.", "Error",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.None);
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

            if (FirstFrame)
            {
                ImGui.SetNextWindowFocus();
            }

            ctx = Tracy.TracyCZoneN(1, "Editor");
            foreach (var editor in _editors)
            {
                string[] commands = null;
                if (commandsplit != null && commandsplit[0] == editor.CommandEndpoint)
                {
                    commands = commandsplit.Skip(1).ToArray();
                    ImGui.SetNextWindowFocus();
                }
                
                if (_context.Device == null)
                    ImGui.PushStyleColor(ImGuiCol.WindowBg, *ImGui.GetStyleColorVec4(ImGuiCol.WindowBg));
                else
                    ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
                if (ImGui.Begin(editor.EditorName))
                {
                    ImGui.PopStyleColor(1);
                    ImGui.PopStyleVar(1);
                    editor.OnGUI(commands);
                    ImGui.End();
                    _focusedEditor = editor;
                    editor.Update(deltaseconds);
                }
                else
                {
                    ImGui.PopStyleColor(1);
                    ImGui.PopStyleVar(1);
                    ImGui.End();
                }
            }

            // Global shortcut keys
            if (!_focusedEditor.InputCaptured())
            {
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_SaveCurrentEditor))
                    SaveFocusedEditor();
                if (InputTracker.GetKeyDown(KeyBindings.Current.Core_SaveAllEditors))
                {
                    SaveAll();
                }
            }

            ImGui.PopStyleVar(2);
            UnapplyStyle();
            Tracy.TracyCZoneEnd(ctx);

            ctx = Tracy.TracyCZoneN(1, "Resource");
            Resource.ResourceManager.UpdateTasks();
            Tracy.TracyCZoneEnd(ctx);

            if (!_initialLoadComplete)
            {
                if (!tasks.Any())
                {
                    _initialLoadComplete = true;
                }
            }

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

        public static float GetUIScale()
        {
            // TODO: Multiply by monitor DPI when available.
            return CFG.Current.UIScale;
        }
    }
}
