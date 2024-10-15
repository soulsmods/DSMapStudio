using static Andre.Native.ImGuiBindings;
using Microsoft.Extensions.Logging;
using Octokit;
using Silk.NET.SDL;
using SoapstoneLib;
using SoulsFormats;
using StudioCore.Banks;
using StudioCore.Banks.AliasBank;
using StudioCore.Editor;
using StudioCore.Graphics;
using StudioCore.Help;
using StudioCore.MsbEditor;
using StudioCore.ParamEditor;
using StudioCore.Platform;
using StudioCore.Renderer.Resource;
using StudioCore.Tests;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;
using StudioCore.Editor;
using StudioCore.Utilities;
using Renderer = StudioCore.Renderer.Scene.Renderer;
using Thread = System.Threading.Thread;
using Version = System.Version;

namespace StudioCore;

public class MapStudioNew
{
    private static double _desiredFrameLengthSeconds = 1.0 / 20.0f;
    private static readonly bool _limitFrameRate = true;

    private static bool _initialLoadComplete;
    private static bool _firstframe = true;
    public static bool FirstFrame = true;

    public static bool LowRequirementsMode;

    private static IGraphicsContext _context;

    private readonly List<EditorScreen> _editors;
    private readonly HelpWindow HelpWindow;

    private readonly NewProjectOptions _newProjectOptions = new();
    private readonly string _programTitle;
    private readonly SettingsMenu _settingsMenu;

    private readonly SoapstoneService _soapstoneService;
    private readonly string _version;

    /// <summary>
    ///     Characters to load that FromSoft use, but aren't included in the ImGui Japanese glyph range.
    /// </summary>
    private const string _extraGlyphs = "⇒⑮⑭⑲⑳⑥⑤㎏⑯②①⑨⑦⑰―㌃’㌦㌧㌻㍉㍊㍍㍑㍗㍻㍼㍽㍾⑫㌶Ⅱ※⑱⑧⑩⑪㊤㊦㊧㊨●㌍⑬㌣“”•㎎←↑→↓蜘蛛牢墟壷熔吊塵屍♀彷徨徘徊吠祀…騙眷嘘穢擲罠邂逅涜扁罹脆蠢繍蝕袂鍮鴉嘴檻娼腑賤鍔囁祓縋歪躯仇啜繋痺搦壺覗咆哮夥隕蹂躙詛哭捩嘯蕩舐嗜僻裔贄抉鉈叩膂迸厭鉾誂呆跪攫滲唸躊躇∞靭棘篝㌔㌘㌢㌫③④瑕疵■頬髭痣埃窩枷戮僥濘侘噛呻怯碌懺吼縷爺餞誑邁儂儚憑糞眩瞞讐澹軛鶯瀉鋤蝋⇔磔ΩⅠ賽渠瞑蛆澱揶揄篭贖帷冑熾斃屠謳烙痒爛褪鑽矮傅虔瘴躱泄瘤蟲燻滓蝙蝠楔剥膿簒矜拗欒炸烽譚謐咬佇蜥蜴噺嵌掴僭貶朧峙棍鋲鬨薔薇滾洩髯剃Ⅲ™竄–誅掻愴鼠涎蛭蛾贔屓鎚鉤芒傀儡αβγ礫♂○鍾囮踵誹囃碍鄙賎掟娑弩蜀靄蛙轢嗟贅Ⅳ齧咎奢頚燐填鏃△□謬諌憺媚垢宸憫蝿蟇嚢─悶櫃咳狗艱倅箪淤飴梟曰仄呟吽刎鬘睨鈷屏汞翡籃蝉箒猩埒閂癪皺憚杞甕弑祟狐貉撓褄★祠廠燼衒狸酩酊殲鹵閾謗—한국어體简Руский언변경최종사용자라이선스계약개인정보처리방침데터에관동의페지넘기택실행닫하다거절變權隱擇關语变终户许协议隐游戏换页选择执关闭ęŻŃŚżźńПарметыязЛИЦЕНЗОСГАШКЧЫМЬВТФДоглшнбпьвдхЯю猜諜聘站腿恫賁戈絨毯攪倶洒掩頸懣愾啼狽捌頷轍輜儘淘餐廓撹飄坩堝屹鬣孕痾衾聳嘶疼蠅茹朦鹸閨闢竦焉斂蛹蜃孵蟻癌瘡蠍鋏讒姦仗拵跋扈鮫笏錨銛撻⟪⟫ภาษ​ไทย禿驟咥慟糺麾藉蠱奸躾吝嗇孺濾滸訝煽蛸‐恍隧臍蟷螂蜷œ„업트년월일본을신중게읽으십시오귀가당임또는서비접근나를것은술된모든조건그고참급통해부루구속되로함미합니러두않우마재항제과집단소송포있며외역주들적습독특됩세내및확유럽연호북남칭와멀티플레온면전문콘텐츠운드능련위액입판매아여대명권른품존떤식도키추혹삭공요금청차프션규칙음징될수달룹상충체결립따치같취뒤안완히준할법성령만후견야코등록넷디털못작반영컬컴퓨웨랫폼크웃됨했간점직배타양불널버별예범느허락복목분석설파셈블발생물저알렵벗회폐래텍픽진화템열산더장없족효각써무런책즉료환받량새메승필획득럼철교줄혀향칠격증익원현려져색출활걸쳐누평찬킬표검토감때론퇴훼손욕쾌란노골괴롭협박퍼학킹편밀광력뜨팸류봇뮬움순올네워끊밖었바담닙황팩질형태천애묵롯므강축쟁패큼까백민초험망멸휴벌총높테심탈병엄앞울엇닌탕겠뿐냄납긴날찍름번케팅념엔캘섭랑클카갖쿄옹탁머잔너캔르객갱컨롤짜릴홈살펴벨델브뉴밍센랭글균맞춥랍돕응답벤링좋탐률채턴웹믿옵난람떠곳낼閱讀們您處豁歐澳稱雙說內屬售會產刪絕銷另發點參當效區齡對號續數腦經據獨譯圖找碼衍檔沒滿隨仍繼兌值賺賬戶圍佔稅做聲譽歸擔查佈雖猥褻擾勵攬假僱聯亂喊寫垃圾缺礙斷幫贏證贊輕濟潛兩址份釋簽遞餘啟你窗瀏舉營估獎趨聊辨隸竊请细阅读访问们务处约这节为则适于详见亚订进并网络发线载书电该权产删确绝张贴费结销规说间冲类缔签实决过显获时经达辖龄监护须对册续连联损运设备统帐拥话转让补带况复个业编译汇试图寻码创标计范围它尝饰义软币拟档财识现满毁负责暂还购买无继视邮应紧货专门兑价际认响赚赎额赠赁员账开弃频项广传赔偿资归错误论违审储报虽辑从给诽谤伤秽亵骚扰胁滥动诈种揽输坛领导陈坏优势骗仿维难击败帮赢赛卖师东证质赞荐颁样减强竞仅测诺长严伙惩罚润轻坚众调构济诉讼团纠纷讯别组较两亲阶级针释预递题记录营赋圣县杂夺启扫忆侦浏览阐顶沟态评术奖兴闻趋检顾绍单织丢链齐异ćĘĆĄŁąłśŹЙЭУЖБЮЩХжфцчщэъЪเลือนกดํิรปีมับ่ข้ตงใหสธแผูชจุคณถึะพวญโซศฐฏ์ๆ็ฉฑฎฟฮฝฆฌฤฯฒ鎗≪≫隘髑髏";

