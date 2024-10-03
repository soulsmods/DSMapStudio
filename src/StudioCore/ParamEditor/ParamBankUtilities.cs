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

    private Dictionary<string, List<string?>> _storedStrippedRowNames;

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
}
