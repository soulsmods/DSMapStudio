using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudioCore.ParamEditor;

/// <summary>
///     Utilities for dealing with global paramdefs for a game
/// </summary>
public class ParamDefBank : StudioResource
{
    /// <summary>
    ///     Mapping from path -> PARAMDEF for cache and and comparison purposes. TODO: check for paramdef comparisons and evaluate if the file/paramdef was actually the same.
    /// </summary>
    private static readonly Dictionary<string, PARAMDEF> _paramdefsCache = new();

    /// <summary>
    ///     Mapping from ParamType -> PARAMDEF.
    /// </summary>
    private Dictionary<string, PARAMDEF> _paramdefs = new();

    private List<(string, PARAMDEF)> _defPairs = new();

    /// <summary>
    ///     Mapping from Param filename -> Manual ParamType.
    ///     This is for params with no usable ParamType at some particular game version.
    ///     By convention, ParamTypes ending in "_TENTATIVE" do not have official data to reference.
    /// </summary>
    private Dictionary<string, string> _tentativeParamType;

    public ParamDefBank() : base(Locator.ActiveProject.Type, "Paramdefs")
    {
    }

    public Dictionary<string, PARAMDEF> GetParamDefs()
    {
        return _paramdefs;
    }

    public List<(string, PARAMDEF)> GetParamDefByFileNames()
    {
        return _defPairs;
    }

    public Dictionary<string, string> GetTentativeParamTypes()
    {
        return _tentativeParamType;
    }

    private void LoadParamdefs()
    {
        _paramdefs = new Dictionary<string, PARAMDEF>();
        _tentativeParamType = new Dictionary<string, string>();
        var files = Locator.ActiveProject.AssetLocator.GetAllProjectFiles($@"Paramdex\{AssetUtils.GetGameIDForDir(Locator.ActiveProject.Type)}\Defs", ["*.xml"], true, false);
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

        var tentativeMappingPath = Locator.ActiveProject.AssetLocator.GetProjectFilePath($@"{Locator.ActiveProject.AssetLocator.GetParamdexDir()}\Defs\TentativeParamType.csv");
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

        _defPairs = defPairs;
    }
    protected override void Load()
    {
        LoadParamdefs();
    }

    protected override IEnumerable<StudioResource> GetDependencies(Project project)
    {
        return [];
    }
}
