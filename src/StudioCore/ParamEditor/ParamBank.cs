using Andre.Formats;
using Microsoft.Extensions.Logging;
using Octokit;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.Platform;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudioCore.ParamEditor;

/// <summary>
///     Utilities for dealing with global params for a game
/// </summary>
public class ParamBank
{
    public enum ParamUpgradeResult
    {
        Success = 0,
        RowConflictsFound = -1,
        OldRegulationNotFound = -2,
        OldRegulationVersionMismatch = -3,
        OldRegulationMatchesCurrent = -4
    }

    public enum RowGetType
    {
        AllRows = 0,
        ModifiedRows = 1,
        SelectedRows = 2
    }

    public static ParamBank PrimaryBank => Locator.ActiveProject.ParamBank;
    public static ParamBank VanillaBank => Locator.ActiveProject.ParentProject.ParamBank;
    public static Dictionary<string, ParamBank> AuxBanks = new();

    /// <summary>
    ///     Mapping from path -> PARAMDEF for cache and and comparison purposes. TODO: check for paramdef comparisons and evaluate if the file/paramdef was actually the same.
    /// </summary>
    private static readonly Dictionary<string, PARAMDEF> _paramdefsCache = new();


    public static string ClipboardParam = null;
    public static List<Param.Row> ClipboardRows = new();

    /// <summary>
    ///     Mapping from ParamType -> PARAMDEF.
    /// </summary>
    private Dictionary<string, PARAMDEF> _paramdefs = new();

    //TODO private this
    public Dictionary<PARAMDEF, ParamMetaData> ParamMetas = new();


    /// <summary>
    ///     Mapping from Param filename -> Manual ParamType.
    ///     This is for params with no usable ParamType at some particular game version.
    ///     By convention, ParamTypes ending in "_TENTATIVE" do not have official data to reference.
    /// </summary>
    private Dictionary<string, string> _tentativeParamType;

    /// <summary>
    ///     Map related params.
    /// </summary>
    public static readonly List<string> DS2MapParamlist = new()
    {
        "demopointlight",
        "demospotlight",
        "eventlocation",
        "eventparam",
        "GeneralLocationEventParam",
        "generatorparam",
        "generatorregistparam",
        "generatorlocation",
        "generatordbglocation",
        "hitgroupparam",
        "intrudepointparam",
        "mapobjectinstanceparam",
        "maptargetdirparam",
        "npctalkparam",
        "treasureboxparam"
    };

    /// <summary>
    ///     Param name - FMGCategory map
    /// </summary>
    public static readonly List<(string, FmgEntryCategory)> ParamToFmgCategoryList = new()
    {
        ("EquipParamAccessory", FmgEntryCategory.Rings),
        ("EquipParamGoods", FmgEntryCategory.Goods),
        ("EquipParamWeapon", FmgEntryCategory.Weapons),
        ("EquipParamProtector", FmgEntryCategory.Armor),
        ("Magic", FmgEntryCategory.Spells),
        ("EquipParamGem", FmgEntryCategory.Gem),
        ("SwordArtsParam", FmgEntryCategory.SwordArts),
        ("EquipParamGenerator", FmgEntryCategory.Generator),
        ("EquipParamFcs", FmgEntryCategory.FCS),
        ("EquipParamBooster", FmgEntryCategory.Booster),
        ("ArchiveParam", FmgEntryCategory.Archive),
        ("MissionParam", FmgEntryCategory.Mission)
    };

    private static readonly HashSet<int> EMPTYSET = new();

    public Project Project;

    private Dictionary<string, Param> _params;

    private ulong _paramVersion;

    private bool _pendingUpgrade;
    private Dictionary<string, HashSet<int>> _primaryDiffCache; //If param != primaryparam
    private Dictionary<string, List<string?>> _storedStrippedRowNames;

    /// <summary>
    ///     Dictionary of param file names that were given a tentative ParamType, and the original ParamType it had.
    ///     Used to later restore original ParamType on write (if possible).
    /// </summary>
    private Dictionary<string, string?> _usedTentativeParamTypes;

    private Dictionary<string, HashSet<int>> _vanillaDiffCache; //If param != vanillaparam

    private Param EnemyParam => _params["EnemyParam"];

    public bool IsDefsLoaded { get; private set; }
    public static bool IsMetaLoaded { get; private set; }
    public bool IsLoadingParams { get; private set; }

    public IReadOnlyDictionary<string, Param> Params
    {
        get
        {
            if (IsLoadingParams)
            {
                return null;
            }

            return _params;
        }
    }

    public ulong ParamVersion => _paramVersion;

    public IReadOnlyDictionary<string, HashSet<int>> VanillaDiffCache
    {
        get
        {
            if (IsLoadingParams)
            {
                return null;
            }

            {
                if (VanillaBank == this)
                {
                    return null;
                }
            }
            return _vanillaDiffCache;
        }
    }

    public IReadOnlyDictionary<string, HashSet<int>> PrimaryDiffCache
    {
        get
        {
            if (IsLoadingParams)
            {
                return null;
            }

            {
                if (PrimaryBank == this)
                {
                    return null;
                }
            }
            return _primaryDiffCache;
        }
    }

    public ParamBank(Project owner)
    {
        Project = owner;
    }

    public Dictionary<string, PARAMDEF> GetParamDefs()
    {
        return _paramdefs;
    }

    private static FileNotFoundException CreateParamMissingException(GameType type)
    {
        if (type is GameType.DarkSoulsPTDE or GameType.Sekiro)
        {
            return new FileNotFoundException(
                $"Cannot locate param files for {type}.\nThis game must be unpacked before modding, please use UXM Selective Unpacker.");
        }

        if (type is GameType.DemonsSouls or GameType.Bloodborne)
        {
            return new FileNotFoundException(
                $"Cannot locate param files for {type}.\nYour game folder may be missing game files.");
        }

        return new FileNotFoundException(
            $"Cannot locate param files for {type}.\nYour game folder may be missing game files, please verify game files through steam to restore them.");
    }

    private List<(string, PARAMDEF)> LoadParamdefs()
    {
        _paramdefs = new Dictionary<string, PARAMDEF>();
        _tentativeParamType = new Dictionary<string, string>();
        var files = Project.AssetLocator.GetAllProjectFiles($@"Paramdex\{AssetUtils.GetGameIDForDir(Locator.ActiveProject.Type)}\Defs", ["*.xml"], true, false);
        List<(string, PARAMDEF)> defPairs = new();
        foreach (var f in files)
        {
            if (!_paramdefsCache.TryGetValue(f, out PARAMDEF pdef))
            {
                pdef = PARAMDEF.XmlDeserialize(f, true);
            } 
            _paramdefs.Add(pdef.ParamType, pdef);
            defPairs.Add((f, pdef));
        }

        var tentativeMappingPath = Project.AssetLocator.GetProjectFilePath($@"{Project.AssetLocator.GetParamdexDir()}\Defs\TentativeParamType.csv");
        if (File.Exists(tentativeMappingPath))
        {
            // No proper CSV library is used currently, and all CSV parsing is in the context of param files.
            // If a CSV library is introduced in DSMapStudio, use it here.
            foreach (var line in File.ReadAllLines(tentativeMappingPath).Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                {
                    throw new FormatException($"Malformed line in {tentativeMappingPath}: {line}");
                }

                _tentativeParamType[parts[0]] = parts[1];
            }
        }

        return defPairs;
    }
    public void LoadParamMeta(List<(string, PARAMDEF)> defPairs)
    {
        //This way of tying stuff together still sucks
        var mdir = Project.AssetLocator.GetProjectFilePath($@"{Locator.ActiveProject.AssetLocator.GetParamdexDir()}\Meta");
        foreach ((var f, PARAMDEF pdef) in defPairs)
        {
            var fName = f.Substring(f.LastIndexOf('\\') + 1);
            var md = ParamMetaData.XmlDeserialize($@"{mdir}\{fName}", pdef);
            ParamMetas.Add(pdef, md);
        }
    }

