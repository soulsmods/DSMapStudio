using Andre.Formats;
using Microsoft.Extensions.Logging;
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
public partial class ParamBank : DataBank
{
    public static ParamBank PrimaryBank => Locator.ActiveProject.ParamBank;
    public static ParamBank VanillaBank => Locator.ActiveProject.ParentProject.ParamBank;

    public static string ClipboardParam = null;
    public static List<Param.Row> ClipboardRows = new();

    private Dictionary<string, Param> _params;

    private ulong _paramVersion;

    /// <summary>
    ///     Dictionary of param file names that were given a tentative ParamType, and the original ParamType it had.
    ///     Used to later restore original ParamType on write (if possible).
    /// </summary>
    private Dictionary<string, string?> _usedTentativeParamTypes;

    private Param EnemyParam => _params["EnemyParam"];

    public IReadOnlyDictionary<string, Param> Params
    {
        get
        {
            if (IsLoading)
            {
                return null;
            }

            return _params;
        }
    }

    public ulong ParamVersion => _paramVersion;
    public ParamBank(Project owner) : base(owner, "Params")
    {
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
                    if (!ResDirectory.CurrentGame.ParamDefBank.GetParamDefs().ContainsKey(p.ParamType))
                    {
                        if (ResDirectory.CurrentGame.ParamDefBank.GetTentativeParamTypes().TryGetValue(paramName, out var newParamType))
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
                    if (ResDirectory.CurrentGame.ParamDefBank.GetTentativeParamTypes().TryGetValue(paramName, out var newParamType))
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
                if (!ResDirectory.CurrentGame.ParamDefBank.GetParamDefs().ContainsKey(p.ParamType ?? ""))
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

            PARAMDEF def = ResDirectory.CurrentGame.ParamDefBank.GetParamDefs()[p.ParamType];
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
                    PARAMDEF def = ResDirectory.CurrentGame.ParamDefBank.GetParamDefs()[lp.ParamType];
                    lp.ApplyParamdef(def);
                    _params[name] = lp;
                }
                else
                {
                    // Non-loose params: do not override params already loaded via regulation
                    if (!_params.ContainsKey(name))
                    {
                        PARAMDEF def = ResDirectory.CurrentGame.ParamDefBank.GetParamDefs()[lp.ParamType];
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
    protected override void Load()
    {
        _params = new Dictionary<string, Param>();

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

        UICache.ClearCaches();
    }

    // TODO: Repair on-load actions
    //Some returns and repetition, but it keeps all threading and loading-flags visible inside this method
    /*public static void ReloadParams(ProjectSettings settings, NewProjectOptions options)
    {
        //TODO: subsume with databank system
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
    }*/

    public static void LoadAuxBank(string dir, ProjectSettings settings = null)
    {
        Project siblingVirtualProject = new Project(dir, Locator.ActiveProject.ParentProject, settings);
        StudioResource.Load(siblingVirtualProject, [siblingVirtualProject.ParamBank, siblingVirtualProject.ParamDiffBank]);
        ResDirectory.CurrentGame.AuxProjects[Path.GetFileName(siblingVirtualProject.AssetLocator.RootDirectory).Replace(' ', '_')] = siblingVirtualProject;
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
    public override void Save()
    {
        bool loose = Project.Settings.UseLooseParams;
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

    protected override IEnumerable<StudioResource> GetDependencies(Project project)
    {
        return [ResDirectory.CurrentGame.ParamDefBank];
    }
}
