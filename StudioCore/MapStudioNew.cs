using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace StudioCore
{
    public class MapStudioNew
    {
        private static string _version = "Preview 6";

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
        private MsbEditor.MsbEditorScreen MSBEditor;
        private bool _modelEditorFocused = false;
        private MsbEditor.ModelEditorScreen ModelEditor;
        private bool _paramEditorFocused = false;
        private MsbEditor.ParamEditorScreen ParamEditor;
        private bool _textEditorFocused = false;
        private MsbEditor.TextEditorScreen TextEditor;

        public static RenderDoc RenderDocManager;

        private const bool UseRenderdoc = false;

        private AssetLocator _assetLocator;
        private MsbEditor.ProjectSettings _projectSettings = null;

        private MsbEditor.ProjectSettings _newProjectSettings;
        private string _newProjectDirectory = "";

        private static bool _firstframe = true;
        public static bool FirstFrame = true;

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
                WindowTitle = "Dark Souls Map Studio " + _version + " by Katalash",
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
            MSBEditor = new MsbEditor.MsbEditorScreen(_window, _gd, _assetLocator);
            ModelEditor = new MsbEditor.ModelEditorScreen(_window, _gd, _assetLocator);
            ParamEditor = new MsbEditor.ParamEditorScreen(_window, _gd);
            TextEditor = new MsbEditor.TextEditorScreen(_window, _gd);

            MsbEditor.ParamBank.LoadParams(_assetLocator);
            MsbEditor.FMGBank.LoadFMGs(_assetLocator);
            MsbEditor.MtdBank.LoadMtds(_assetLocator);

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
            ImguiRenderer.OnSetupDone();

            var style = ImGui.GetStyle();
            style.TabBorderSize = 0;

            if (CFG.Current.LastProjectFile != null && CFG.Current.LastProjectFile != "")
            {
                if (File.Exists(CFG.Current.LastProjectFile))
                {
                    _projectSettings = MsbEditor.ProjectSettings.Deserialize(CFG.Current.LastProjectFile);
                    ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(CFG.Current.LastProjectFile));
                }
            }
        }

        public void Run()
        {
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

        private void ChangeProjectSettings(MsbEditor.ProjectSettings newsettings, string moddir)
        {
            _projectSettings = newsettings;
            _assetLocator.SetFromProjectSettings(newsettings, moddir);
            MsbEditor.ParamBank.ReloadParams();
            MsbEditor.FMGBank.ReloadFMGs();
            MsbEditor.MtdBank.ReloadMtds();
            MSBEditor.OnProjectChanged(_projectSettings);
            ModelEditor.OnProjectChanged(_projectSettings);
            ParamEditor.OnProjectChanged(_projectSettings);
            TextEditor.OnProjectChanged(_projectSettings);
        }

        public void ApplyStyle()
        {
            // Colors
            //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
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
        }

        public void UnapplyStyle()
        {
            ImGui.PopStyleColor(26);
            ImGui.PopStyleVar(4);
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
            if (ImGui.Begin("DockSpace_W", flags))
            {
                //Console.WriteLine("hi");
            }
            var dsid = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.NoSplit);
            ImGui.PopStyleVar(1);
            ImGui.End();

            bool newProject = false;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0.0f);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (_projectSettings == null || _projectSettings.ProjectName == null)
                    {
                        ImGui.MenuItem("No project open", false);
                    }
                    else
                    {
                        ImGui.MenuItem($@"Settings: {_projectSettings.ProjectName}");
                    }

                    if (ImGui.MenuItem("Enable Texturing (alpha)", "", CFG.Current.EnableTexturing))
                    {
                        CFG.Current.EnableTexturing = !CFG.Current.EnableTexturing;
                    }

                    if (ImGui.MenuItem("New Project", "CTRL+N") || InputTracker.GetControlShortcut(Key.I))
                    {
                        newProject = true;
                    }
                    if (ImGui.MenuItem("Open Project", ""))
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
                            _projectSettings = MsbEditor.ProjectSettings.Deserialize(browseDlg.FileName);
                            ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(browseDlg.FileName));
                            CFG.Current.LastProjectFile = browseDlg.FileName;
                            var recent = new CFG.RecentProject();
                            recent.Name = _projectSettings.ProjectName;
                            recent.GameType = _projectSettings.GameType;
                            recent.ProjectFile = browseDlg.FileName;
                            CFG.Current.RecentProjects.Insert(0, recent);
                            if (CFG.Current.RecentProjects.Count > CFG.MAX_RECENT_PROJECTS)
                            {
                                CFG.Current.RecentProjects.RemoveAt(CFG.Current.RecentProjects.Count - 1);
                            }
                        }
                    }
                    if (ImGui.BeginMenu("Recent Projects"))
                    {
                        CFG.RecentProject recent = null;
                        foreach (var p in CFG.Current.RecentProjects)
                        {
                            if (ImGui.MenuItem($@"{p.GameType.ToString()}:{p.Name}"))
                            {
                                if (File.Exists(p.ProjectFile))
                                {
                                    _projectSettings = MsbEditor.ProjectSettings.Deserialize(p.ProjectFile);
                                    ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(p.ProjectFile));
                                    recent = p;
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
                    if (ImGui.MenuItem("Save", "Ctrl-S"))
                    {
                        if (_msbEditorFocused)
                        {
                            MSBEditor.Save();
                        }
                        if (_modelEditorFocused)
                        {
                            ModelEditor.Save();
                        }
                        if (_paramEditorFocused)
                        {
                            ParamEditor.Save();
                        }
                        if (_textEditorFocused)
                        {
                            TextEditor.Save();
                        }
                    }
                    if (ImGui.MenuItem("Save All", ""))
                    {
                        MSBEditor.SaveAll();
                        ModelEditor.SaveAll();
                        ParamEditor.SaveAll();
                        TextEditor.SaveAll();
                    }
                    if (Resource.FlverResource.CaptureMaterialLayouts && ImGui.MenuItem("Dump Flver Layouts (Debug)", ""))
                    {
                        DumpFlverLayouts();
                    }
                    ImGui.EndMenu();
                }
                if (_msbEditorFocused)
                {
                    MSBEditor.DrawEditorMenu();
                }
                else if (_modelEditorFocused)
                {
                    ModelEditor.DrawEditorMenu();
                }
                else if (_paramEditorFocused)
                {
                    ParamEditor.DrawEditorMenu();
                }
                else if (_textEditorFocused)
                {
                    TextEditor.DrawEditorMenu();
                }
                ImGui.EndMainMenuBar();
            }
            ImGui.PopStyleVar();

            // New project modal
            if (newProject)
            {
                _newProjectSettings = new MsbEditor.ProjectSettings();
                _newProjectDirectory = "";
                ImGui.OpenPopup("New Project");
            }
            bool open = true;
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 7.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f));
            if (ImGui.BeginPopupModal("New Project", ref open, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Project Name:      ");
                ImGui.SameLine();
                var pname = _newProjectSettings.ProjectName;
                if (ImGui.InputText("##pname", ref pname, 255))
                {
                    _newProjectSettings.ProjectName = pname;
                }

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Project Directory: ");
                ImGui.SameLine();
                ImGui.InputText("##pdir", ref _newProjectDirectory, 255);
                ImGui.SameLine();
                if (ImGui.Button($@"{ForkAwesome.FileO}"))
                {
                    var browseDlg = new System.Windows.Forms.FolderBrowserDialog();

                    if (browseDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _newProjectDirectory = browseDlg.SelectedPath;
                    }
                }

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Game Executable:   ");
                ImGui.SameLine();
                var gname = _newProjectSettings.GameRoot;
                if (ImGui.InputText("##gdir", ref gname, 255))
                {
                    _newProjectSettings.GameRoot = gname;
                    _newProjectSettings.GameType = _assetLocator.GetGameTypeForExePath(_newProjectSettings.GameRoot);
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
                        _newProjectSettings.GameRoot = browseDlg.FileName;
                        _newProjectSettings.GameType = _assetLocator.GetGameTypeForExePath(_newProjectSettings.GameRoot);
                    }
                }
                ImGui.PopID();
                ImGui.Text($@"Detected Game:      {_newProjectSettings.GameType.ToString()}");

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();
                if (_newProjectSettings.GameType == GameType.DarkSoulsIISOTFS || _newProjectSettings.GameType == GameType.DarkSoulsIII)
                {
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($@"Use Loose Params:  ");
                    ImGui.SameLine();
                    var looseparams = _newProjectSettings.UseLooseParams;
                    if (ImGui.Checkbox("##looseparams", ref looseparams))
                    {
                        _newProjectSettings.UseLooseParams = looseparams;
                    }
                    ImGui.NewLine();
                }

                if (ImGui.Button("Create", new Vector2(120, 0)))
                {
                    bool validated = true;
                    if (_newProjectSettings.GameRoot == null || !File.Exists(_newProjectSettings.GameRoot))
                    {
                        System.Windows.Forms.MessageBox.Show("Your game executable path does not exist. Please select a valid executable.", "Error",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && _newProjectSettings.GameType == GameType.Undefined)
                    {
                        System.Windows.Forms.MessageBox.Show("Your game executable is not a valid supported game.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && (_newProjectDirectory == null || !Directory.Exists(_newProjectDirectory)))
                    {
                        System.Windows.Forms.MessageBox.Show("Your selected project directory is not valid.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && File.Exists($@"{_newProjectDirectory}\project.json"))
                    {
                        System.Windows.Forms.MessageBox.Show("Your selected project directory is already a project.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }
                    if (validated && (_newProjectSettings.ProjectName == null || _newProjectSettings.ProjectName == ""))
                    {
                        System.Windows.Forms.MessageBox.Show("You must specify a project name.", "Error",
                                         System.Windows.Forms.MessageBoxButtons.OK,
                                         System.Windows.Forms.MessageBoxIcon.None);
                        validated = false;
                    }

                    if (validated)
                    {
                        _projectSettings = _newProjectSettings;
                        _projectSettings.GameRoot = Path.GetDirectoryName(_projectSettings.GameRoot);
                        if (_projectSettings.GameType == GameType.Bloodborne)
                        {
                            _projectSettings.GameRoot = _projectSettings.GameRoot + @"\dvdroot_ps4";
                        }
                        _projectSettings.Serialize($@"{_newProjectDirectory}\project.json");
                        ChangeProjectSettings(_projectSettings, _newProjectDirectory);

                        CFG.Current.LastProjectFile = $@"{_newProjectDirectory}\project.json";
                        var recent = new CFG.RecentProject();
                        recent.Name = _projectSettings.ProjectName;
                        recent.GameType = _projectSettings.GameType;
                        recent.ProjectFile = $@"{_newProjectDirectory}\project.json";
                        CFG.Current.RecentProjects.Insert(0, recent);
                        if (CFG.Current.RecentProjects.Count > CFG.MAX_RECENT_PROJECTS)
                        {
                            CFG.Current.RecentProjects.RemoveAt(CFG.Current.RecentProjects.Count - 1);
                        }

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
            if (ImGui.Begin("Map Editor"))
            {
                ImGui.PopStyleVar(1);
                MSBEditor.OnGUI(mapcmds);
                ImGui.End();
                _msbEditorFocused = true;
                MSBEditor.Update(deltaseconds);
            }
            else
            {
                ImGui.PopStyleVar(1);
                _msbEditorFocused = false;
                ImGui.End();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            if (ImGui.Begin("Model Editor"))
            {
                ImGui.PopStyleVar(1);
                ModelEditor.OnGUI();
                _modelEditorFocused = true;
                ModelEditor.Update(deltaseconds);
            }
            else
            {
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
                ParamEditor.OnGUI(paramcmds);
                _paramEditorFocused = true;
            }
            else
            {
                _paramEditorFocused = false;
            }
            ImGui.End();

            string[] textcmds = null;
            if (commandsplit != null && commandsplit[0] == "text")
            {
                textcmds = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4));
            if (ImGui.Begin("Text Editor"))
            {
                TextEditor.OnGUI(textcmds);
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

            Resource.ResourceManager.UpdateTasks();

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
                MSBEditor.EditorResized(_window, _gd);
                ModelEditor.EditorResized(_window, _gd);
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
                MSBEditor.Draw(_gd, MainWindowCommandList);
            }
            if (_modelEditorFocused)
            {
                ModelEditor.Draw(_gd, MainWindowCommandList);
            }
            Scene.Renderer.Frame(MainWindowCommandList);
            //GuiCommandList.Begin();
            //GuiCommandList.SetFramebuffer(_gd.SwapchainFramebuffer);
            MainWindowCommandList.SetFullViewport(0);
            MainWindowCommandList.SetFullScissorRects();
            ImguiRenderer.Render(_gd, MainWindowCommandList);
            //GuiCommandList.End();
            MainWindowCommandList.End();
            _gd.SubmitCommands(MainWindowCommandList);
            //Scene.SceneRenderPipeline.TestUpdateView(_gd, MainWindowCommandList, TestWorldView.CameraTransform.CameraViewMatrix);

            _gd.SwapBuffers();
        }
    }
}