    public CompoundAction LoadParamDefaultNames(string param = null, bool onlyAffectEmptyNames = false, bool onlyAffectVanillaNames = false)
    {
        var files = param == null
            ? Project.AssetLocator.GetAllProjectFiles($@"{Project.AssetLocator.GetParamdexDir()}\Names", ["*.txt"], true)
            : new[] { Project.AssetLocator.GetProjectFilePath($@"{Project.AssetLocator.GetParamdexDir()}\Names\{param}.txt") };
        List<EditorAction> actions = new();
        foreach (var f in files)
        {
            var fName = Path.GetFileNameWithoutExtension(f);
            if (!_params.ContainsKey(fName))
            {
                continue;
            }

            var names = File.ReadAllText(f);
            (var result, CompoundAction action) =
                ParamIO.ApplySingleCSV(this, names, fName, "Name", ' ', true, onlyAffectEmptyNames, onlyAffectVanillaNames);
            if (action == null)
            {
                TaskLogs.AddLog($"Could not apply name files for {fName}",
                    LogLevel.Warning);
            }
            else
            {
                actions.Add(action);
            }
        }

        return new CompoundAction(actions);
    }

    public ActionManager TrimNewlineChrsFromNames()
    {
        (MassEditResult r, ActionManager child) =
            MassParamEditRegex.PerformMassEdit(this, "param .*: id .*: name: replace \r:0", null);
        return child;
    }

    private void LoadParamFromBinder(IBinder parambnd, ref Dictionary<string, Param> paramBank, out ulong version,
        bool checkVersion = false)
    {
        var success = ulong.TryParse(parambnd.Version, out version);
        if (checkVersion && !success)
        {
            throw new Exception(@"Failed to get regulation version. Params might be corrupt.");
        }

        // Load every param in the regulation
        foreach (BinderFile f in parambnd.Files)
        {
            var paramName = Path.GetFileNameWithoutExtension(f.Name);

            if (!f.Name.ToUpper().EndsWith(".PARAM"))
            {
                continue;
            }

            if (paramBank.ContainsKey(paramName))
            {
                continue;
            }

            Param p;

            if (Project.Type == GameType.ArmoredCoreVI)
            {
                _usedTentativeParamTypes = new Dictionary<string, string>();
                p = Param.ReadIgnoreCompression(f.Bytes);
                if (!string.IsNullOrEmpty(p.ParamType))
                {
                    if (!_paramdefs.ContainsKey(p.ParamType))
                    {
                        if (_tentativeParamType.TryGetValue(paramName, out var newParamType))
                        {
                            _usedTentativeParamTypes.Add(paramName, p.ParamType);
                            p.ParamType = newParamType;
                            TaskLogs.AddLog(
                                $"Couldn't find ParamDef for {paramName}, but tentative ParamType \"{newParamType}\" exists.",
                                LogLevel.Debug);
                        }
                        else
                        {
                            TaskLogs.AddLog(
                                $"Couldn't find ParamDef for param {paramName} and no tentative ParamType exists.",
                                LogLevel.Error, TaskLogs.LogPriority.High);
                            continue;
                        }
                    }
                }
                else
                {
                    if (_tentativeParamType.TryGetValue(paramName, out var newParamType))
                    {
                        _usedTentativeParamTypes.Add(paramName, p.ParamType);
                        p.ParamType = newParamType;
                        TaskLogs.AddLog(
                            $"Couldn't read ParamType for {paramName}, but tentative ParamType \"{newParamType}\" exists.",
                            LogLevel.Debug);
                    }
                    else
                    {
                        TaskLogs.AddLog(
                            $"Couldn't read ParamType for {paramName} and no tentative ParamType exists.",
                            LogLevel.Error, TaskLogs.LogPriority.High);
                        continue;
                    }
                }
            }
            else
            {
                p = Param.ReadIgnoreCompression(f.Bytes);
                if (!_paramdefs.ContainsKey(p.ParamType ?? ""))
                {
                    TaskLogs.AddLog(
                        $"Couldn't find ParamDef for param {paramName} with ParamType \"{p.ParamType}\".",
                        LogLevel.Warning);
                    continue;
                }
            }

            // Try to fixup Elden Ring ChrModelParam for ER 1.06 because many have been saving botched params and
            // it's an easy fixup
            if (Project.Type == GameType.EldenRing &&
                p.ParamType == "CHR_MODEL_PARAM_ST" &&
                version == 10601000)
            {
                p.FixupERChrModelParam();
            }

            if (p.ParamType == null)
            {
                throw new Exception("Param type is unexpectedly null");
            }

            PARAMDEF def = _paramdefs[p.ParamType];
            try
            {
                p.ApplyParamdef(def, version);
                paramBank.Add(paramName, p);
            }
            catch (Exception e)
            {
                var name = f.Name.Split("\\").Last();
                var message = $"Could not apply ParamDef for {name}";

                if (Project.Type == GameType.DarkSoulsRemastered &&
                    name is "m99_ToneMapBank.param" or "m99_ToneCorrectBank.param"
                        or "default_ToneCorrectBank.param")
                {
                    // Known cases that don't affect standard modmaking
                    TaskLogs.AddLog(message,
                        LogLevel.Warning, TaskLogs.LogPriority.Low);
                }
                else
                {
                    TaskLogs.AddLog(message,
                        LogLevel.Warning, TaskLogs.LogPriority.Normal, e);
                }
            }
        }
    }
    private void LoadParamsDES()
    {
        var param = Project.AssetLocator.GetAssetPathFromOptions([@$"\param\gameparam\gameparamna.parambnd.dcx", @$"\param\gameparam\gameparamna.parambnd", @$"\param\gameparam\gameparam.parambnd.dcx", @$"\param\gameparam\gameparam.parambnd"]).Item2;
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsDESFromFile(param);

        var drawparams = Project.AssetLocator.GetAllAssets($@"\param\drawparam", ["*.parambnd.dcx", "*.parambnd"]);
        foreach (string drawparam in drawparams)
        {
            LoadParamsDESFromFile(drawparam);
        }
    }

    private void LoadParamsDESFromFile(string path)
    {
        using BND3 bnd = BND3.Read(path);
        LoadParamFromBinder(bnd, ref _params, out _paramVersion);
    }

    private void LoadParamsDS1()
    {
        var param = Project.AssetLocator.GetAssetPath($@"param\GameParam\GameParam.parambnd");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsDS1FromFile(param);

        var drawparams = Project.AssetLocator.GetAllAssets($@"param\DrawParam", ["*.parambnd"]);
        foreach (string drawparam in drawparams)
        {
            LoadParamsDS1FromFile(drawparam);
        }
    }

    private void LoadParamsDS1FromFile(string path)
    {
        using BND3 bnd = BND3.Read(path);
        LoadParamFromBinder(bnd, ref _params, out _paramVersion);
    }

    private void LoadParamsDS1R()
    {
        var param = Project.AssetLocator.GetAssetPath($@"param\GameParam\GameParam.parambnd.dcx");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsDS1RFromFile(param);

        var drawparams = Project.AssetLocator.GetAllAssets($@"param\DrawParam", ["*.parambnd.dcx"]);
        foreach (string drawparam in drawparams)
        {
            LoadParamsDS1RFromFile(drawparam);
        }
    }