    private EditorScreen _focusedEditor;

    private bool _programUpdateAvailable;
    private ProjectSettings _projectSettings;
    private string _releaseUrl = "";
    private bool _showImGuiDebugLogWindow;

    // ImGui Debug windows
    private bool _showImGuiDemoWindow;
    private bool _showImGuiMetricsWindow;
    private bool _showImGuiStackToolWindow;

    private bool _standardProjectUIOpened = true;

    public static EventHandler UIScaleChanged;
    public static bool FontRebuildRequest;

    public unsafe MapStudioNew(IGraphicsContext context, string version)
    {
        _version = version;
        _programTitle = $"Dark Souls Map Studio version {_version}";

        UIScaleChanged += (_, _) =>
        {
            FontRebuildRequest = true;
        };

        // Hack to make sure dialogs work before the main window is created
        PlatformUtils.InitializeWindows(null);
        CFG.AttemptLoadOrDefault();

        Environment.SetEnvironmentVariable("PATH",
            Environment.GetEnvironmentVariable("PATH") + Path.PathSeparator + "bin");

        _context = context;
        _context.Initialize();
        _context.Window.Title = _programTitle;
        PlatformUtils.InitializeWindows(context.Window.SdlWindowHandle);

        Locator.AssetLocator = new AssetLocator();

        // Banks
        ModelAliasBank.Bank = new AliasBank(AliasType.Model);
        MapAliasBank.Bank = new AliasBank(AliasType.Map);

        MsbEditorScreen msbEditor = new(_context.Window, _context.Device);
        ModelEditorScreen modelEditor = new(_context.Window, _context.Device);
        ParamEditorScreen paramEditor = new(_context.Window, _context.Device);
        TextEditorScreen textEditor = new(_context.Window, _context.Device);
        _editors = new List<EditorScreen> { msbEditor, modelEditor, paramEditor, textEditor };
        _focusedEditor = msbEditor;

        _soapstoneService = new SoapstoneService(_version, msbEditor);

        _settingsMenu = new SettingsMenu();
        _settingsMenu.MsbEditor = msbEditor;
        _settingsMenu.ModelEditor = modelEditor;
        _settingsMenu.ParamEditor = paramEditor;
        _settingsMenu.TextEditor = textEditor;

        HelpWindow = new HelpWindow();

        ImGui.GetIO()->ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        _context.ImguiRenderer.OnSetupDone();

        ImGuiStyle* style = ImGui.GetStyle();
        style->TabBorderSize = 0;

        if (CFG.Current.LastProjectFile != null && CFG.Current.LastProjectFile != "")
        {
            if (File.Exists(CFG.Current.LastProjectFile))
            {
                ProjectSettings settings = ProjectSettings.Deserialize(CFG.Current.LastProjectFile);
                if (settings == null)
                {
                    CFG.Current.LastProjectFile = "";
                    CFG.Save();
                }
                else
                {
                    try
                    {
                        AttemptLoadProject(settings, CFG.Current.LastProjectFile);
                    }
                    catch
                    {
                        CFG.Current.LastProjectFile = "";
                        CFG.Save();
                        PlatformUtils.Instance.MessageBox(
                            "Failed to load last project. Project will not be loaded after restart.",
                            "Project Load Error", MessageBoxButtons.OK);
                        throw;
                    }
                }
            }
            else
            {
                CFG.Current.LastProjectFile = "";
                CFG.Save();
                TaskLogs.AddLog($"Cannot load project: \"{CFG.Current.LastProjectFile}\" does not exist.",
                    LogLevel.Warning, TaskLogs.LogPriority.High);
            }
        }
    }

