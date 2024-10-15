using SoulsFormats;
using System.Collections.Generic;

namespace StudioCore.ParamEditor;

/// <summary>
///     Utilities for dealing with global paramdefs for a game
/// </summary>
public class ParamMetaBank : StudioResource
{
    public Dictionary<PARAMDEF, ParamMetaData> ParamMetas = new();

    public ParamMetaBank() : base(Locator.ActiveProject.Type, "ParamMetas")
    {
    }
    public void LoadParamMeta()
    {
        List<(string, PARAMDEF)> defPairs = ResDirectory.CurrentGame.ParamDefBank.GetParamDefByFileNames();
        var mdir = Locator.ActiveProject.AssetLocator.GetProjectFilePath($@"{Locator.ActiveProject.AssetLocator.GetParamdexDir()}\Meta");
        foreach ((var f, PARAMDEF pdef) in defPairs)
        {
            var fName = f.Substring(f.LastIndexOf('\\') + 1);
            var md = ParamMetaData.XmlDeserialize($@"{mdir}\{fName}", pdef);
            ParamMetas.Add(pdef, md);
        }
    }
    protected override void Load()
    {
        LoadParamMeta();
    }

    protected override IEnumerable<StudioResource> GetDependencies(Project project)
    {
        return [ResDirectory.CurrentGame.ParamDefBank];
    }
}