    private void LoadParamsDS1RFromFile(string path)
    {
        using BND3 bnd = BND3.Read(path);
        LoadParamFromBinder(bnd, ref _params, out _paramVersion);
    }

    private void LoadParamsBBSekiro()
    {
        var param = Project.AssetLocator.GetAssetPath($@"param\gameparam\gameparam.parambnd.dcx");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsBBSekiroFromFile(param);
    }

    private void LoadParamsBBSekiroFromFile(string path)
    {
        using BND4 bnd = BND4.Read(path);
        LoadParamFromBinder(bnd, ref _params, out _paramVersion);
    }

    private static List<string> GetLooseParamsInDir(string dir)
    {
        List<string> looseParams = new();
        if (Directory.Exists($@"{dir}\Param"))
        {
            looseParams.AddRange(Directory.GetFileSystemEntries($@"{dir}\Param", @"*.param"));
        }

        return looseParams;
    }

    private void LoadParamsDS2(bool loose)
    {
        var param = Project.AssetLocator.GetAssetPath($@"enc_regulation.bnd.dcx");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        var looseParams = Project.AssetLocator.GetAllAssets($@"Param", [$@"*.param"]);
        LoadParamsDS2FromFile(looseParams, param, loose);
        LoadExternalRowNames();
    }

    private void LoadParamsDS2FromFile(IEnumerable<string> looseParams, string path, bool loose)
    {
        BND4 paramBnd;
        if (!BND4.Is(path))
        {
            paramBnd = SFUtil.DecryptDS2Regulation(path);
        }
        // No need to decrypt
        else
        {
            paramBnd = BND4.Read(path);
        }

        LoadParamFromBinder(paramBnd, ref _params, out _paramVersion);

        foreach (var p in looseParams)
        {
            var name = Path.GetFileNameWithoutExtension(p);
            Param lp = Param.Read(p);
            var fname = lp.ParamType;

            try
            {
                if (loose)
                {
                    // Loose params: override params already loaded via regulation
                    PARAMDEF def = _paramdefs[lp.ParamType];
                    lp.ApplyParamdef(def);
                    _params[name] = lp;
                }
                else
                {
                    // Non-loose params: do not override params already loaded via regulation
                    if (!_params.ContainsKey(name))
                    {
                        PARAMDEF def = _paramdefs[lp.ParamType];
                        lp.ApplyParamdef(def);
                        _params.Add(name, lp);
                    }
                }
            }
            catch (Exception e)
            {
                var message = $"Could not apply ParamDef for {fname}";
                if (Project.Type == GameType.DarkSoulsIISOTFS &&
                    fname is "GENERATOR_DBG_LOCATION_PARAM")
                {
                    // Known cases that don't affect standard modmaking
                    TaskLogs.AddLog(message,
                        LogLevel.Warning, TaskLogs.LogPriority.Low);
                }
                else
                {
                    TaskLogs.AddLog(message,
                        LogLevel.Warning, TaskLogs.LogPriority.Normal, e);
                }
            }
        }

        paramBnd.Dispose();
    }

    private void LoadParamsDS3(bool loose)
    {
        string param;
        bool looseFile;
        if (loose)
        {
            var p = Project.AssetLocator.GetAssetPathFromOptions([$@"param\gameparam\gameparam_dlc2.parambnd.dcx", $@"Data0.bdt"]);
            looseFile = p.Item1 == 0;
            param = p.Item2;
        }
        else
        {
            var p = Project.AssetLocator.GetAssetPathFromOptions([$@"Data0.bdt", $@"param\gameparam\gameparam_dlc2.parambnd.dcx"]);
            looseFile = p.Item1 == 1;
            param = p.Item2;
        }
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsDS3FromFile(param, looseFile);
    }

    private void LoadParamsDS3FromFile(string path, bool isLoose)
    {
        using BND4 lparamBnd = isLoose ? BND4.Read(path) : SFUtil.DecryptDS3Regulation(path);
        LoadParamFromBinder(lparamBnd, ref _params, out _paramVersion);
    }

    private void LoadParamsER()
    {
        var param = Project.AssetLocator.GetAssetPath($@"regulation.bin");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsERFromFile(param);

        string sysParam = Project.AssetLocator.GetAssetPath(@"param\systemparam\systemparam.parambnd.dcx");
        if (File.Exists(sysParam))
        {
            LoadParamsERFromFile(sysParam, false);
        }
        else
        {
            TaskLogs.AddLog("Systemparam could not be found. These require an unpacked game to modify.", LogLevel.Information, TaskLogs.LogPriority.Normal);
        }

        var eventParam = Project.AssetLocator.GetAssetPath(@"param\eventparam\eventparam.parambnd.dcx");
        if (File.Exists(eventParam))
        {
            LoadParamsERFromFile(eventParam, false);
        }
        else
        {
            TaskLogs.AddLog("Eventparam could not be found. These are not shipped with the game and must be created manually.", LogLevel.Information, TaskLogs.LogPriority.Normal);
        }
    }

    private void LoadParamsERFromFile(string path, bool encrypted = true)
    {
        if (encrypted)
        {
            using BND4 bnd = SFUtil.DecryptERRegulation(path);
            LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
        }
        else
        {
            using BND4 bnd = BND4.Read(path);
            LoadParamFromBinder(bnd, ref _params, out _, false);
        }
    }

    private void LoadParamsAC6()
    {
        var param = Project.AssetLocator.GetAssetPath($@"regulation.bin");
        if (param == null)
        {
            throw CreateParamMissingException(Project.Type);
        }
        LoadParamsAC6FromFile(param, true);

        string sysParam = Project.AssetLocator.GetAssetPath(@"param\systemparam\systemparam.parambnd.dcx");
        if (sysParam != null)
        {
            LoadParamsAC6FromFile(sysParam, false);
        }
        else
        {
            TaskLogs.AddLog("Systemparam could not be found. These require an unpacked game to modify.", LogLevel.Information, TaskLogs.LogPriority.Normal);
        }

        string graphicsConfigParam = Project.AssetLocator.GetAssetPath(@"param\graphicsconfig\graphicsconfig.parambnd.dcx");
        if (graphicsConfigParam != null)
        {
            LoadParamsAC6FromFile(graphicsConfigParam, false);
        }
        else
        {
            TaskLogs.AddLog("Graphicsconfig could not be found. These require an unpacked game to modify.", LogLevel.Information, TaskLogs.LogPriority.Normal);
        }

        string eventParam = Project.AssetLocator.GetAssetPath(@"param\eventparam\eventparam.parambnd.dcx");
        if (eventParam != null)
        {
            LoadParamsAC6FromFile(eventParam, false);
        }
        else
        {
            TaskLogs.AddLog("Eventparam could not be found. These require an unpacked game to modify.", LogLevel.Information, TaskLogs.LogPriority.Normal);
        }
    }

    private void LoadParamsAC6FromFile(string path, bool encrypted = true)
    {
        if (encrypted)
        {
            using BND4 bnd = SFUtil.DecryptAC6Regulation(path);
            LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
        }
        else
        {
            using BND4 bnd = BND4.Read(path);
            LoadParamFromBinder(bnd, ref _params, out _, false);
        }
    }