    private unsafe void SetupFonts()
    {
        ImFontAtlas* fonts = ImGui.GetIO()->Fonts;
        var fileEn = Path.Combine(AppContext.BaseDirectory, @"Assets\Fonts\RobotoMono-Light.ttf");
        var fontEn = File.ReadAllBytes(fileEn);
        var fontEnNative = new IntPtr(ImGui.MemAlloc(fontEn.Length));
        Marshal.Copy(fontEn, 0, fontEnNative, fontEn.Length);
        var fileOther = Path.Combine(AppContext.BaseDirectory, @"Assets\Fonts\NotoSansCJKtc-Light.otf");
        var fontOther = File.ReadAllBytes(fileOther);
        var fontOtherNative = new IntPtr(ImGui.MemAlloc(fontOther.Length));
        Marshal.Copy(fontOther, 0, fontOtherNative, fontOther.Length);
        var fileIcon = Path.Combine(AppContext.BaseDirectory, @"Assets\Fonts\forkawesome-webfont.ttf");
        var fontIcon = File.ReadAllBytes(fileIcon);
        var fontIconNative = new IntPtr(ImGui.MemAlloc(fontIcon.Length));
        Marshal.Copy(fontIcon, 0, fontIconNative, fontIcon.Length);
        ImFontAtlasClear(fonts);

        var scale = GetUIScale();

        // English fonts
        {
            ImFontConfig* cfg = ImFontConfigImFontConfig();
            cfg->GlyphMinAdvanceX = 5.0f;
            cfg->OversampleH = 5;
            cfg->OversampleV = 5;
            ImFontAtlasAddFontFromMemoryTTF(fonts, fontEnNative.ToPointer(), fontEn.Length, (float)Math.Round(14.0f * scale), cfg,
                ImFontAtlasGetGlyphRangesDefault(fonts));
        }

        // Other language fonts
        {
            ImFontConfig* cfg = ImFontConfigImFontConfig();
            cfg->MergeMode = true;
            cfg->GlyphMinAdvanceX = 7.0f;
            cfg->OversampleH = 5;
            cfg->OversampleV = 5;

            ImFontGlyphRangesBuilder* glyphRanges = ImFontGlyphRangesBuilderImFontGlyphRangesBuilder();
            ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesJapanese(fonts));
            foreach (var c in _extraGlyphs)
            {
                ImFontGlyphRangesBuilderAddChar(glyphRanges, c);
            }

            if (CFG.Current.FontChinese)
            {
                ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesChineseFull(fonts));
            }

