using Andre.Formats;
using Microsoft.Extensions.Logging;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using StudioCore.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Utilities;
public static class ParamValidationTool
{
    private static Dictionary<string, PARAMDEF> _paramdefs = new Dictionary<string, PARAMDEF>();
    private static Dictionary<string, Param> _params = new Dictionary<string, Param>();
    private static ulong _paramVersion;

    public static void ValidatePadding()
    {
        foreach (var entry in ParamBank.VanillaBank.Params)
        {
            var selectedParamName = entry.Key;
            ValidatePaddingForParam(selectedParamName);
        }
    }

    public static void ValidatePaddingForParam(string selectedParamName)
    {
        var currentParam = ParamBank.VanillaBank.Params[selectedParamName];
        var currentRow = 0;

        TaskManager.Run(new TaskManager.LiveTask($"Validate {selectedParamName} Padding", TaskManager.RequeueType.None, false,
        () =>
        {
            foreach (var row in currentParam.Rows)
            {
                currentRow = row.ID;

                foreach (var cell in row.Cells)
                {
                    if (cell.Def.InternalType == "dummy8")
                    {
                        //TaskLogs.AddLog(cell.Value.GetType().Name);

                        if (cell.Value.GetType() == typeof(byte[]))
                        {
                            // TaskLogs.AddLog($"{currentParam}: {cell.Def.InternalName}");

                            byte[] bytes = (byte[])cell.Value;
                            foreach (var b in bytes)
                            {
                                if (b != 0)
                                {
                                    TaskLogs.AddLog($"{selectedParamName}: {currentRow}: {cell.Def.InternalName} contains non-zero values");
                                }
                            }
                        }
                        else if (cell.Value.GetType() == typeof(byte))
                        {
                            //TaskLogs.AddLog($"{currentParam}: {cell.Def.InternalName}");

                            byte b = (byte)cell.Value;
                            if (b != 0)
                            {
                                TaskLogs.AddLog($"{selectedParamName}: {currentRow}: {cell.Def.InternalName} contains non-zero values");
                            }
                        }
                    }
                }
            }
        }));
    }

    public static void ValidateParamdef()
    {
        // Read params from regulation.bin via SF PARAM impl
        _paramdefs = ParamBank.PrimaryBank.GetParamDefs();

        var dir = Locator.ActiveProject.ParentProject.AssetLocator.RootDirectory;
        var mod = Locator.ActiveProject.AssetLocator.RootDirectory;

        TaskLogs.AddLog(mod);

        var param = $@"{mod}\regulation.bin";

        // DES, DS1, DS1R
        if (Locator.ActiveProject.Type is GameType.DemonsSouls or GameType.DarkSoulsPTDE or GameType.DarkSoulsRemastered)
        {
            try
            {
                using BND3 bnd = BND3.Read(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // DS2
        if (Locator.ActiveProject.Type is GameType.DarkSoulsIISOTFS)
        {
            try
            {
                using BND4 bnd = SFUtil.DecryptDS2Regulation(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // DS3
        if (Locator.ActiveProject.Type is GameType.DarkSoulsIII)
        {
            param = $@"{mod}\Data0.bdt";

            try
            {
                using BND4 bnd = SFUtil.DecryptDS3Regulation(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // BB, SDT
        if (Locator.ActiveProject.Type is GameType.Sekiro or GameType.Bloodborne)
        {
            try
            {
                using BND4 bnd = BND4.Read(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // ER
        if (Locator.ActiveProject.Type is GameType.EldenRing)
        {
            try
            {
                using BND4 bnd = SFUtil.DecryptERRegulation(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // AC6
        if (Locator.ActiveProject.Type is GameType.ArmoredCoreVI)
        {
            try
            {
                using BND4 bnd = SFUtil.DecryptAC6Regulation(param);
                LoadParamFromBinder(bnd, ref _params, out _paramVersion, true);
            }
            catch (Exception e)
            {
                PlatformUtils.Instance.MessageBox($"Param Load failed: {param}: {e.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private static void LoadParamFromBinder(IBinder parambnd, ref Dictionary<string, Param> paramBank, out ulong version,
        bool checkVersion = false, bool validatePadding = false)
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

            PARAM p;

            p = PARAM.ReadIgnoreCompression(f.Bytes);
            if (!_paramdefs.ContainsKey(p.ParamType ?? ""))
            {
                TaskLogs.AddLog(
                    $"Couldn't find ParamDef for param {paramName} with ParamType \"{p.ParamType}\".",
                    LogLevel.Warning);
                continue;
            }

            if (p.ParamType == null)
            {
                throw new Exception("Param type is unexpectedly null");
            }

            PARAMDEF def = _paramdefs[p.ParamType];
            try
            {
                p.ApplyParamdef(def);
            }
            catch (Exception e)
            {
                var name = f.Name.Split("\\").Last();
                var message = $"Could not apply ParamDef for {name}";

                TaskLogs.AddLog(message,
                        LogLevel.Warning, TaskLogs.LogPriority.Normal, e);
            }

            TaskLogs.AddLog($"{paramName} validated");
        }
    }
}