    private void LoadParams()
    {

        IsDefsLoaded = false;
        IsLoadingParams = true;

        _params = new Dictionary<string, Param>();

        if (Project.Type != GameType.Undefined)
        {
            List<(string, PARAMDEF)> defPairs = LoadParamdefs();
            IsDefsLoaded = true;
            TaskManager.Run(new TaskManager.LiveTask("Param - Load Meta",
                TaskManager.RequeueType.WaitThenRequeue, false, () =>
                {
                    LoadParamMeta(defPairs);
                    IsMetaLoaded = true;
                }));
        }

        if (Project.Type == GameType.DemonsSouls)
        {
            LoadParamsDES();
        }

        if (Project.Type == GameType.DarkSoulsPTDE)
        {
            LoadParamsDS1();
        }

        if (Project.Type == GameType.DarkSoulsRemastered)
        {
            LoadParamsDS1R();
        }

        if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            LoadParamsDS2(Project.Settings.UseLooseParams);
        }

        if (Project.Type == GameType.DarkSoulsIII)
        {
            LoadParamsDS3(Project.Settings.UseLooseParams);
        }

        if (Project.Type == GameType.Bloodborne || Project.Type == GameType.Sekiro)
        {
            LoadParamsBBSekiro();
        }

        if (Project.Type == GameType.EldenRing)
        {
            LoadParamsER();
        }

        if (Project.Type == GameType.ArmoredCoreVI)
        {
            LoadParamsAC6();
        }

        ClearParamDiffCaches();

