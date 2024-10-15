using Silk.NET.OpenGL;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore.ParamEditor;

/// <summary>
///     Utilities for dealing with global paramdefs for a game
/// </summary>
public class MSBBank : DataBank
{

    /// <summary>
    ///     List of msb names
    /// </summary>
    private List<string> _msbIDs = null;

    /// <summary>
    ///     Mapping from msb path|name -> IMsb.
    /// </summary>
    private Dictionary<string, IMsb> _msbs = new();

    public MSBBank(Project project) : base(project, "MSBs")
    {

    }

    protected override void Load()
    {
        //move this functionality to msbbank
        _msbIDs = LoadFullMapList();
    }

    public override void Save()
    {
        //move this functionality to msbbank - requires editing IMsb rather than Map! Current editor is a big liar!
    }

    protected override IEnumerable<StudioResource> GetDependencies(Project project)
    {
        return [];
    }

    public IMsb GetMsb(string mapId)
    {
        if (_msbs.TryGetValue(mapId, out IMsb msb))
        {
            return msb;
        }

        AssetDescription ad = GetMapMSB(mapId);
        if (ad.AssetPath == null)
        {
            return null;
        }
        string path = ad.AssetPath;

        if (Project.Type == GameType.DarkSoulsIII)
        {
            msb = MSB3.Read(path);
        }
        else if (Project.Type == GameType.Sekiro)
        {
            msb = MSBS.Read(path);
        }
        else if (Project.Type == GameType.EldenRing)
        {
            msb = MSBE.Read(path);
        }
        else if (Project.Type == GameType.ArmoredCoreVI)
        {
            msb = MSB_AC6.Read(path);
        }
        else if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            msb = MSB2.Read(path);
        }
        else if (Project.Type == GameType.Bloodborne)
        {
            msb = MSBB.Read(path);
        }
        else if (Project.Type == GameType.DemonsSouls)
        {
            msb = MSBD.Read(path);
        }
        else
        {
            msb = MSB1.Read(path);
        }
        _msbs[mapId] = msb;
        return msb;
    }


    /// <summary>
    ///     Gets the full list of maps in the game (excluding chalice dungeons). Basically if there's an msb for it,
    ///     it will be in this list.
    /// </summary>
    /// <returns></returns>
    public List<string> GetFullMapList()
    {
        if (_msbIDs == null)
            _msbIDs = LoadFullMapList();
        return _msbIDs;
    }
    private List<string> LoadFullMapList()
    {
        HashSet<string> mapSet = new();

        // DS2 has its own structure for msbs, where they are all inside individual folders
        if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            foreach (var map in Project.AssetLocator.GetAllAssets(@"map", [@"*.msb"], true, true))
            {
                mapSet.Add(Path.GetFileNameWithoutExtension(map));
            }
        }
        else
        {
            foreach (var msb in Project.AssetLocator.GetAllAssets(@"map\MapStudio\", [@"*.msb", @"*.msb.dcx"]))
            {
                mapSet.Add(ProjectAssetLocator.GetFileNameWithoutExtensions(msb));
            }
        }
        Regex mapRegex = new(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
        List<string> mapList = mapSet.Where(x => mapRegex.IsMatch(x)).ToList();
        mapList.Sort();
        return mapList;
    }
    public AssetDescription GetMapMSB(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        ad.AssetPath = null;
        if (mapid.Length != 12)
        {
            return ad;
        }

        string preferredPath;
        string backupPath;
        // SOFTS
        if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            preferredPath = $@"map\{mapid}\{mapid}.msb";
            backupPath = $@"map\{mapid}\{mapid}.msb";
        }
        // BB chalice maps
        else if (Project.Type == GameType.Bloodborne && mapid.StartsWith("m29"))
        {
            preferredPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb.dcx";
            backupPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb";
        }
        // DeS, DS1, DS1R
        else if (Project.Type == GameType.DarkSoulsPTDE || Project.Type == GameType.DarkSoulsRemastered ||
                 Project.Type == GameType.DemonsSouls)
        {
            preferredPath = $@"\map\MapStudio\{mapid}.msb";
            backupPath = $@"\map\MapStudio\{mapid}.msb.dcx";
        }
        // BB, DS3, ER, SSDT
        else if (Project.Type == GameType.Bloodborne || Project.Type == GameType.DarkSoulsIII || Project.Type == GameType.EldenRing ||
                 Project.Type == GameType.Sekiro)
        {
            preferredPath = $@"\map\MapStudio\{mapid}.msb.dcx";
            backupPath = $@"\map\MapStudio\{mapid}.msb";
        }
        else
        {
            preferredPath = $@"\map\MapStudio\{mapid}.msb.dcx";
            backupPath = $@"\map\MapStudio\{mapid}.msb";
        }

        

        if (writemode)
        {
            ad.AssetPath = $@"{Project.AssetLocator.RootDirectory}\{preferredPath}";
        }
        else
        {
            ad.AssetPath = Project.AssetLocator.GetAssetPathFromOptions([preferredPath, backupPath]).Item2;
        }

        ad.AssetName = mapid;
        return ad;
    }
}