            if (CFG.Current.FontKorean)
            {
                ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesKorean(fonts));
            }

            if (CFG.Current.FontThai)
            {
                ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesThai(fonts));
            }

            if (CFG.Current.FontVietnamese)
            {
                ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesVietnamese(fonts));
            }

            if (CFG.Current.FontCyrillic)
            {
                ImFontGlyphRangesBuilderAddRanges(glyphRanges, ImFontAtlasGetGlyphRangesCyrillic(fonts));
            }

            ImVectorImWchar glyphRange;
            ImFontGlyphRangesBuilderBuildRanges(glyphRanges, &glyphRange);
            ImFontAtlasAddFontFromMemoryTTF(fonts, fontOtherNative.ToPointer(), fontOther.Length, 16.0f * scale, cfg, glyphRange.Data);
            ImFontGlyphRangesBuilderDestroy(glyphRanges);
        }

        // Icon fonts
        {
            ushort[] ranges = { ForkAwesome.IconMin, ForkAwesome.IconMax, 0 };
            ImFontConfig* cfg = ImFontConfigImFontConfig();
            cfg->MergeMode = true;
            cfg->GlyphMinAdvanceX = 12.0f;
            cfg->OversampleH = 5;
            cfg->OversampleV = 5;

            fixed (ushort* r = ranges)
            {
                ImFontAtlasAddFontFromMemoryTTF(fonts, fontIconNative.ToPointer(), fontIcon.Length, 16.0f * scale, cfg, r);
            }
        }

        _context.ImguiRenderer.RecreateFontDeviceTexture();
    }

    public void SetupCSharpDefaults()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private void CheckProgramUpdate()
    {
        GitHubClient gitHubClient = new(new ProductHeaderValue("DSMapStudio"));
        Release release = gitHubClient.Repository.Release.GetLatest("soulsmods", "DSMapStudio").Result;
        var isVer = false;
        var verstring = "";
        foreach (var c in release.TagName)
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
            {
                File.Copy("imgui.ini.backup", "imgui.ini");
            }
        }
        else if (!File.Exists("imgui.ini.backup"))
        {
            if (File.Exists("imgui.ini"))
            {
                File.Copy("imgui.ini", "imgui.ini.backup");
            }
        }
    }

    public void Run()
    {
        SetupCSharpDefaults();
        ManageImGuiConfigBackups();

        if (CFG.Current.EnableSoapstone)
        {
            TaskManager.RunPassiveTask(new TaskManager.LiveTask("Soapstone Server",
                TaskManager.RequeueType.None, true,
                () => SoapstoneServer.RunAsync(KnownServer.DSMapStudio, _soapstoneService).Wait()));
        }

        if (CFG.Current.EnableCheckProgramUpdate)
        {
            TaskManager.Run(new TaskManager.LiveTask("Check Program Updates",
                TaskManager.RequeueType.None, true,
                () => CheckProgramUpdate()));
        }

        long previousFrameTicks = 0;
        Stopwatch sw = new();
        sw.Start();
        Tracy.Startup();
        while (_context.Window.Exists)
        {
            Tracy.TracyCFrameMark();

            // Limit frame rate when window isn't focused unless we are profiling
            var focused = Tracy.EnableTracy ? true : _context.Window.Focused;

            if (!focused)
            {
                _desiredFrameLengthSeconds = 1.0 / CFG.Current.GFX_Framerate_Limit_Unfocused;
            }
            else
            {
                _desiredFrameLengthSeconds = 1.0 / CFG.Current.GFX_Framerate_Limit;
            }

            var currentFrameTicks = sw.ElapsedTicks;
            var deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

            Tracy.___tracy_c_zone_context ctx = Tracy.TracyCZoneNC(1, "Sleep", 0xFF0000FF);
            while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds)
            {
                currentFrameTicks = sw.ElapsedTicks;
                deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
                Thread.Sleep(focused ? 0 : 1);
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

            if (true) //_window.Focused)
            {
                ctx = Tracy.TracyCZoneNC(1, "Draw", 0xFFFF0000);
                _context.Draw(_editors, _focusedEditor);
                Tracy.TracyCZoneEnd(ctx);
            }
            else
            {
                // Flush the background queues
                Renderer.Scene.Renderer.Frame(null, true);
            }
        }

        //DestroyAllObjects();
        Tracy.Shutdown();
        ResourceManager.Shutdown();
        _context.Dispose();
        CFG.Save();
    }

    // Try to shutdown things gracefully on a crash
    public void CrashShutdown()
    {
        Tracy.Shutdown();
        ResourceManager.Shutdown();
        _context.Dispose();
    }

    private void ChangeProjectSettings(ProjectSettings newsettings, string moddir, NewProjectOptions options)
    {
        _projectSettings = newsettings;
        Locator.ActiveProject = new Project(newsettings, moddir);
        _settingsMenu.ProjSettings = _projectSettings;

        // Banks
        ModelAliasBank.Bank.ReloadAliasBank();
        MapAliasBank.Bank.ReloadAliasBank();

        MtdBank.ReloadMtds();

        foreach (EditorScreen editor in _editors)
        {
            editor.OnProjectChanged(_projectSettings);
            editor.Load(Locator.ActiveProject);
        }

        
    }

    public unsafe void ApplyStyle()
    {
        var scale = GetUIScale();
        ImGuiStyle *style = ImGui.GetStyle();

        // Colors
        ImGui.PushStyleColorVec4(ImGuiCol.WindowBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        //ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.PopupBg, new Vector4(0.106f, 0.106f, 0.110f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.Border, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.FrameBg, new Vector4(0.200f, 0.200f, 0.216f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.FrameBgHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.FrameBgActive, new Vector4(0.200f, 0.200f, 0.216f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TitleBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TitleBgActive, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.MenuBarBg, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ScrollbarBg, new Vector4(0.243f, 0.243f, 0.249f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ScrollbarGrab, new Vector4(0.408f, 0.408f, 0.408f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ScrollbarGrabHovered, new Vector4(0.635f, 0.635f, 0.635f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ScrollbarGrabActive, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.CheckMark, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.SliderGrab, new Vector4(0.635f, 0.635f, 0.635f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.SliderGrabActive, new Vector4(1.000f, 1.000f, 1.000f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.Button, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ButtonHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.ButtonActive, new Vector4(0.200f, 0.600f, 1.000f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.Header, new Vector4(0.000f, 0.478f, 0.800f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.HeaderHovered, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.HeaderActive, new Vector4(0.161f, 0.550f, 0.939f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.Tab, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TabHovered, new Vector4(0.110f, 0.592f, 0.918f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TabActive, new Vector4(0.200f, 0.600f, 1.000f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TabUnfocused, new Vector4(0.176f, 0.176f, 0.188f, 1.0f));
        ImGui.PushStyleColorVec4(ImGuiCol.TabUnfocusedActive, new Vector4(0.247f, 0.247f, 0.275f, 1.0f));

        // Sizes
        ImGui.PushStyleVarFloat(ImGuiStyleVar.FrameBorderSize, 1.0f);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.TabRounding, 0.0f);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.ScrollbarRounding, 0.0f);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.ScrollbarSize, 16.0f * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowMinSize, new Vector2(100f, 100f) * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.FramePadding, style->FramePadding * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.CellPadding, style->CellPadding * scale);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.IndentSpacing, style->IndentSpacing * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemSpacing, style->ItemSpacing * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemInnerSpacing, style->ItemInnerSpacing * scale);
    }

    public void UnapplyStyle()
    {
        ImGui.PopStyleColor(27);
        ImGui.PopStyleVar(10);
    }

    private void DumpFlverLayouts()
    {
        if (PlatformUtils.Instance.SaveFileDialog("Save Flver layout dump", new[] { AssetUtils.TxtFilter },
                out var path))
        {
            using (StreamWriter file = new(path))
            {
                foreach (KeyValuePair<string, FLVER2.BufferLayout> mat in FlverResource.MaterialLayouts)
                {
                    file.WriteLine(mat.Key + ":");
                    foreach (FLVER.LayoutMember member in mat.Value)
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
            TaskLogs.AddLog(
                $"The files for {gameType} do not appear to be unpacked. Please use UDSFM for DS1:PTDE and UXM for DS2 to unpack game files",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return false;
        }

        TaskLogs.AddLog(
            $"The files for {gameType} do not appear to be fully unpacked. Functionality will be limited. Please use UXM selective unpacker to unpack game files",
            LogLevel.Warning);
        return true;
    }

    private bool AttemptLoadProject(ProjectSettings settings, string filename, NewProjectOptions options = null)
    {
        var success = true;
        // Check if game exe exists
        if (!Directory.Exists(settings.GameRoot))
        {
            success = false;
            PlatformUtils.Instance.MessageBox(
                $@"Could not find game data directory for {settings.GameType}. Please select the game executable.",
                "Error",
                MessageBoxButtons.OK);

            while (true)
            {
                if (PlatformUtils.Instance.OpenFileDialog(
                        $"Select executable for {settings.GameType}...",
                        new[] { AssetUtils.GameExecutableFilter },
                        out var path))
                {
                    settings.GameRoot = path;
                    GameType gametype = AssetUtils.GetGameTypeForExePath(settings.GameRoot);
                    if (gametype == settings.GameType)
                    {
                        success = true;
                        settings.GameRoot = Path.GetDirectoryName(settings.GameRoot);
                        if (settings.GameType == GameType.Bloodborne)
                        {
                            settings.GameRoot += @"\dvdroot_ps4";
                        }

                        settings.Serialize(filename);
                        break;
                    }

                    PlatformUtils.Instance.MessageBox(
                        $@"Selected executable was not for {settings.GameType}. Please select the correct game executable.",
                        "Error",
                        MessageBoxButtons.OK);
                }
                else
                {
                    break;
                }
            }
        }

        if (success)
        {
            if (!AssetUtils.CheckFilesExpanded(settings.GameRoot, settings.GameType))
            {
                if (!GameNotUnpackedWarning(settings.GameType))
                {
                    return false;
                }
            }

            if (settings.GameType == GameType.Sekiro || settings.GameType == GameType.EldenRing)
            {
                if (!StealGameDllIfMissing(settings, "oo2core_6_win64"))
                {
                    return false;
                }
            }
            else if (settings.GameType == GameType.ArmoredCoreVI)
            {
                if (!StealGameDllIfMissing(settings, "oo2core_8_win64"))
                {
                    return false;
                }
            }

            _projectSettings = settings;
            ChangeProjectSettings(_projectSettings, Path.GetDirectoryName(filename), options);
            _context.Window.Title = $"{_programTitle}  -  {_projectSettings.ProjectName}";

            CFG.RecentProject recent = new()
            {
                Name = _projectSettings.ProjectName,
                GameType = _projectSettings.GameType,
                ProjectFile = filename
            };
            CFG.AddMostRecentProject(recent);
        }

        return success;
    }

    private bool StealGameDllIfMissing(ProjectSettings settings, string dllName)
    {
        dllName = dllName + ".dll";
        if (File.Exists(Path.Join(Path.GetFullPath("."), dllName)))
        {
            return true;
        }

        if (!File.Exists(Path.Join(settings.GameRoot, dllName)))
        {
            PlatformUtils.Instance.MessageBox(
                $"Could not find file \"{dllName}\" in \"{settings.GameRoot}\", which should be included by default.\n\nTry verifying or reinstalling the game.",
                "Error",
                MessageBoxButtons.OK);
            return false;
        }

        File.Copy(Path.Join(settings.GameRoot, dllName), Path.Join(Path.GetFullPath("."), dllName));
        return true;
    }

    //Unhappy with this being here
    [DllImport("user32.dll", EntryPoint = "ShowWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _user32_ShowWindow(IntPtr hWnd, int nCmdShow);

    public void SaveAll()
    {
        if (_projectSettings != null && _projectSettings.ProjectName != null)
        {
            // Danger zone assuming on lastProjectFile
            _projectSettings.Serialize(CFG.Current.LastProjectFile);
        }
        foreach (EditorScreen editor in _editors)
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
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Unable to save config during crash recovery.\n" +
                                                  $"If you continue to crash on startup, delete config in AppData\\Local\\DSMapStudio\n\n" +
                                                  $"{e.Message} {e.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        var success = Locator.AssetLocator.CreateRecoveryProject();
        if (success)
        {
            SaveAll();
            PlatformUtils.Instance.MessageBox(
                $"Attempted to save project files to {Locator.AssetLocator.GameModDirectory} for manual recovery.\n" +
                "You must manually replace your project files with these recovery files should you wish to restore them.\n" +
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

    private unsafe void NewProject_NameGUI()
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
            if (PlatformUtils.Instance.OpenFolderDialog("Select project directory...", out var path))
            {
                _newProjectOptions.directory = path;
            }
        }
    }

    private void NewProject_GameTypeComboGUI()
    {
        ImGui.AlignTextToFramePadding();
        ImGui.Text(@"Game Type:         ");
        ImGui.SameLine();
        var games = Enum.GetNames(typeof(GameType));
        var gameIndex = Array.IndexOf(games, _newProjectOptions.settings.GameType.ToString());
        if (ImGui.Combo("##GameTypeCombo", ref gameIndex, games, games.Length))
        {
            _newProjectOptions.settings.GameType = Enum.Parse<GameType>(games[gameIndex]);
        }
    }

    private unsafe void Update(float deltaseconds)
    {
        Tracy.___tracy_c_zone_context ctx = Tracy.TracyCZoneN(1, "Imgui");

        UpdateDpi();
        var scale = GetUIScale();

        if (FontRebuildRequest)
        {
            _context.ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, SetupFonts);
            FontRebuildRequest = false;
        }
        else
        {
            _context.ImguiRenderer.Update(deltaseconds, InputTracker.FrameSnapshot, null);
        }

        Tracy.TracyCZoneEnd(ctx);
        TaskManager.ThrowTaskExceptions();

        var commandsplit = EditorCommandQueue.GetNextCommand();
        if (commandsplit != null && commandsplit[0] == "windowFocus")
        {
            //this is a hack, cannot grab focus except for when un-minimising
            _user32_ShowWindow(_context.Window.Handle, 6);
            _user32_ShowWindow(_context.Window.Handle, 9);
        }

        ctx = Tracy.TracyCZoneN(1, "Style");
        ApplyStyle();
        ImGuiViewport* vp = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(vp->Pos);
        ImGui.SetNextWindowSize(vp->Size);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
        ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                                 ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        flags |= ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.MenuBar;
        flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        flags |= ImGuiWindowFlags.NoBackground;
        ImGui.PushStyleColorVec4(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        if (ImGui.Begin("DockSpace_W", flags))
        {
        }

        var dsid = ImGui.GetID("DockSpace");
        ImGui.DockSpace(dsid, new Vector2(0, 0), ImGuiDockNodeFlags.NoDockingSplit, null);
        ImGui.PopStyleVar(1);
        ImGui.End();
        ImGui.PopStyleColor(1);
        Tracy.TracyCZoneEnd(ctx);

        ctx = Tracy.TracyCZoneN(1, "Menu");
        var newProject = false;
        ImGui.PushStyleVarFloat(ImGuiStyleVar.FrameBorderSize, 0.0f);

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {

                if (ImGui.MenuItem("Enable Texturing (alpha)", "", CFG.Current.EnableTexturing))
                {
                    CFG.Current.EnableTexturing = !CFG.Current.EnableTexturing;
                }

                if (ImGui.MenuItem("New Project", "", false, !TaskManager.AnyActiveTasks()))
                {
                    newProject = true;
                }

                if (ImGui.MenuItem("Open Project", "", false, !TaskManager.AnyActiveTasks()))
                {
                    if (PlatformUtils.Instance.OpenFileDialog(
                            "Choose the project json file",
                            new[] { AssetUtils.ProjectJsonFilter },
                            out var path))
                    {
                        ProjectSettings settings = ProjectSettings.Deserialize(path);
                        if (settings != null)
                        {
                            AttemptLoadProject(settings, path);
                        }
                    }
                }

                if (ImGui.BeginMenu("Recent Projects",
                        !TaskManager.AnyActiveTasks() && CFG.Current.RecentProjects.Count > 0))
                {
                    CFG.RecentProject recent = null;
                    var id = 0;
                    foreach (CFG.RecentProject p in CFG.Current.RecentProjects.ToArray())
                    {
                        if (ImGui.MenuItem($@"{p.GameType}: {p.Name}##{id}"))
                        {
                            if (File.Exists(p.ProjectFile))
                            {
                                ProjectSettings settings = ProjectSettings.Deserialize(p.ProjectFile);
                                if (settings != null)
                                {
                                    if (AttemptLoadProject(settings, p.ProjectFile))
                                    {
                                        recent = p;
                                    }
                                }
                            }
                            else
                            {
                                DialogResult result = PlatformUtils.Instance.MessageBox(
                                    $"Project file at \"{p.ProjectFile}\" does not exist.\n\n" +
                                    $"Remove project from list of recent projects?",
                                    $"Project.json cannot be found", MessageBoxButtons.YesNo);
                                if (result == DialogResult.Yes)
                                {
                                    CFG.RemoveRecentProject(p);
                                }
                            }
                        }

                        if (ImGui.BeginPopupContextItem())
                        {
                            if (ImGui.Selectable("Remove from list"))
                            {
                                CFG.RemoveRecentProject(p);
                            }

                            ImGui.EndPopup();
                        }

                        id++;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Open in Explorer",
                        !TaskManager.AnyActiveTasks() && CFG.Current.RecentProjects.Count > 0))
                {
                    if (ImGui.MenuItem("Open Project Folder", "", false, !TaskManager.AnyActiveTasks()))
                    {
                        var projectPath = Locator.AssetLocator.GameModDirectory;
                        Process.Start("explorer.exe", projectPath);
                    }

                    if (ImGui.MenuItem("Open Game Folder", "", false, !TaskManager.AnyActiveTasks()))
                    {
                        var gamePath = Locator.AssetLocator.GameRootDirectory;
                        Process.Start("explorer.exe", gamePath);
                    }

                    if (ImGui.MenuItem("Open Config Folder", "", false, !TaskManager.AnyActiveTasks()))
                    {
                        var configPath = CFG.GetConfigFolderPath();
                        Process.Start("explorer.exe", configPath);
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem($"Save {_focusedEditor.SaveType}",
                        KeyBindings.Current.Core_SaveCurrentEditor.HintText))
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

                if (FlverResource.CaptureMaterialLayouts && ImGui.MenuItem("Dump Flver Layouts (Debug)", ""))
                {
                    DumpFlverLayouts();
                }

                ImGui.EndMenu();
            }

            _focusedEditor.DrawEditorMenu();

            if (ImGui.BeginMenu("Help"))
            {
                if (ImGui.MenuItem("Help Window", KeyBindings.Current.Core_HelpMenu.HintText))
                {
                    HelpWindow.ToggleMenuVisibility();
                }

                ImGui.EndMenu();
            }

            if (FeatureFlags.TestMenu)
            {
                if (ImGui.BeginMenu("Tests"))
                {
                    if (ImGui.MenuItem("Paramdef Validation"))
                    {
                        ParamValidationTool.ValidateParamdef();
                    }
                    if (ImGui.MenuItem("Param Validation"))
                    {
                        ParamValidationTool.ValidatePadding();
                    }
                    if (ImGui.MenuItem("Map Validation"))
                    {
                        MapValidationTest.ValidateMSB();
                    }

                    if (ImGui.MenuItem("Crash me (will actually crash)"))
                    {
                        var badArray = new int[2];
                        var crash = badArray[5];
                    }

                    if (ImGui.MenuItem("MSBE read/write test"))
                    {
                        MSBReadWrite.Run(Locator.AssetLocator);
                    }

                    if (ImGui.MenuItem("MSB_AC6 Read/Write Test"))
                    {
                        MSB_AC6_Read_Write.Run(Locator.AssetLocator);
                    }

                    if (ImGui.MenuItem("BTL read/write test"))
                    {
                        BTLReadWrite.Run(Locator.AssetLocator);
                    }

                    if (ImGui.MenuItem("Insert unique rows IDs into params"))
                    {
                        ParamUniqueRowFinder.Run();
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
                ImGui.PushStyleColorVec4(ImGuiCol.Text, new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
                if (ImGui.Button("Update Available"))
                {
                    Process myProcess = new();
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = _releaseUrl;
                    myProcess.Start();
                }

                ImGui.PopStyleColor(1);
            }

            if (ImGui.BeginMenu("Tasks", TaskManager.GetLiveThreads().Count > 0))
            {
                foreach (var task in TaskManager.GetLiveThreads())
                {
                    ImGui.Text(task);
                }

                ImGui.EndMenu();
            }

            TaskLogs.Display();

            ImGui.EndMainMenuBar();
        }

        _settingsMenu.Display(_editors);
        HelpWindow.Display();

        ImGui.PopStyleVar(1);
        Tracy.TracyCZoneEnd(ctx);

        var open = true;
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowRounding, 7.0f);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowBorderSize, 1.0f);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f) * scale);

        // ImGui Debug windows
        if (_showImGuiDemoWindow)
        {
            ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
        }

        if (_showImGuiMetricsWindow)
        {
            ImGui.ShowMetricsWindow(ref _showImGuiMetricsWindow);
        }

        if (_showImGuiDebugLogWindow)
        {
            ImGui.ShowDebugLogWindow(ref _showImGuiDebugLogWindow);
        }

        if (_showImGuiStackToolWindow)
        {
            ImGui.ShowIDStackToolWindow(ref _showImGuiStackToolWindow);
        }

        // New project modal
        if (newProject)
        {
            _newProjectOptions.settings = new ProjectSettings();
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
                {
                    _newProjectOptions.settings.GameType = GameType.Undefined;
                }

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
                    {
                        _newProjectOptions.settings.GameRoot = Path.GetDirectoryName(gname);
                    }
                    else
                    {
                        _newProjectOptions.settings.GameRoot = gname;
                    }

                    _newProjectOptions.settings.GameType = AssetUtils.GetGameTypeForExePath(gname);

                    if (_newProjectOptions.settings.GameType == GameType.Bloodborne)
                    {
                        _newProjectOptions.settings.GameRoot += @"\dvdroot_ps4";
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button($@"{ForkAwesome.FileO}##fd2"))
                {
                    if (PlatformUtils.Instance.OpenFileDialog(
                            "Select executable for the game you want to mod...",
                            new[] { AssetUtils.GameExecutableFilter },
                            out var path))
                    {
                        _newProjectOptions.settings.GameRoot = Path.GetDirectoryName(path);
                        _newProjectOptions.settings.GameType = AssetUtils.GetGameTypeForExePath(path);

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
                    if (PlatformUtils.Instance.OpenFolderDialog("Select project directory...", out var path))
                    {
                        _newProjectOptions.settings.GameRoot = path;
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
                ImGui.Text(@"Loose Params:      ");
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
            else if (_newProjectOptions.settings.GameType is GameType.ArmoredCoreVI)
            {
                //TODO AC6
            }

            ImGui.NewLine();

            ImGui.AlignTextToFramePadding();
            ImGui.Text(@"Import row names:  ");
            ImGui.SameLine();
            Utils.ImGuiGenericHelpPopup("?", "##Help_ImportRowNames",
                "Default: ON\nImports and applies row names from lists stored in Assets folder.\nRow names can be imported at any time in the param editor's Edit menu.");
            ImGui.SameLine();
            ImGui.Checkbox("##loadDefaultNames", ref _newProjectOptions.loadDefaultNames);
            ImGui.NewLine();

            if (_newProjectOptions.settings.GameType == GameType.Undefined)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.Button("Create", new Vector2(120, 0) * scale))
            {
                var validated = true;
                if (_newProjectOptions.settings.GameRoot == null ||
                    !Directory.Exists(_newProjectOptions.settings.GameRoot))
                {
                    PlatformUtils.Instance.MessageBox(
                        "Your game executable path does not exist. Please select a valid executable.", "Error",
                        MessageBoxButtons.OK);
                    validated = false;
                }

                if (validated && _newProjectOptions.settings.GameType == GameType.Undefined)
                {
                    PlatformUtils.Instance.MessageBox("Your game executable is not a valid supported game.",
                        "Error",
                        MessageBoxButtons.OK);
                    validated = false;
                }

                if (validated && (_newProjectOptions.directory == null ||
                                  !Directory.Exists(_newProjectOptions.directory)))
                {
                    PlatformUtils.Instance.MessageBox("Your selected project directory is not valid.", "Error",
                        MessageBoxButtons.OK);
                    validated = false;
                }

                if (validated && File.Exists($@"{_newProjectOptions.directory}\project.json"))
                {
                    DialogResult message = PlatformUtils.Instance.MessageBox(
                        "Your selected project directory already contains a project.json. Would you like to replace it?",
                        "Error",
                        MessageBoxButtons.YesNo);
                    if (message == DialogResult.No)
                    {
                        validated = false;
                    }
                }

                if (validated && _newProjectOptions.settings.GameRoot == _newProjectOptions.directory)
                {
                    DialogResult message = PlatformUtils.Instance.MessageBox(
                        "Project Directory is the same as Game Directory, which allows game files to be overwritten directly.\n\n" +
                        "It's highly recommended you use the Mod Engine mod folder as your project folder instead (if possible).\n\n" +
                        "Continue and create project anyway?", "Caution",
                        MessageBoxButtons.OKCancel);
                    if (message != DialogResult.OK)
                    {
                        validated = false;
                    }
                }

                if (validated && string.IsNullOrEmpty(_newProjectOptions.settings.ProjectName))
                {
                    PlatformUtils.Instance.MessageBox("You must specify a project name.", "Error",
                        MessageBoxButtons.OK);
                    validated = false;
                }

                var gameroot = _newProjectOptions.settings.GameRoot;
                if (!AssetUtils.CheckFilesExpanded(gameroot, _newProjectOptions.settings.GameType))
                {
                    if (!GameNotUnpackedWarning(_newProjectOptions.settings.GameType))
                    {
                        validated = false;
                    }
                }

                if (validated)
                {
                    _newProjectOptions.settings.GameRoot = gameroot;
                    _newProjectOptions.settings.Serialize($@"{_newProjectOptions.directory}\project.json");
                    AttemptLoadProject(_newProjectOptions.settings, $@"{_newProjectOptions.directory}\project.json",
                        _newProjectOptions);

                    ImGui.CloseCurrentPopup();
                }
            }

            if (_newProjectOptions.settings.GameType == GameType.Undefined)
            {
                ImGui.EndDisabled();
            }

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
        foreach (EditorScreen editor in _editors)
        {
            string[] commands = null;
            if (commandsplit != null && commandsplit[0] == editor.CommandEndpoint)
            {
                commands = commandsplit.Skip(1).ToArray();
                ImGui.SetNextWindowFocus();
            }

            if (_context.Device == null)
            {
                ImGui.PushStyleColorVec4(ImGuiCol.WindowBg, *ImGui.GetStyleColorVec4(ImGuiCol.WindowBg));
            }
            else
            {
                ImGui.PushStyleColorVec4(ImGuiCol.WindowBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            }

            ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
            if (ImGui.Begin(editor.EditorName))
            {
                ImGui.PopStyleColor(1);
                ImGui.PopStyleVar(1);
                if (editor.IsEnabled(Locator.ActiveProject))
                    editor.OnGUI(commands);
                else
                    ImGui.Text("Resources required for editor not loaded.");
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
            {
                SaveFocusedEditor();
            }

            if (InputTracker.GetKeyDown(KeyBindings.Current.Core_SaveAllEditors))
            {
                SaveAll();
            }

            if (InputTracker.GetKeyDown(KeyBindings.Current.Core_HelpMenu))
            {
                HelpWindow.ToggleMenuVisibility();
            }
        }

        ImGui.PopStyleVar(2);
        UnapplyStyle();
        Tracy.TracyCZoneEnd(ctx);

        ctx = Tracy.TracyCZoneN(1, "Resource");
        ResourceManager.UpdateTasks();
        Tracy.TracyCZoneEnd(ctx);

        if (!_initialLoadComplete)
        {
            if (!TaskManager.AnyActiveTasks())
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

    private const float DefaultDpi = 96f;
    private static float _dpi = DefaultDpi;

    public static float Dpi
    {
        get => _dpi;
        set
        {
            if (Math.Abs(_dpi - value) < 0.0001f) return; // Skip doing anything if no difference

            _dpi = value;
            if (CFG.Current.UIScaleByDPI)
                UIScaleChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    private static unsafe void UpdateDpi()
    {
        if (SdlProvider.SDL.IsValueCreated && _context?.Window != null)
        {
            var window = _context.Window.SdlWindowHandle;
            int index = SdlProvider.SDL.Value.GetWindowDisplayIndex(window);
            float ddpi = 96f;
            float _ = 0f;
            SdlProvider.SDL.Value.GetDisplayDPI(index, ref ddpi, ref _, ref _);

            Dpi = ddpi;
        }
    }

    public static float GetUIScale()
    {
        var scale = CFG.Current.UIScale;
        if (CFG.Current.UIScaleByDPI)
            scale = scale / DefaultDpi * Dpi;
        return scale;
    }
}