        IsLoadingParams = false;
    }

    //Some returns and repetition, but it keeps all threading and loading-flags visible inside this method
    public static void ReloadParams(ProjectSettings settings, NewProjectOptions options)
    {
        IsMetaLoaded = false;

        AuxBanks = new Dictionary<string, ParamBank>();

        UICache.ClearCaches();

        TaskManager.Run(new TaskManager.LiveTask("Param - Load Params", TaskManager.RequeueType.WaitThenRequeue,
            false, () =>
            {
                PrimaryBank.LoadParams();

                TaskManager.Run(new TaskManager.LiveTask("Param - Load Vanilla Params",
                    TaskManager.RequeueType.WaitThenRequeue, false, () =>
                    {
                        VanillaBank.LoadParams();

                        TaskManager.Run(new TaskManager.LiveTask("Param - Check Differences",
                            TaskManager.RequeueType.WaitThenRequeue, false,
                            () => RefreshAllParamDiffCaches(true)));
                        UICache.ClearCaches();
                    }));

                if (options != null)
                {
                    if (options.loadDefaultNames)
                    {
                        try
                        {
                            new ActionManager().ExecuteAction(PrimaryBank.LoadParamDefaultNames());
                            PrimaryBank.SaveParams(settings.UseLooseParams);
                        }
                        catch
                        {
                            TaskLogs.AddLog("Could not locate or apply name files",
                                LogLevel.Warning);
                        }
                    }
                }
                UICache.ClearCaches();
            }));
    }

    public static void LoadAuxBank(string dir, ProjectSettings settings = null)
    {
        // skip the meme and just treat as project
        Project siblingVirtualProject = new Project(dir, Locator.ActiveProject.ParentProject, settings);
        ParamBank newBank = siblingVirtualProject.ParamBank;

        newBank.LoadParams();

        newBank.RefreshParamDiffCaches(true);
        AuxBanks[Path.GetFileName(dir).Replace(' ', '_')] = newBank;
    }


    public void ClearParamDiffCaches()
    {
        _vanillaDiffCache = new Dictionary<string, HashSet<int>>();
        _primaryDiffCache = new Dictionary<string, HashSet<int>>();
        foreach (var param in _params.Keys)
        {
            _vanillaDiffCache.Add(param, new HashSet<int>());
            _primaryDiffCache.Add(param, new HashSet<int>());
        }
    }

    public static void RefreshAllParamDiffCaches(bool checkAuxVanillaDiff)
    {
        PrimaryBank.RefreshParamDiffCaches(true);
        foreach (KeyValuePair<string, ParamBank> bank in AuxBanks)
        {
            bank.Value.RefreshParamDiffCaches(checkAuxVanillaDiff);
        }

        UICache.ClearCaches();
    }

    public void RefreshParamDiffCaches(bool checkVanillaDiff)
    {
        if (this != VanillaBank && checkVanillaDiff)
        {
            _vanillaDiffCache = GetParamDiff(VanillaBank);
        }

        if (this == VanillaBank && PrimaryBank._vanillaDiffCache != null)
        {
            _primaryDiffCache = PrimaryBank._vanillaDiffCache;
        }
        else if (this != PrimaryBank)
        {
            _primaryDiffCache = GetParamDiff(PrimaryBank);
        }

        UICache.ClearCaches();
    }

    private Dictionary<string, HashSet<int>> GetParamDiff(ParamBank otherBank)
    {
        if (IsLoadingParams || otherBank == null || otherBank.IsLoadingParams)
        {
            return null;
        }

        Dictionary<string, HashSet<int>> newCache = new();
        foreach (var param in _params.Keys)
        {
            HashSet<int> cache = new();
            newCache.Add(param, cache);
            Param p = _params[param];
            if (!otherBank._params.ContainsKey(param))
            {
                Console.WriteLine("Missing vanilla param " + param);
                continue;
            }

            Param.Row[] rows = _params[param].Rows.OrderBy(r => r.ID).ToArray();
            Param.Row[] vrows = otherBank._params[param].Rows.OrderBy(r => r.ID).ToArray();

            var vanillaIndex = 0;
            var lastID = -1;
            ReadOnlySpan<Param.Row> lastVanillaRows = default;
            for (var i = 0; i < rows.Length; i++)
            {
                var ID = rows[i].ID;
                if (ID == lastID)
                {
                    RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                }
                else
                {
                    lastID = ID;
                    while (vanillaIndex < vrows.Length && vrows[vanillaIndex].ID < ID)
                    {
                        vanillaIndex++;
                    }

                    if (vanillaIndex >= vrows.Length)
                    {
                        RefreshParamRowDiffCache(rows[i], Span<Param.Row>.Empty, cache);
                    }
                    else
                    {
                        var count = 0;
                        while (vanillaIndex + count < vrows.Length && vrows[vanillaIndex + count].ID == ID)
                        {
                            count++;
                        }

                        lastVanillaRows = new ReadOnlySpan<Param.Row>(vrows, vanillaIndex, count);
                        RefreshParamRowDiffCache(rows[i], lastVanillaRows, cache);
                        vanillaIndex += count;
                    }
                }
            }
        }

        return newCache;
    }

    private static void RefreshParamRowDiffCache(Param.Row row, ReadOnlySpan<Param.Row> otherBankRows,
        HashSet<int> cache)
    {
        if (IsChanged(row, otherBankRows))
        {
            cache.Add(row.ID);
        }
        else
        {
            cache.Remove(row.ID);
        }
    }

    public void RefreshParamRowDiffs(Param.Row row, string param)
    {
        if (param == null)
        {
            return;
        }

        if (VanillaBank.Params.ContainsKey(param) && VanillaDiffCache != null &&
            VanillaDiffCache.ContainsKey(param))
        {
            Param.Row[] otherBankRows = VanillaBank.Params[param].Rows.Where(cell => cell.ID == row.ID).ToArray();
            RefreshParamRowDiffCache(row, otherBankRows, VanillaDiffCache[param]);
        }

        if (this != PrimaryBank)
        {
            return;
        }

        foreach (ParamBank aux in AuxBanks.Values)
        {
            if (!aux.Params.ContainsKey(param) || aux.PrimaryDiffCache == null ||
                !aux.PrimaryDiffCache.ContainsKey(param))
            {
                continue; // Don't try for now
            }

            Param.Row[] otherBankRows = aux.Params[param].Rows.Where(cell => cell.ID == row.ID).ToArray();
            RefreshParamRowDiffCache(row, otherBankRows, aux.PrimaryDiffCache[param]);
        }
    }

    private static bool IsChanged(Param.Row row, ReadOnlySpan<Param.Row> vanillaRows)
    {
        //List<Param.Row> vanils = vanilla.Rows.Where(cell => cell.ID == row.ID).ToList();
        if (vanillaRows.Length == 0)
        {
            return true;
        }

        foreach (Param.Row vrow in vanillaRows)
        {
            if (row.RowMatches(vrow))
            {
                return false; //if we find a matching vanilla row
            }
        }

        return true;
    }
    private void SaveParamsDS1()
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.AssetLocator.GetAssetPath($@"param\GameParam\GameParam.parambnd");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        using BND3 paramBnd = BND3.Read(param);

        // Replace params with edited ones
        foreach (BinderFile p in paramBnd.Files)
        {
            if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
            {
                p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
            }
        }

        Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd", paramBnd);

        // Drawparam
        foreach (var bnd in Project.AssetLocator.GetAllAssets($@"param\DrawParam", [$@"*.parambnd"]))
        {
            using BND3 drawParamBnd = BND3.Read(bnd);
            foreach (BinderFile p in drawParamBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }

            Utils.WriteWithBackup(dir, mod, @$"param\DrawParam\{Path.GetFileName(bnd)}", drawParamBnd);
        }
    }

    private void SaveParamsDS1R()
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.AssetLocator.GetAssetPath($@"param\GameParam\GameParam.parambnd.dcx");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        using BND3 paramBnd = BND3.Read(param);

        // Replace params with edited ones
        foreach (BinderFile p in paramBnd.Files)
        {
            if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
            {
                p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
            }
        }

        Utils.WriteWithBackup(dir, mod, @"param\GameParam\GameParam.parambnd.dcx", paramBnd);

        // Drawparam
        foreach (var bnd in Project.AssetLocator.GetAllAssets($@"param\DrawParam", ["*.parambnd.dcx"]))
        {
            using BND3 drawParamBnd = BND3.Read(bnd);
            foreach (BinderFile p in drawParamBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }

            Utils.WriteWithBackup(dir, mod, @$"param\DrawParam\{Path.GetFileName(bnd)}", drawParamBnd);
        }
    }

    private void SaveParamsDS2(bool loose)
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.AssetLocator.GetAssetPath($@"enc_regulation.bnd.dcx");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }

        // Load params
        BND4 paramBnd;
        if (!BND4.Is(param))
        {
            // Decrypt the file
            paramBnd = SFUtil.DecryptDS2Regulation(param);

            // Since the file is encrypted, check for a backup. If it has none, then make one and write a decrypted one.
            if (!File.Exists($@"{param}.bak"))
            {
                File.Copy(param, $@"{param}.bak", true);
                paramBnd.Write(param);
            }
        }
        // No need to decrypt
        else
        {
            paramBnd = BND4.Read(param);
        }

        if (!loose)
        {
            // Save params non-loosely: Replace params regulation and write remaining params loosely.

            if (paramBnd.Files.Find(e => e.Name.EndsWith(".param")) == null)
            {
                if (PlatformUtils.Instance.MessageBox(
                        "It appears that you are trying to save params non-loosely with an \"enc_regulation.bnd\" that has previously been saved loosely." +
                        "\n\nWould you like to reinsert params into the bnd that were previously stripped out?",
                        "DS2 de-loose param",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    paramBnd.Dispose();
                    param = $@"{dir}\enc_regulation.bnd.dcx";
                    if (!BND4.Is($@"{dir}\enc_regulation.bnd.dcx"))
                    {
                        // Decrypt the file.
                        paramBnd = SFUtil.DecryptDS2Regulation(param);

                        // Since the file is encrypted, check for a backup. If it has none, then make one and write a decrypted one.
                        if (!File.Exists($@"{param}.bak"))
                        {
                            File.Copy(param, $@"{param}.bak", true);
                            paramBnd.Write(param);
                        }
                    }
                    else
                    {
                        paramBnd = BND4.Read(param);
                    }
                }
            }

            try
            {
                // Strip and store row names before saving, as too many row names can cause DS2 to crash.
                StripRowNames();

                foreach (KeyValuePair<string, Param> p in _params)
                {
                    BinderFile bnd = paramBnd.Files.Find(e => Path.GetFileNameWithoutExtension(e.Name) == p.Key);
                    if (bnd != null)
                    {
                        // Regulation contains this param, overwrite it.
                        bnd.Bytes = p.Value.Write();
                    }
                    else
                    {
                        // Regulation does not contain this param, write param loosely.
                        Utils.WriteWithBackup(dir, mod, $@"Param\{p.Key}.param", p.Value);
                    }
                }
            }
            catch
            {
                RestoreStrippedRowNames();
                throw;
            }

            RestoreStrippedRowNames();
        }
        else
        {
            // Save params loosely: Strip params from regulation and write all params loosely.

            List<BinderFile> newFiles = new();
            foreach (BinderFile p in paramBnd.Files)
            {
                // Strip params from regulation bnd
                if (!p.Name.ToUpper().Contains(".PARAM"))
                {
                    newFiles.Add(p);
                }
            }

            paramBnd.Files = newFiles;

            try
            {
                // Strip and store row names before saving, as too many row names can cause DS2 to crash.
                StripRowNames();

                // Write params to loose files.
                foreach (KeyValuePair<string, Param> p in _params)
                {
                    Utils.WriteWithBackup(dir, mod, $@"Param\{p.Key}.param", p.Value);
                }
            }
            catch
            {
                RestoreStrippedRowNames();
                throw;
            }

            RestoreStrippedRowNames();
        }

        Utils.WriteWithBackup(dir, mod, @"enc_regulation.bnd.dcx", paramBnd);
        paramBnd.Dispose();
    }

    private void SaveParamsDS3(bool loose)
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        string param;
        bool looseFile;
        if (loose)
        {
            var p = Project.AssetLocator.GetAssetPathFromOptions([$@"param\gameparam\gameparam_dlc2.parambnd.dcx", $@"Data0.bdt"]);
            looseFile = p.Item1 == 0;
            param = p.Item2;
        }
        else
        {
            var p = Project.AssetLocator.GetAssetPathFromOptions([$@"Data0.bdt", $@"param\gameparam\gameparam_dlc2.parambnd.dcx"]);
            looseFile = p.Item1 != 1;
            param = p.Item2;
        }
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        BND4 paramBnd = SFUtil.DecryptDS3Regulation(param);

        // Replace params with edited ones
        foreach (BinderFile p in paramBnd.Files)
        {
            if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
            {
                p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
            }
        }

        // If not loose write out the new regulation
        if (!loose)
        {
            Utils.WriteWithBackup(dir, mod, @"Data0.bdt", paramBnd, GameType.DarkSoulsIII);
        }
        else
        {
            // Otherwise write them out as parambnds
            BND4 paramBND = new()
            {
                BigEndian = false,
                Compression = DCX.Type.DCX_DFLT_10000_44_9,
                Extended = 0x04,
                Unk04 = false,
                Unk05 = false,
                Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets |
                         Binder.Format.Names1,
                Unicode = true,
                Files = paramBnd.Files.Where(f => f.Name.EndsWith(".param")).ToList()
            };
            Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam_dlc2.parambnd.dcx", paramBND);
        }
    }

    private void SaveParamsBBSekiro()
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.AssetLocator.GetAssetPath($@"param\gameparam\gameparam.parambnd.dcx");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        BND4 paramBnd = BND4.Read(param);

        // Replace params with edited ones
        foreach (BinderFile p in paramBnd.Files)
        {
            if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
            {
                p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
            }
        }

        Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam.parambnd.dcx", paramBnd);
    }

    private void SaveParamsDES()
    {
        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.ParentProject.AssetLocator.GetAssetPathFromOptions([@$"\param\gameparam\gameparamna.parambnd.dcx", @$"\param\gameparam\gameparamna.parambnd", @$"\param\gameparam\gameparam.parambnd.dcx", @$"\param\gameparam\gameparam.parambnd"]).Item2;

        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }

        using BND3 paramBnd = BND3.Read(param);

        // Replace params with edited ones
        foreach (BinderFile p in paramBnd.Files)
        {
            if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
            {
                p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
            }
        }

        // Write all gameparam variations since we don't know which one the the game will use.
        // Compressed
        paramBnd.Compression = DCX.Type.DCX_EDGE;
        var naParamPath = @"param\gameparam\gameparamna.parambnd.dcx";
        if (File.Exists($@"{dir}\{naParamPath}"))
        {
            Utils.WriteWithBackup(dir, mod, naParamPath, paramBnd);
        }

        Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam.parambnd.dcx", paramBnd);

        // Decompressed
        paramBnd.Compression = DCX.Type.None;
        naParamPath = @"param\gameparam\gameparamna.parambnd";
        if (File.Exists($@"{dir}\{naParamPath}"))
        {
            Utils.WriteWithBackup(dir, mod, naParamPath, paramBnd);
        }

        Utils.WriteWithBackup(dir, mod, @"param\gameparam\gameparam.parambnd", paramBnd);

        // Drawparam
        var drawparambnds = Project.AssetLocator.GetAllAssets($@"param\drawparam", ["*.parambnd.dcx", "*.parambnd"]);
        foreach (var bnd in drawparambnds)
        {
            using BND3 drawParamBnd = BND3.Read(bnd);
            foreach (BinderFile p in drawParamBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    p.Bytes = _params[Path.GetFileNameWithoutExtension(p.Name)].Write();
                }
            }

            Utils.WriteWithBackup(dir, mod, @$"param\drawparam\{Path.GetFileName(bnd)}", drawParamBnd);
        }
    }

    private void SaveParamsER()
    {
        void OverwriteParamsER(BND4 paramBnd)
        {
            // Replace params with edited ones
            foreach (BinderFile p in paramBnd.Files)
            {
                if (_params.ContainsKey(Path.GetFileNameWithoutExtension(p.Name)))
                {
                    Param paramFile = _params[Path.GetFileNameWithoutExtension(p.Name)];
                    p.Bytes = paramFile.Write();
                }
            }
        }

        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;

        var param = Project.AssetLocator.GetAssetPath($@"regulation.bin");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        BND4 regParams = SFUtil.DecryptERRegulation(param);
        OverwriteParamsER(regParams);
        Utils.WriteWithBackup(dir, mod, @"regulation.bin", regParams, GameType.EldenRing);

        string sysParam = Project.AssetLocator.GetAssetPath(@"param\systemparam\systemparam.parambnd.dcx");
        if (sysParam != null)
        {
            using BND4 sysParams = BND4.Read(sysParam);
            OverwriteParamsER(sysParams);
            Utils.WriteWithBackup(dir, mod, @"param\systemparam\systemparam.parambnd.dcx", sysParams);
        }

        var eventParam = Project.AssetLocator.GetAssetPath(@"param\eventparam\eventparam.parambnd.dcx");
        if (eventParam != null)
        {
            using var eventParams = BND4.Read(eventParam);
            OverwriteParamsER(eventParams);
            Utils.WriteWithBackup(dir, mod, @"param\eventparam\eventparam.parambnd.dcx", eventParams);
        }

        _pendingUpgrade = false;
    }

    private void SaveParamsAC6()
    {
        void OverwriteParamsAC6(BND4 paramBnd)
        {
            // Replace params with edited ones
            foreach (BinderFile p in paramBnd.Files)
            {
                var paramName = Path.GetFileNameWithoutExtension(p.Name);
                if (_params.TryGetValue(paramName, out Param paramFile))
                {
                    IReadOnlyList<Param.Row> backup = paramFile.Rows;
                    if (Project.Type is GameType.ArmoredCoreVI)
                    {
                        if (_usedTentativeParamTypes.TryGetValue(paramName, out var oldParamType))
                        {
                            // This param was given a tentative ParamType, return original ParamType if possible.
                            oldParamType ??= "";
                            var prevParamType = paramFile.ParamType;
                            paramFile.ParamType = oldParamType;

                            p.Bytes = paramFile.Write();
                            paramFile.ParamType = prevParamType;
                            paramFile.Rows = backup;
                            continue;
                        }
                    }

                    p.Bytes = paramFile.Write();
                    paramFile.Rows = backup;
                }
            }
        }

        var dir = Project.ParentProject.AssetLocator.RootDirectory;
        var mod = Project.AssetLocator.RootDirectory;
        var param = Project.AssetLocator.GetAssetPath($@"regulation.bin");
        if (param == null)
        {
            TaskLogs.AddLog("Cannot locate param files. Save failed.",
                LogLevel.Error, TaskLogs.LogPriority.High);
            return;
        }
        BND4 regParams = SFUtil.DecryptAC6Regulation(param);
        OverwriteParamsAC6(regParams);
        Utils.WriteWithBackup(dir, mod, @"regulation.bin", regParams, GameType.ArmoredCoreVI);

        string sysParam = Project.AssetLocator.GetAssetPath(@"param\systemparam\systemparam.parambnd.dcx");
        if (sysParam != null)
        {
            using BND4 sysParams = BND4.Read(sysParam);
            OverwriteParamsAC6(sysParams);
            Utils.WriteWithBackup(dir, mod, @"param\systemparam\systemparam.parambnd.dcx", sysParams);
        }

        string graphicsConfigParam = Project.AssetLocator.GetAssetPath(@"param\graphicsconfig\graphicsconfig.parambnd.dcx");
        if (graphicsConfigParam != null)
        {
            using BND4 graphicsConfigParams = BND4.Read(graphicsConfigParam);
            OverwriteParamsAC6(graphicsConfigParams);
            Utils.WriteWithBackup(dir, mod, @"param\graphicsconfig\graphicsconfig.parambnd.dcx", graphicsConfigParams);
        }

        string eventParam = Project.AssetLocator.GetAssetPath(@"param\eventparam\eventparam.parambnd.dcx");
        if (eventParam != null)
        {
            using BND4 eventParams = BND4.Read(eventParam);
            OverwriteParamsAC6(eventParams);
            Utils.WriteWithBackup(dir, mod, @"param\eventparam\eventparam.parambnd.dcx", eventParams);
        }

        _pendingUpgrade = false;
    }

    public void SaveParams(bool loose = false)
    {
        if (_params == null)
        {
            return;
        }

        if (Project.Type == GameType.DarkSoulsPTDE)
        {
            SaveParamsDS1();
        }

        if (Project.Type == GameType.DarkSoulsRemastered)
        {
            SaveParamsDS1R();
        }

        if (Project.Type == GameType.DemonsSouls)
        {
            SaveParamsDES();
        }

        if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            SaveParamsDS2(loose);
        }

        if (Project.Type == GameType.DarkSoulsIII)
        {
            SaveParamsDS3(loose);
        }

        if (Project.Type == GameType.Bloodborne || Project.Type == GameType.Sekiro)
        {
            SaveParamsBBSekiro();
        }

        if (Project.Type == GameType.EldenRing)
        {
            SaveParamsER();
        }

        if (Project.Type == GameType.ArmoredCoreVI)
        {
            SaveParamsAC6();
        }
    }

    private static Param UpgradeParam(Param source, Param oldVanilla, Param newVanilla, HashSet<int> rowConflicts)
    {
        // Presorting this would make it easier, but we're trying to preserve order as much as possible
        // Unfortunately given that rows aren't guaranteed to be sorted and there can be duplicate IDs,
        // we try to respect the existing order and IDs as much as possible.

        // In order to assemble the final param, the param needs to know where to sort rows from given the
        // following rules:
        // 1. If a row with a given ID is unchanged from source to oldVanilla, we source from newVanilla
        // 2. If a row with a given ID is deleted from source compared to oldVanilla, we don't take any row
        // 3. If a row with a given ID is changed from source compared to oldVanilla, we source from source
        // 4. If a row has duplicate IDs, we treat them as if the rows were deduplicated and process them
        //    in the order they appear.

        // List of rows that are in source but not oldVanilla
        Dictionary<int, List<Param.Row>> addedRows = new(source.Rows.Count);

        // List of rows in oldVanilla that aren't in source
        Dictionary<int, List<Param.Row>> deletedRows = new(source.Rows.Count);

        // List of rows that are in source and oldVanilla, but are modified
        Dictionary<int, List<Param.Row>> modifiedRows = new(source.Rows.Count);

        // List of rows that only had the name changed
        Dictionary<int, List<Param.Row>> renamedRows = new(source.Rows.Count);

        // List of ordered edit operations for each ID
        Dictionary<int, List<EditOperation>> editOperations = new(source.Rows.Count);

        // First off we go through source and everything starts as an added param
        foreach (Param.Row row in source.Rows)
        {
            if (!addedRows.ContainsKey(row.ID))
            {
                addedRows.Add(row.ID, new List<Param.Row>());
            }

            addedRows[row.ID].Add(row);
        }

        // Next we go through oldVanilla to determine if a row is added, deleted, modified, or unmodified
        foreach (Param.Row row in oldVanilla.Rows)
        {
            // First off if the row did not exist in the source, it's deleted
            if (!addedRows.ContainsKey(row.ID))
            {
                if (!deletedRows.ContainsKey(row.ID))
                {
                    deletedRows.Add(row.ID, new List<Param.Row>());
                }

                deletedRows[row.ID].Add(row);
                if (!editOperations.ContainsKey(row.ID))
                {
                    editOperations.Add(row.ID, new List<EditOperation>());
                }

                editOperations[row.ID].Add(EditOperation.Delete);
                continue;
            }

            // Otherwise the row exists in source. Time to classify it.
            List<Param.Row> list = addedRows[row.ID];

            // First we see if we match the first target row. If so we can remove it.
            if (row.DataEquals(list[0]))
            {
                Param.Row modrow = list[0];
                list.RemoveAt(0);
                if (list.Count == 0)
                {
                    addedRows.Remove(row.ID);
                }

                if (!editOperations.ContainsKey(row.ID))
                {
                    editOperations.Add(row.ID, new List<EditOperation>());
                }

                // See if the name was not updated
                if ((modrow.Name == null && row.Name == null) ||
                    (modrow.Name != null && row.Name != null && modrow.Name == row.Name))
                {
                    editOperations[row.ID].Add(EditOperation.Match);
                    continue;
                }

                // Name was updated
                editOperations[row.ID].Add(EditOperation.NameChange);
                if (!renamedRows.ContainsKey(row.ID))
                {
                    renamedRows.Add(row.ID, new List<Param.Row>());
                }

                renamedRows[row.ID].Add(modrow);

                continue;
            }

            // Otherwise it is modified
            if (!modifiedRows.ContainsKey(row.ID))
            {
                modifiedRows.Add(row.ID, new List<Param.Row>());
            }

            modifiedRows[row.ID].Add(list[0]);
            list.RemoveAt(0);
            if (list.Count == 0)
            {
                addedRows.Remove(row.ID);
            }

            if (!editOperations.ContainsKey(row.ID))
            {
                editOperations.Add(row.ID, new List<EditOperation>());
            }

            editOperations[row.ID].Add(EditOperation.Modify);
        }

        // Mark all remaining rows as added
        foreach (KeyValuePair<int, List<Param.Row>> entry in addedRows)
        {
            if (!editOperations.ContainsKey(entry.Key))
            {
                editOperations.Add(entry.Key, new List<EditOperation>());
            }

            foreach (List<EditOperation> k in editOperations.Values)
            {
                editOperations[entry.Key].Add(EditOperation.Add);
            }
        }

        if (editOperations.All(kvp => kvp.Value.All(eo => eo == EditOperation.Match)))
        {
            return oldVanilla;
        }

        Param dest = new(newVanilla);

        // Now try to build the destination from the new regulation with the edit operations in mind
        var pendingAdds = addedRows.Keys.OrderBy(e => e).ToArray();
        var currPendingAdd = 0;
        var lastID = 0;
        foreach (Param.Row row in newVanilla.Rows)
        {
            // See if we have any pending adds we can slot in
            while (currPendingAdd < pendingAdds.Length &&
                   pendingAdds[currPendingAdd] >= lastID &&
                   pendingAdds[currPendingAdd] < row.ID)
            {
                if (!addedRows.ContainsKey(pendingAdds[currPendingAdd]))
                {
                    currPendingAdd++;
                    continue;
                }

                foreach (Param.Row arow in addedRows[pendingAdds[currPendingAdd]])
                {
                    dest.AddRow(new Param.Row(arow, dest));
                }

                addedRows.Remove(pendingAdds[currPendingAdd]);
                editOperations.Remove(pendingAdds[currPendingAdd]);
                currPendingAdd++;
            }

            lastID = row.ID;

            if (!editOperations.ContainsKey(row.ID))
            {
                // No edit operations for this ID, so just add it (likely a new row in the update)
                dest.AddRow(new Param.Row(row, dest));
                continue;
            }

            // Pop the latest operation we need to do
            EditOperation operation = editOperations[row.ID][0];
            editOperations[row.ID].RemoveAt(0);
            if (editOperations[row.ID].Count == 0)
            {
                editOperations.Remove(row.ID);
            }

            if (operation == EditOperation.Add)
            {
                // Getting here means both the mod and the updated regulation added a row. Our current strategy is
                // to overwrite the new vanilla row with the modded one and add to the conflict log to give the user
                rowConflicts.Add(row.ID);
                dest.AddRow(new Param.Row(addedRows[row.ID][0], dest));
                addedRows[row.ID].RemoveAt(0);
                if (addedRows[row.ID].Count == 0)
                {
                    addedRows.Remove(row.ID);
                }
            }
            else if (operation == EditOperation.Match)
            {
                // Match means we inherit updated param
                dest.AddRow(new Param.Row(row, dest));
            }
            else if (operation == EditOperation.Delete)
            {
                // deleted means we don't add anything
                deletedRows[row.ID].RemoveAt(0);
                if (deletedRows[row.ID].Count == 0)
                {
                    deletedRows.Remove(row.ID);
                }
            }
            else if (operation == EditOperation.Modify)
            {
                // Modified means we use the modded regulation's param
                dest.AddRow(new Param.Row(modifiedRows[row.ID][0], dest));
                modifiedRows[row.ID].RemoveAt(0);
                if (modifiedRows[row.ID].Count == 0)
                {
                    modifiedRows.Remove(row.ID);
                }
            }
            else if (operation == EditOperation.NameChange)
            {
                // Inherit name
                Param.Row newRow = new(row, dest);
                newRow.Name = renamedRows[row.ID][0].Name;
                dest.AddRow(newRow);
                renamedRows[row.ID].RemoveAt(0);
                if (renamedRows[row.ID].Count == 0)
                {
                    renamedRows.Remove(row.ID);
                }
            }
        }

        // Take care of any more pending adds
        for (; currPendingAdd < pendingAdds.Length; currPendingAdd++)
        {
            // If the pending add doesn't exist in the added rows list, it was a conflicting row
            if (!addedRows.ContainsKey(pendingAdds[currPendingAdd]))
            {
                continue;
            }

            foreach (Param.Row arow in addedRows[pendingAdds[currPendingAdd]])
            {
                dest.AddRow(new Param.Row(arow, dest));
            }

            addedRows.Remove(pendingAdds[currPendingAdd]);
            editOperations.Remove(pendingAdds[currPendingAdd]);
        }

        return dest;
    }

    // Param upgrade. Currently for Elden Ring only.
    public ParamUpgradeResult UpgradeRegulation(ParamBank vanillaBank, string oldVanillaParamPath,
        Dictionary<string, HashSet<int>> conflictingParams)
    {
        // First we need to load the old regulation
        if (!File.Exists(oldVanillaParamPath))
        {
            return ParamUpgradeResult.OldRegulationNotFound;
        }    
        
        // Backup modded params
        string modRegulationPath = $@"{Project.AssetLocator.RootDirectory}\regulation.bin";
        File.Copy(modRegulationPath, $@"{modRegulationPath}.upgrade.bak", true);

        // Load old vanilla regulation
        BND4 oldVanillaParamBnd;
        if (Project.Type == GameType.EldenRing)
        {
            oldVanillaParamBnd = SFUtil.DecryptERRegulation(oldVanillaParamPath);
        }
        else if (Project.Type == GameType.ArmoredCoreVI)
        {
            oldVanillaParamBnd = SFUtil.DecryptAC6Regulation(oldVanillaParamPath);
        }
        else
        {
            throw new NotImplementedException(
                $"Param upgrading for game type {Project.Type} is not supported.");
        }

        Dictionary<string, Param> oldVanillaParams = new();
        ulong version;
        LoadParamFromBinder(oldVanillaParamBnd, ref oldVanillaParams, out version, true);
        if (version != ParamVersion)
        {
            return ParamUpgradeResult.OldRegulationVersionMismatch;
        }

        Dictionary<string, Param> updatedParams = new();
        // Now we must diff everything to try and find changed/added rows for each param
        var anyUpgrades = false;
        foreach (var k in vanillaBank.Params.Keys)
        {
            // If the param is completely new, just take it
            if (!oldVanillaParams.ContainsKey(k) || !Params.ContainsKey(k))
            {
                updatedParams.Add(k, vanillaBank.Params[k]);
                continue;
            }

            // Otherwise try to upgrade
            HashSet<int> conflicts = new();
            Param res = UpgradeParam(Params[k], oldVanillaParams[k], vanillaBank.Params[k], conflicts);
            if (res != oldVanillaParams[k])
            {
                anyUpgrades = true;
            }

            updatedParams.Add(k, res);

            if (conflicts.Count > 0)
            {
                conflictingParams.Add(k, conflicts);
            }
        }

        if (!anyUpgrades)
        {
            return ParamUpgradeResult.OldRegulationMatchesCurrent;
        }

        var oldVersion = _paramVersion;

        // Set new params
        _params = updatedParams;
        _paramVersion = VanillaBank.ParamVersion;
        _pendingUpgrade = true;

        // Refresh dirty cache
        UICache.ClearCaches();
        RefreshAllParamDiffCaches(false);

        return conflictingParams.Count > 0 ? ParamUpgradeResult.RowConflictsFound : ParamUpgradeResult.Success;
    }

    public string GetChrIDForEnemy(long enemyID)
    {
        Param.Row enemy = EnemyParam?[(int)enemyID];
        return enemy != null ? $@"{enemy.GetCellHandleOrThrow("chr_id").Value:D4}" : null;
    }

    public string GetKeyForParam(Param param)
    {
        if (Params == null)
        {
            return null;
        }

        foreach (KeyValuePair<string, Param> pair in Params)
        {
            if (param == pair.Value)
            {
                return pair.Key;
            }
        }

        return null;
    }

    public Param GetParamFromName(string param)
    {
        if (Params == null)
        {
            return null;
        }

        foreach (KeyValuePair<string, Param> pair in Params)
        {
            if (param == pair.Key)
            {
                return pair.Value;
            }
        }

        return null;
    }

    public HashSet<int> GetVanillaDiffRows(string param)
    {
        IReadOnlyDictionary<string, HashSet<int>> allDiffs = VanillaDiffCache;
        if (allDiffs == null || !allDiffs.ContainsKey(param))
        {
            return EMPTYSET;
        }

        return allDiffs[param];
    }

    public HashSet<int> GetPrimaryDiffRows(string param)
    {
        IReadOnlyDictionary<string, HashSet<int>> allDiffs = PrimaryDiffCache;
        if (allDiffs == null || !allDiffs.ContainsKey(param))
        {
            return EMPTYSET;
        }

        return allDiffs[param];
    }

    /// <summary>
    ///     Loads row names from external files and applies them to params.
    ///     Uses indicies rather than IDs.
    /// </summary>
    private void LoadExternalRowNames()
    {
        var failCount = 0;
        foreach (KeyValuePair<string, Param> p in _params)
        {
            var path = Project.AssetLocator.GetStrippedRowNamesPath(p.Key);
            if (File.Exists(path))
            {
                var names = File.ReadAllLines(path);
                if (names.Length != p.Value.Rows.Count)
                {
                    TaskLogs.AddLog($"External row names could not be applied to {p.Key}, row count does not match",
                        LogLevel.Warning, TaskLogs.LogPriority.Low);
                    failCount++;
                    continue;
                }

                for (var i = 0; i < names.Length; i++)
                {
                    p.Value.Rows[i].Name = names[i];
                }
            }
        }

        if (failCount > 0)
        {
            TaskLogs.AddLog(
                $"External row names could not be applied to {failCount} params due to non-matching row counts.",
                LogLevel.Warning);
        }
    }

    /// <summary>
    ///     Strips row names from params, saves them to files, and stores them to be restored after saving params.
    ///     Should always be used in conjunction with RestoreStrippedRowNames().
    /// </summary>
    private void StripRowNames()
    {
        _storedStrippedRowNames = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, Param> p in _params)
        {
            _storedStrippedRowNames.TryAdd(p.Key, new List<string>());
            List<string> list = _storedStrippedRowNames[p.Key];
            foreach (Param.Row r in p.Value.Rows)
            {
                list.Add(r.Name);
                r.Name = "";
            }

            var path = Project.AssetLocator.GetStrippedRowNamesPath(p.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllLines(path, list);
        }
    }

    /// <summary>
    ///     Restores stripped row names back to all params.
    ///     Should always be used in conjunction with StripRowNames().
    /// </summary>
    private void RestoreStrippedRowNames()
    {
        if (_storedStrippedRowNames == null)
        {
            throw new InvalidOperationException("No stripped row names have been stored.");
        }

        foreach (KeyValuePair<string, Param> p in _params)
        {
            List<string> storedNames = _storedStrippedRowNames[p.Key];
            for (var i = 0; i < p.Value.Rows.Count; i++)
            {
                p.Value.Rows[i].Name = storedNames[i];
            }
        }

        _storedStrippedRowNames = null;
    }

    private enum EditOperation
    {
        Add,
        Delete,
        Modify,
        NameChange,
        Match
    }
}
