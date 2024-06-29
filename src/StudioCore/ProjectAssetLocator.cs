using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore;

/// <summary>
///     Exposes an interface to retrieve game assets from the various souls games.
/// </summary>
public class ProjectAssetLocator
{
    public readonly Project Project;

    public GameType Type => Project.Type;
    public ProjectAssetLocator ParentAssetLocator => Project.ParentProject?.AssetLocator;

    /// <summary>
    ///     The root directory where all the assets are
    /// </summary>
    public readonly string RootDirectory;

    /// <summary>
    ///     Directory where misc DSMapStudio files associated with a project are stored.
    /// </summary>
    public string ProjectMiscDir => @$"{RootDirectory}\DSMapStudio";

    private List<string> MapList;

    public ProjectAssetLocator(Project owner, string dir)
    {
        Project = owner;
        RootDirectory = dir;
    }

    private string GetFileNameWithoutExtensions(string path)
    {
        return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
    }

    public string GetAssetPath(string relpath)
    {
        if (File.Exists($@"{RootDirectory}\{relpath}"))
        {
            return $@"{RootDirectory}\{relpath}";
        }
        if (ParentAssetLocator != null)
        {
            return ParentAssetLocator.GetAssetPath(relpath);
        }
        return null;
    }
    public string GetProjectFilePath(string relpath)
    {
        if (ParentAssetLocator == null)
        {
            return $@"Assets\{relpath}";
        }
        string path = $@"{ProjectMiscDir}\{relpath}";
        if (File.Exists(path))
        {
            return path;
        }
        else
        {
            return ParentAssetLocator.GetProjectFilePath(relpath);
        }
    }

    public IEnumerable<string> GetProjectFileAllPaths(string relpath, List<string> paths = null)
    {
        if (paths == null)
        {
            paths = new();
        }
        if (ParentAssetLocator == null)
        {
            paths.Add($@"Assets\{relpath}");
            return paths;
        }
        string path = $@"{ProjectMiscDir}\{relpath}";
        if (File.Exists(path))
        {
            paths.Add(path);
        }
        return ParentAssetLocator.GetProjectFileAllPaths(relpath, paths);;
    }
    
    public (int, string) GetAssetPathFromOptions(IEnumerable<string> relpaths)
    {
        int i = 0;
        foreach (string relpath in relpaths)
        {
            var path = $@"{RootDirectory}\{relpath}";
            if (File.Exists(path))
            {
                return (i, path);
            }
            i++;
        }
        if (ParentAssetLocator != null)
        {
            return ParentAssetLocator.GetAssetPathFromOptions(relpaths);
        }
        return (-1, null);
    }

    public bool FileExists(string relpath)
    {
        if (ParentAssetLocator != null && ParentAssetLocator.FileExists(relpath))
        {
            return true;
        }

        if (File.Exists($@"{RootDirectory}\{relpath}"))
        {
            return true;
        }

        return false;
    }
    public IEnumerable<string> GetAllAssets(string relpath, string[] extensionPatterns, bool distinct = true, bool subDirectories = false)
    {
        List<string> files = new();
        string rpath = $@"{RootDirectory}\{relpath}";
        if (Directory.Exists(rpath))
        {
            foreach (string pattern in extensionPatterns)
            {
                files.AddRange(Directory.GetFiles(rpath, pattern, subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            }
        }
        if (ParentAssetLocator != null)
        {
            files.AddRange(ParentAssetLocator.GetAllAssets(relpath, extensionPatterns, distinct, subDirectories));
        }
        if (distinct)
        {
            // WARNING DistinctBy doesn't guarantee preference by order, though implementations do
            return files.DistinctBy(GetFileNameWithoutExtensions);
        }
        else
        {
            return files;
        }
    }
    public IEnumerable<string> GetAllProjectFiles(string relpath, string[] extensionPatterns, bool distinct = true, bool subDirectories = false)
    {
        List<string> files = new();
        string rpath;
        if (ParentAssetLocator == null)
        {
            rpath = $@"Assets\{relpath}";
            foreach (string pattern in extensionPatterns)
            {
                files.AddRange(Directory.GetFiles(rpath, pattern, subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            }
            return files;
        }
        rpath = $@"{RootDirectory}\{ProjectMiscDir}\{relpath}";
        if (Directory.Exists(rpath))
        {
            foreach (string pattern in extensionPatterns)
            {
                files.AddRange(Directory.GetFiles(rpath, pattern, subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            }
        }
        if (ParentAssetLocator != null)
        {
            files.AddRange(ParentAssetLocator.GetAllProjectFiles(relpath, extensionPatterns, distinct, subDirectories));
        }
        if (distinct)
        {
            // WARNING DistinctBy doesn't guarantee preference by order, though implementations do
            return files.DistinctBy(GetFileNameWithoutExtensions);
        }
        else
        {
            return files;
        }
    }
    public IEnumerable<string> GetAllSubDirs(string relpath, bool distinct = true, bool subDirectories = false)
    {
        List<string> dirs = new();
        string rpath = $@"{RootDirectory}\{relpath}";
        if (Directory.Exists(rpath))
        {
            dirs.AddRange(Directory.GetDirectories(rpath, "*", subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
        }
        if (ParentAssetLocator != null)
        {
            dirs.AddRange(ParentAssetLocator.GetAllSubDirs(relpath, subDirectories));
        }
        if (distinct)
        {
            // WARNING DistinctBy doesn't guarantee preference by order, though implementations do
            return dirs.DistinctBy(Path.GetFileName);
        }
        else
        {
            return dirs;
        }
    }

    /// <summary>
    ///     Gets the full list of maps in the game (excluding chalice dungeons). Basically if there's an msb for it,
    ///     it will be in this list.
    /// </summary>
    /// <returns></returns>
    public List<string> GetFullMapList()
    {

        if (MapList != null)
        {
            return MapList;
        }

        try
        {
            HashSet<string> mapSet = new();

            // DS2 has its own structure for msbs, where they are all inside individual folders
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                foreach (var map in GetAllAssets(@"map", [@"*.msb"], true, true))
                {
                    mapSet.Add(Path.GetFileNameWithoutExtension(map));
                }
            }
            else
            {
                foreach (var msb in GetAllAssets(@"map\MapStudio\", [@"*.msb", @"*.msb.dcx"]))
                {
                    mapSet.Add(GetFileNameWithoutExtensions(msb));
                }
            }
            Regex mapRegex = new(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
            List<string> mapList = mapSet.Where(x => mapRegex.IsMatch(x)).ToList();
            mapList.Sort();
            MapList = mapList;
            return MapList;
        }
        catch (DirectoryNotFoundException e)
        {
            // Game is likely not UXM unpacked
            if (ParentAssetLocator != null)
            {
                MapList = ParentAssetLocator.GetFullMapList();
                return MapList;
            }
            return new List<string>();
        }
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
        if (Type == GameType.DarkSoulsIISOTFS)
        {
            preferredPath = $@"map\{mapid}\{mapid}.msb";
            backupPath = $@"map\{mapid}\{mapid}.msb";
        }
        // BB chalice maps
        else if (Type == GameType.Bloodborne && mapid.StartsWith("m29"))
        {
            preferredPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb.dcx";
            backupPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb";
        }
        // DeS, DS1, DS1R
        else if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered ||
                 Type == GameType.DemonsSouls)
        {
            preferredPath = $@"\map\MapStudio\{mapid}.msb";
            backupPath = $@"\map\MapStudio\{mapid}.msb.dcx";
        }
        // BB, DS3, ER, SSDT
        else if (Type == GameType.Bloodborne || Type == GameType.DarkSoulsIII || Type == GameType.EldenRing ||
                 Type == GameType.Sekiro)
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
            ad.AssetPath = $@"{RootDirectory}\{preferredPath}";
        }
        else
        {
            ad.AssetPath = GetAssetPathFromOptions([preferredPath, backupPath]).Item2;
        }

        ad.AssetName = mapid;
        return ad;
    }

    public List<AssetDescription> GetMapBTLs(string mapid, bool writemode = false)
    {
        List<AssetDescription> adList = new();
        if (mapid.Length != 12)
        {
            return adList;
        }

        if (Type is GameType.DarkSoulsIISOTFS)
        {
            // DS2 BTL is located inside map's .gibdt file
            AssetDescription ad = new();
            var path = $@"model\map\g{mapid[1..]}.gibhd";

            if (writemode)
            {
                ad.AssetPath = $@"{RootDirectory}\{path}";
            }
            else
            {
                ad.AssetPath = GetAssetPath(path);
            }

            if (ad.AssetPath != null)
            {
                ad.AssetName = $@"g{mapid[1..]}";
                ad.AssetVirtualPath = $@"{mapid}\light.btl.dcx";
                adList.Add(ad);
            }

            AssetDescription ad2 = new();
            path = $@"model_lq\map\g{mapid[1..]}.gibhd";

            if (writemode)
            {
                ad2.AssetPath = $@"{RootDirectory}\{path}";
            }
            else
            {
                ad2.AssetPath = GetAssetPath(path);
            }

            if (ad2.AssetPath != null)
            {
                ad2.AssetName = $@"g{mapid[1..]}_lq";
                ad2.AssetVirtualPath = $@"{mapid}\light.btl.dcx";
                adList.Add(ad2);
            }
        }
        else if (Type is GameType.Bloodborne or GameType.DarkSoulsIII or GameType.Sekiro or GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            string path;
            if (Type is GameType.EldenRing or GameType.ArmoredCoreVI)
            {
                path = $@"map\{mapid[..3]}\{mapid}";
            }
            else
            {
                path = $@"map\{mapid}";
            }

            List<string> files = new();

            files = GetAllAssets(path, ["*.btl", "*.btl.dcx"]).ToList();

            foreach (var file in files)
            {
                AssetDescription ad = new();
                var fileName = file.Split("\\").Last();
                if (writemode)
                {
                    ad.AssetPath = $@"{RootDirectory}\{path}\{fileName}";
                }
                else
                {
                     ad.AssetPath = GetAssetPath($@"{path}\{fileName}");
                }

                if (ad.AssetPath != null)
                {
                    ad.AssetName = fileName;
                    adList.Add(ad);
                }
            }
        }

        return adList;
    }
    public AssetDescription GetMapNVA(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        ad.AssetPath = null;
        if (mapid.Length != 12)
        {
            return ad;
        }
        // BB chalice maps

        if (Type == GameType.Bloodborne && mapid.StartsWith("m29"))
        {
            var path = $@"\map\{mapid.Substring(0, 9)}_00\{mapid}";
            if (writemode)
            {
                ad.AssetPath = $@"{RootDirectory}\{path}.nva.dcx";
            }
            else
            {
                ad.AssetPath = GetAssetPath($@"{path}.nva.dcx");
            }
        }
        else if (Type == GameType.DarkSoulsPTDE)
        {
            var path = $@"\map\{mapid}\{mapid}";
            if (writemode)
            {
                ad.AssetPath = $@"{RootDirectory}\{path}.nva";
            }
            else
            {
                ad.AssetPath = GetAssetPathFromOptions([$@"{path}.nva", $@"{path}.nva.dcx"]).Item2;
            }
        }
        else
        {
            var path = $@"\map\{mapid}\{mapid}";
            if (writemode)
            {
                ad.AssetPath = $@"{RootDirectory}\{path}.nva.dcx";
            }
            else
            {
                ad.AssetPath = GetAssetPathFromOptions([$@"{path}.nva.dcx", $@"{path}.nva"]).Item2;
            }
        }

        ad.AssetName = mapid;
        return ad;
    }

    public AssetDescription GetWorldLoadListList(bool writemode = false)
    {
        AssetDescription ad = new();

        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\map\worldmsblist.worldloadlistlist.dcx";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"map\worldmsblist.worldloadlistlist.dcx");
        }
        
        return ad;
    }

    /// <summary>
    ///     Get folders with msgbnds used in-game
    /// </summary>
    /// <returns>Dictionary with language name and path</returns>
    public Dictionary<string, string> GetMsgLanguages()
    {
        Dictionary<string, string> dict = new();
        List<string> folders = new();
        try
        {
            if (Type == GameType.DemonsSouls)
            {
                folders = GetAllSubDirs(@"\msg").ToList();
                // Japanese uses root directory
                if (FileExists(@"\msg\menu.msgbnd.dcx") || FileExists(@"\msg\item.msgbnd.dcx"))
                {
                    dict.Add("Japanese", "");
                }
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                folders = GetAllSubDirs(@"\menu\text").ToList();
            }
            else
            {
                // Exclude folders that don't have typical msgbnds
                folders = GetAllSubDirs(@"\msg")
                    .Where(x => !"common,as,eu,jp,na,uk,japanese".Contains(Path.GetFileName(x))).ToList();
            }

            foreach (var path in folders)
            {
                dict.Add(path.Split("\\").Last(), path);
            }
        }
        catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
        {
        }

        return dict;
    }

    /// <summary>
    ///     Get path of item.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetItemMsgbnd(string langFolder, bool writemode = false)
    {
        return GetMsgbnd("item", langFolder, writemode);
    }

    /// <summary>
    ///     Get path of menu.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetMenuMsgbnd(string langFolder, bool writemode = false)
    {
        return GetMsgbnd("menu", langFolder, writemode);
    }

    public AssetDescription GetMsgbnd(string msgBndType, string langFolder, bool writemode = false)
    {
        AssetDescription ad = new();
        IEnumerable<string> path;
        if (Project.Type == GameType.DemonsSouls)
        {
            // Demon's Souls has msgbnds directly in the msg folder
            path = [$@"msg\{langFolder}\{msgBndType}.msgbnd.dcx", $@"msg\{msgBndType}.msgbnd.dcx"];
        }
        else if (Project.Type == GameType.DarkSoulsPTDE)
        {
            path = [$@"msg\{langFolder}\{msgBndType}.msgbnd"];
        }
        else if (Project.Type == GameType.DarkSoulsRemastered)
        {
            path = [$@"msg\{langFolder}\{msgBndType}.msgbnd.dcx"];
        }
        else if (Project.Type == GameType.DarkSoulsIISOTFS)
        {
            // DS2 does not have an msgbnd but loose fmg files instead
            path = [$@"menu\text\{langFolder}"];
        }
        else if (Project.Type == GameType.DarkSoulsIII)
        {
            path = [$@"msg\{langFolder}\{msgBndType}_dlc2.msgbnd.dcx"];
        }
        else
        {
            path = [$@"msg\{langFolder}\{msgBndType}.msgbnd.dcx"];
        }

        if (writemode)
        {
            ad.AssetPath = $@"{Project.AssetLocator.RootDirectory}\{path.First()}";
        }
        else
        {
            ad.AssetPath = Project.AssetLocator.GetAssetPathFromOptions(path).Item2;
        }

        return ad;
    }

    public string GetAliasAssetsDir()
    {
        return GetProjectFilePath($@"Aliases");
    }

    public string GetParamdexDir()
    {
        return $@"Paramdex\{AssetUtils.GetGameIDForDir(Type)}";
    }

    public string GetStrippedRowNamesPath(string paramName)
    {
        return GetProjectFilePath($@"Stripped Row Names\{paramName}.txt");
    }

    public string GetScriptAssetsCommonDir()
    {
        return GetProjectFilePath($@"MassEditScripts\Common");
    }

    public string GetScriptAssetsDir()
    {
        return GetProjectFilePath($@"MassEditScripts\{AssetUtils.GetGameIDForDir(Type)}");
    }

    public string GetUpgraderAssetsDir()
    {
        return GetProjectFilePath($@"{GetParamdexDir()}\Upgrader");
    }

    public string GetGameOffsetsAssetsDir()
    {
        return GetProjectFilePath($@"GameOffsets\{AssetUtils.GetGameIDForDir(Type)}");
    }

    public PARAMDEF GetParamdefForParam(string paramType)
    {
        //This is ds2 only and really ds2 should be using parambank params not its own copies
        PARAMDEF pd = PARAMDEF.XmlDeserialize($@"{GetParamdexDir()}\{AssetUtils.GetGameIDForDir(Type)}\Defs\{paramType}.xml");
        return pd;
    }

    public AssetDescription GetDS2GeneratorParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\generatorparam_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_generators";
        return ad;
    }

    public AssetDescription GetDS2GeneratorLocationParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\generatorlocation_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_generator_locations";
        return ad;
    }

    public AssetDescription GetDS2GeneratorRegistParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\generatorregistparam_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_generator_registrations";
        return ad;
    }

    public AssetDescription GetDS2EventParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\eventparam_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_event_params";
        return ad;
    }

    public AssetDescription GetDS2EventLocationParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\eventlocation_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_event_locations";
        return ad;
    }

    public AssetDescription GetDS2ObjInstanceParam(string mapid, bool writemode = false)
    {
        AssetDescription ad = new();
        var path = $@"Param\mapobjectinstanceparam_{mapid}";
        if (writemode)
        {
            ad.AssetPath = $@"{RootDirectory}\{path}.param";
        }
        else
        {
            ad.AssetPath = GetAssetPath($@"{path}.param");
        }

        ad.AssetName = mapid + "_object_instance_params";
        return ad;
    }

    // Used to get the map model list from within the mapbhd/bdt
    public List<AssetDescription> GetMapModelsFromBXF(string mapid)
    {
        List<AssetDescription> ret = new();

        if (Locator.AssetLocator.Type is GameType.DarkSoulsIISOTFS)
        {
            var path = GetAssetPath($@"model/map/{mapid}.mapbdt");

            if (path != null)
            {
                var bdtPath = path;
                var bhdPath = path.Replace("bdt", "bhd");

                var bxf = BXF4.Read(bhdPath, bdtPath);

                if (bxf != null)
                {
                    foreach (var file in bxf.Files)
                    {
                        if (file.Name.Contains(".flv"))
                        {
                            var name = GetFileNameWithoutExtensions(file.Name);

                            AssetDescription ad = new();
                            ad.AssetName = name;
                            ad.AssetArchiveVirtualPath = $@"map/{name}/model/";

                            ret.Add(ad);
                        }
                    }
                }
            }
        }

        return ret;
    }

    public List<AssetDescription> GetMapModels(string mapid)
    {
        List<AssetDescription> ret = new();
        if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
        {
            foreach (var f in GetAllAssets($@"\map\{mapid}\", [@"*.mapbnd.dcx"]))
            {
                AssetDescription ad = new();
                ad.AssetPath = f;
                var name = GetFileNameWithoutExtensions(f);
                ad.AssetName = name;
                ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                ret.Add(ad);
            }
        }
        else if (Type == GameType.DarkSoulsIISOTFS)
        {
            AssetDescription ad = new();
            var name = mapid;
            ad.AssetName = name;
            ad.AssetArchiveVirtualPath = $@"map/{mapid}/model";
            ret.Add(ad);
        }
        else if (Type == GameType.EldenRing)
        {
            var mapPath = RootDirectory + $@"\map\{mapid[..3]}\{mapid}";
            foreach (var f in GetAllAssets(mapPath, [@"*.mapbnd.dcx"]))
            {
                AssetDescription ad = new();
                ad.AssetPath = f;
                var name = GetFileNameWithoutExtensions(f);
                ad.AssetName = name;
                ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                ret.Add(ad);
            }
        }
        else if (Type == GameType.ArmoredCoreVI)
        {
            var mapPath = RootDirectory + $@"\map\{mapid[..3]}\{mapid}";
            foreach (var f in GetAllAssets(mapPath, [@"*.mapbnd.dcx"]))
            {
                AssetDescription ad = new();
                ad.AssetPath = f;
                var name = GetFileNameWithoutExtensions(f);
                ad.AssetName = name;
                ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                ret.Add(ad);
            }
        }
        else
        {
            if (!Directory.Exists(RootDirectory + $@"\map\{mapid}\"))
            {
                return ret;
            }

            var ext = Type == GameType.DarkSoulsPTDE ? @"*.flver" : @"*.flver.dcx";
            List<string> mapfiles = Directory.GetFileSystemEntries(RootDirectory + $@"\map\{mapid}\", ext)
                .ToList();
            foreach (var f in mapfiles)
            {
                AssetDescription ad = new();
                ad.AssetPath = f;
                var name = GetFileNameWithoutExtensions(f);
                ad.AssetName = name;
                // ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                ret.Add(ad);
            }
        }

        return ret;
    }
    public string MapModelNameToAssetName(string mapid, string modelname)
    {
        if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered)
        {
            return $@"{modelname}A{mapid.Substring(1, 2)}";
        }

        if (Type == GameType.DemonsSouls)
        {
            return $@"{modelname}";
        }

        if (Type == GameType.DarkSoulsIISOTFS)
        {
            return modelname;
        }

        return $@"{mapid}_{modelname.Substring(1)}";
    }

    /// <summary>
    ///     Gets the adjusted map ID that contains all the map assets
    /// </summary>
    /// <param name="mapid">The msb map ID to adjust</param>
    /// <returns>The map ID for the purpose of asset storage</returns>
    public string GetAssetMapID(string mapid)
    {
        if (Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            return mapid;
        }

        if (Type is GameType.DarkSoulsRemastered)
        {
            if (mapid.StartsWith("m99"))
            {
                // DSR m99 maps contain their own assets
                return mapid;
            }
        }
        else if (Type is GameType.DemonsSouls)
        {
            return mapid;
        }
        else if (Type is GameType.Bloodborne)
        {
            if (mapid.StartsWith("m29"))
            {
                // Special case for chalice dungeon assets
                return "m29_00_00_00";
            }
        }

        // Default
        return mapid.Substring(0, 6) + "_00_00";
    }
    public AssetDescription GetMapModel(string mapid, string model)
    {
        AssetDescription ret = new();
        if (Type == GameType.DarkSoulsPTDE || Type == GameType.Bloodborne || Type == GameType.DemonsSouls)
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.flver");
        }
        else if (Type == GameType.DarkSoulsRemastered)
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.flver.dcx");
        }
        else if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetPath = GetAssetPath($@"model\map\{mapid}.mapbhd");
        }
        else if (Type == GameType.EldenRing)
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid[..3]}\{mapid}\{model}.mapbnd.dcx");
        }
        else if (Type == GameType.ArmoredCoreVI)
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid[..3]}\{mapid}\{model}.mapbnd.dcx");
        }
        else
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.mapbnd.dcx");
        }

        ret.AssetName = model;
        if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetArchiveVirtualPath = $@"map/{mapid}/model";
            ret.AssetVirtualPath = $@"map/{mapid}/model/{model}.flv.dcx";
        }
        else
        {
            if (Type is not GameType.DemonsSouls
                and not GameType.DarkSoulsPTDE
                and not GameType.DarkSoulsRemastered
                and not GameType.Bloodborne)
            {
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/model/{model}";
            }

            ret.AssetVirtualPath = $@"map/{mapid}/model/{model}/{model}.flver";
        }

        return ret;
    }

    public AssetDescription GetMapCollisionModel(string mapid, string model, bool hi = true)
    {
        AssetDescription ret = new();
        if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls)
        {
            if (hi)
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.hkx");
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx";
            }
            else
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\l{model.Substring(1)}.hkx");
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/lo/l{model.Substring(1)}.hkx";
            }
        }
        else if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetPath = GetAssetPath($@"model\map\h{mapid.Substring(1)}.hkxbhd");
            ret.AssetName = model;
            ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx.dcx";
            ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
        }
        else if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
        {
            if (hi)
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\h{mapid.Substring(1)}.hkxbhd");
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/h{model.Substring(1)}.hkx.dcx";
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
            }
            else
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\l{mapid.Substring(1)}.hkxbhd");
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/lo/l{model.Substring(1)}.hkx.dcx";
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/lo";
            }
        }
        else
        {
            return AssetUtils.GetNullAsset();
        }

        return ret;
    }

    public List<AssetDescription> GetMapTextures(string mapid)
    {
        List<AssetDescription> ads = new();

        if (Type == GameType.DarkSoulsIISOTFS)
        {
            AssetDescription t = new();
            t.AssetPath = GetAssetPath($@"model\map\t{mapid.Substring(1)}.tpfbhd");
            t.AssetArchiveVirtualPath = $@"map/tex/{mapid}/tex";
            ads.Add(t);
        }
        else if (Type == GameType.DarkSoulsPTDE)
        {
            // TODO
        }
        else if (Type == GameType.EldenRing)
        {
            // TODO ER
        }
        else if (Type == GameType.ArmoredCoreVI)
        {
            // TODO AC6
        }
        else if (Type == GameType.DemonsSouls)
        {
            var mid = mapid.Substring(0, 3);
            var paths = Directory.GetFileSystemEntries($@"{RootDirectory}\map\{mid}\", "*.tpf.dcx");
            foreach (var path in paths)
            {
                AssetDescription ad = new();
                ad.AssetPath = path;
                var tid = Path.GetFileNameWithoutExtension(path).Substring(4, 4);
                ad.AssetVirtualPath = $@"map/tex/{mid}/{tid}";
                ads.Add(ad);
            }
        }
        else
        {
            // Clean this up. Even if it's common code having something like "!=Sekiro" can lead to future issues
            var mid = mapid.Substring(0, 3);

            AssetDescription t0000 = new();
            t0000.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0000.tpfbhd");
            t0000.AssetArchiveVirtualPath = $@"map/tex/{mid}/0000";
            ads.Add(t0000);

            AssetDescription t0001 = new();
            t0001.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0001.tpfbhd");
            t0001.AssetArchiveVirtualPath = $@"map/tex/{mid}/0001";
            ads.Add(t0001);

            AssetDescription t0002 = new();
            t0002.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0002.tpfbhd");
            t0002.AssetArchiveVirtualPath = $@"map/tex/{mid}/0002";
            ads.Add(t0002);

            AssetDescription t0003 = new();
            t0003.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0003.tpfbhd");
            t0003.AssetArchiveVirtualPath = $@"map/tex/{mid}/0003";
            ads.Add(t0003);

            if (Type == GameType.DarkSoulsRemastered)
            {
                AssetDescription env = new();
                env.AssetPath = GetAssetPath($@"map\{mid}\GI_EnvM_{mid}.tpfbhd");
                env.AssetArchiveVirtualPath = $@"map/tex/{mid}/env";
                ads.Add(env);
            }
            else if (Type == GameType.Bloodborne || Type == GameType.DarkSoulsIII)
            {
                AssetDescription env = new();
                env.AssetPath = GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx");
                env.AssetVirtualPath = $@"map/tex/{mid}/env";
                ads.Add(env);
            }
            else if (Type == GameType.Sekiro)
            {
                //TODO SDT
            }
        }

        return ads;
    }
    public List<string> GetEnvMapTextureNames(string mapid)
    {
        List<string> l = new();
        if (Type == GameType.DarkSoulsIII)
        {
            var mid = mapid.Substring(0, 3);
            if (File.Exists(GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx")))
            {
                TPF t = TPF.Read(GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx"));
                foreach (TPF.Texture tex in t.Textures)
                {
                    l.Add(tex.Name);
                }
            }
        }

        return l;
    }

    private string GetChrTexturePath(string chrid)
    {
        if (Type is GameType.DemonsSouls)
        {
            return GetAssetPath($@"chr\{chrid}\{chrid}.tpf");
        }

        if (Type is GameType.DarkSoulsPTDE)
        {
            var path = GetAssetPath($@"chr\{chrid}\{chrid}.tpf");
            if (path != null)
            {
                return path;
            }

            return GetAssetPath($@"chr\{chrid}.chrbnd");
        }

        if (Type is GameType.DarkSoulsIISOTFS)
        {
            return GetAssetPath($@"model\chr\{chrid}.texbnd");
        }

        if (Type is GameType.DarkSoulsRemastered)
        {
            // TODO: Some textures require getting chrtpfbhd from chrbnd, then using it with chrtpfbdt in chr folder.
            return GetAssetPath($@"chr\{chrid}.chrbnd");
        }

        if (Type is GameType.Bloodborne)
        {
            return GetAssetPath($@"chr\{chrid}_2.tpf.dcx");
        }

        if (Type is GameType.DarkSoulsIII or GameType.Sekiro)
        {
            return GetAssetPath($@"chr\{chrid}.texbnd.dcx");
        }

        if (Type is GameType.EldenRing)
        {
            // TODO: Maybe add an option down the line to load lower quality
            return GetAssetPath($@"chr\{chrid}_h.texbnd.dcx");
        }

        if (Type is GameType.ArmoredCoreVI)
        {
            return GetAssetPath($@"chr\{chrid}.texbnd.dcx");
        }

        return null;
    }

    public AssetDescription GetChrTextures(string chrid)
    {
        AssetDescription ad = new();
        ad.AssetArchiveVirtualPath = null;
        ad.AssetPath = null;
        if (Type is GameType.DemonsSouls)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"chr/{chrid}/tex";
            }
        }
        else if (Type is GameType.DarkSoulsPTDE)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad.AssetPath = path;
                if (path.EndsWith(".chrbnd"))
                {
                    ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
                }
                else
                {
                    ad.AssetVirtualPath = $@"chr/{chrid}/tex";
                }
            }
        }
        else if (Type is GameType.DarkSoulsRemastered)
        {
            // TODO: Some textures require getting chrtpfbhd from chrbnd, then using it with chrtpfbdt in chr folder.
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad = new AssetDescription();
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"chr/{chrid}/tex";
            }
        }
        else if (Type is GameType.DarkSoulsIISOTFS)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad = new AssetDescription();
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"chr/{chrid}/tex";
            }
        }
        else if (Type is GameType.Bloodborne)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"chr/{chrid}/tex";
            }
        }
        else if (Type is GameType.DarkSoulsIII or GameType.Sekiro)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
            }
        }
        else if (Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            var path = GetChrTexturePath(chrid);
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
            }
        }

        return ad;
    }
    public AssetDescription GetMapNVMModel(string mapid, string model)
    {
        AssetDescription ret = new();
        if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered || Type == GameType.DemonsSouls)
        {
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.nvm");
            ret.AssetName = model;
            ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
            ret.AssetVirtualPath = $@"map/{mapid}/nav/{model}.nvm";
        }
        else
        {
            return AssetUtils.GetNullAsset();
        }

        return ret;
    }

    public AssetDescription GetHavokNavmeshes(string mapid)
    {
        AssetDescription ret = new();
        ret.AssetPath = GetAssetPath($@"map\{mapid}\{mapid}.nvmhktbnd.dcx");
        ret.AssetName = mapid;
        ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
        return ret;
    }

    public AssetDescription GetHavokNavmeshModel(string mapid, string model)
    {
        AssetDescription ret = new();
        ret.AssetPath = GetAssetPath($@"map\{mapid}\{mapid}.nvmhktbnd.dcx");
        ret.AssetName = model;
        ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
        ret.AssetVirtualPath = $@"map/{mapid}/nav/{model}.hkx";

        return ret;
    }

    public List<string> GetChrModels()
    {
        try
        {
            HashSet<string> chrs = new();
            List<string> ret = new();

            var modelDir = @"\chr";
            var modelExt = @".chrbnd.dcx";
            if (Type == GameType.DarkSoulsPTDE)
            {
                modelExt = ".chrbnd";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                modelDir = @"\model\chr";
                modelExt = ".bnd";
            }

            if (Type == GameType.DemonsSouls)
            {
                foreach (var f in GetAllSubDirs(modelDir).Select(Path.GetFileName).Where(x => x.StartsWith("c")))
                {
                    ret.Add(f);
                }

                return ret;
            }

            foreach (var f in GetAllAssets(modelDir, [$@"*{modelExt}"]))
            {
                var name = GetFileNameWithoutExtensions(f);
                ret.Add(name);
                chrs.Add(name);
            }

            return ret;
        }
        catch (DirectoryNotFoundException e)
        {
            // Game likely isn't UXM unpacked
            return new List<string>();
        }
    }
    public AssetDescription GetChrModel(string chr)
    {
        AssetDescription ret = new();
        ret.AssetName = chr;
        ret.AssetArchiveVirtualPath = $@"chr/{chr}/model";
        if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetVirtualPath = $@"chr/{chr}/model/{chr}.flv";
        }
        else
        {
            ret.AssetVirtualPath = $@"chr/{chr}/model/{chr}.flver";
        }

        return ret;
    }

    public List<string> GetObjModels()
    {
        try
        {
            HashSet<string> objs = new();
            List<string> ret = new();

            var modelDir = @"\obj";
            var modelExt = @".objbnd.dcx";
            if (Type == GameType.DarkSoulsPTDE)
            {
                modelExt = ".objbnd";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                modelDir = @"\model\obj";
                modelExt = ".bnd";
            }
            else if (Type == GameType.EldenRing)
            {
                // AEGs are objs in my heart :(
                modelDir = @"\asset\aeg";
                modelExt = ".geombnd.dcx";
            }
            else if (Type == GameType.ArmoredCoreVI)
            {
                // AEGs are objs in my heart :(
                modelDir = @"\asset\environment\geometry";
                modelExt = ".geombnd.dcx";
            }

            // Directories to search for obj models
            List<string> searchDirs = new();
            if (Type == GameType.EldenRing)
            {
                searchDirs = GetAllSubDirs(modelDir).Where(x => GetFileNameWithoutExtensions(x).StartsWith("aeg")).ToList();
            }
            else
            {
                searchDirs.Add(GetAssetPath(modelDir));
            }

            foreach (var searchDir in searchDirs)
            {
                foreach (var f in GetAllAssets(searchDir, [$@"*{modelExt}"]))
                {
                    var name = GetFileNameWithoutExtensions(f);
                    ret.Add(name);
                    objs.Add(name);
                }
            }

            return ret;
        }
        catch (DirectoryNotFoundException e)
        {
            // Game likely isn't UXM unpacked
            return new List<string>();
        }
    }

    public AssetDescription GetObjModel(string obj)
    {
        AssetDescription ret = new();
        ret.AssetName = obj;
        ret.AssetArchiveVirtualPath = $@"obj/{obj}/model";
        if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetVirtualPath = $@"obj/{obj}/model/{obj}.flv";
        }
        else if (Type is GameType.EldenRing or GameType.ArmoredCoreVI)
        {
            ret.AssetVirtualPath = $@"obj/{obj}/model/{obj.ToUpper()}.flver";
        }
        else
        {
            ret.AssetVirtualPath = $@"obj/{obj}/model/{obj}.flver";
        }

        return ret;
    }

    public AssetDescription GetObjTexture(string obj)
    {
        AssetDescription ad = new();
        ad.AssetPath = null;
        ad.AssetArchiveVirtualPath = null;
        string path = null;
        if (Type == GameType.DarkSoulsPTDE)
        {
            path = GetAssetPath($@"obj\{obj}.objbnd");
        }
        else if (Type is GameType.DemonsSouls or GameType.DarkSoulsRemastered or GameType.Bloodborne
                 or GameType.DarkSoulsIII or GameType.Sekiro)
        {
            path = GetAssetPath($@"obj\{obj}.objbnd.dcx");
        }

        if (path != null)
        {
            ad.AssetPath = path;
            ad.AssetArchiveVirtualPath = $@"obj/{obj}/tex";
        }

        return ad;
    }

    public AssetDescription GetAetTexture(string aetid)
    {
        AssetDescription ad = new();
        ad.AssetPath = null;
        ad.AssetArchiveVirtualPath = null;
        string path;
        if (Type == GameType.EldenRing)
        {
            path = GetAssetPath($@"asset\aet\{aetid.Substring(0, 6)}\{aetid}.tpf.dcx");
        }
        else if (Type is GameType.ArmoredCoreVI)
        {
            path = GetAssetPath($@"\asset\environment\texture\{aetid}.tpf.dcx");
        }
        else
        {
            throw new NotSupportedException();
        }

        if (path != null)
        {
            ad.AssetPath = path;
            ad.AssetArchiveVirtualPath = $@"aet/{aetid}/tex";
        }

        return ad;
    }
    public List<string> GetPartsModels()
    {
        try
        {
            HashSet<string> parts = new();
            List<string> ret = new();

            var modelDir = @"\parts";
            var modelExt = @".partsbnd.dcx";
            if (Type == GameType.DarkSoulsPTDE)
            {
                modelExt = ".partsbnd";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                modelDir = @"\model\parts";
                modelExt = ".bnd";
                var partsGatheredFiles = GetAllAssets(modelDir, ["*"], true, true);
                    Directory.GetFiles(RootDirectory + modelDir, "*", SearchOption.AllDirectories);
                foreach (var f in partsGatheredFiles)
                {
                    if (!f.EndsWith("common.commonbnd.dcx") && !f.EndsWith("common_cloth.commonbnd.dcx") &&
                        !f.EndsWith("facepreset.bnd"))
                    {
                        ret.Add(GetFileNameWithoutExtensions(f));
                    }
                }

                return ret;
            }

            List<string> partsFiles = GetAllAssets(modelDir, [$@"*{modelExt}"]).ToList();
            foreach (var f in partsFiles)
            {
                var name = GetFileNameWithoutExtensions(f);
                ret.Add(name);
                parts.Add(name);
            }

            return ret;
        }
        catch (DirectoryNotFoundException e)
        {
            // Game likely isn't UXM unpacked
            return new List<string>();
        }
    }

    public AssetDescription GetPartsModel(string part)
    {
        AssetDescription ret = new();
        ret.AssetName = part;
        ret.AssetArchiveVirtualPath = $@"parts/{part}/model";
        if (Type == GameType.DarkSoulsIISOTFS)
        {
            ret.AssetVirtualPath = $@"parts/{part}/model/{part}.flv";
        }
        else if (Type is GameType.DarkSoulsPTDE)
        {
            ret.AssetVirtualPath = $@"parts/{part}/model/{part.ToUpper()}.flver";
        }
        else
        {
            ret.AssetVirtualPath = $@"parts/{part}/model/{part}.flver";
        }

        return ret;
    }

    public AssetDescription GetPartTextures(string partsId)
    {
        AssetDescription ad = new();
        ad.AssetArchiveVirtualPath = null;
        ad.AssetPath = null;
        if (Type == GameType.ArmoredCoreVI)
        {
            string path;
            if (partsId.Substring(0, 2) == "wp")
            {
                string id;
                if (partsId.EndsWith("_l"))
                {
                    id = partsId[..^2].Split("_").Last();
                    path = GetAssetPath($@"parts\wp_{id}_l.tpf.dcx");
                }
                else
                {
                    id = partsId.Split("_").Last();
                    path = GetAssetPath($@"parts\wp_{id}.tpf.dcx");
                }
            }
            else
            {
                path = GetAssetPath($@"parts\{partsId}_u.tpf.dcx");
            }

            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"parts/{partsId}/tex";
            }
        }
        else if (Type == GameType.EldenRing)
        {
            // Maybe add an option down the line to load lower quality
            var path = GetAssetPath($@"parts\{partsId}.partsbnd.dcx");
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"parts/{partsId}/tex";
            }
        }
        else if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
        {
            var path = GetAssetPath($@"parts\{partsId}.partsbnd.dcx");
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"parts/{partsId}/tex";
            }
        }
        else if (Type == GameType.Bloodborne)
        {
            var path = GetAssetPath($@"parts\{partsId}.partsbnd.dcx");
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetVirtualPath = $@"parts/{partsId}/tex";
            }
        }
        else if (Type == GameType.DarkSoulsPTDE)
        {
            var path = GetAssetPath($@"parts\{partsId}.partsbnd");
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"parts/{partsId}/tex";
            }
        }
        else if (Type == GameType.DemonsSouls)
        {
            var path = GetAssetPath($@"parts\{partsId}.partsbnd.dcx");
            if (path != null)
            {
                ad.AssetPath = path;
                ad.AssetArchiveVirtualPath = $@"parts/{partsId}/tex";
            }
        }

        return ad;
    }

    public AssetDescription GetNullAsset()
    {
        AssetDescription ret = new();
        ret.AssetPath = "null";
        ret.AssetName = "null";
        ret.AssetArchiveVirtualPath = "null";
        ret.AssetVirtualPath = "null";
        return ret;
    }

    /// <summary>
    ///     Converts a virtual path to an actual filesystem path. Only resolves virtual paths up to the bnd level,
    ///     which the remaining string is output for additional handling
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    public string VirtualToRealPath(string virtualPath, out string bndpath)
    {
        var pathElements = virtualPath.Split('/');
        Regex mapRegex = new(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
        var ret = "";

        // Parse the virtual path with a DFA and convert it to a game path
        var i = 0;
        if (pathElements[i].Equals("map"))
        {
            i++;
            if (pathElements[i].Equals("tex"))
            {
                i++;
                if (Type == GameType.DarkSoulsIISOTFS)
                {
                    var mid = pathElements[i];
                    i++;
                    var id = pathElements[i];
                    if (id == "tex")
                    {
                        bndpath = "";
                        return GetAssetPath($@"model\map\t{mid.Substring(1)}.tpfbhd");
                    }
                }
                else if (Type == GameType.DemonsSouls)
                {
                    var mid = pathElements[i];
                    i++;
                    bndpath = "";
                    return GetAssetPath($@"map\{mid}\{mid}_{pathElements[i]}.tpf.dcx");
                }
                else
                {
                    var mid = pathElements[i];
                    i++;
                    bndpath = "";
                    if (pathElements[i] == "env")
                    {
                        if (Type == GameType.DarkSoulsRemastered)
                        {
                            return GetAssetPath($@"map\{mid}\GI_EnvM_{mid}.tpf.dcx");
                        }

                        return GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx");
                    }

                    return GetAssetPath($@"map\{mid}\{mid}_{pathElements[i]}.tpfbhd");
                }
            }
            else if (mapRegex.IsMatch(pathElements[i]))
            {
                var mapid = pathElements[i];
                i++;
                if (pathElements[i].Equals("model"))
                {
                    i++;
                    bndpath = "";
                    if (Type == GameType.DarkSoulsPTDE)
                    {
                        return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver");
                    }

                    if (Type == GameType.DarkSoulsRemastered)
                    {
                        return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver.dcx");
                    }

                    if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        return GetAssetPath($@"model\map\{mapid}.mapbhd");
                    }

                    if (Type == GameType.Bloodborne || Type == GameType.DemonsSouls)
                    {
                        return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver.dcx");
                    }

                    if (Type == GameType.EldenRing)
                    {
                        return GetAssetPath($@"map\{mapid.Substring(0, 3)}\{mapid}\{pathElements[i]}.mapbnd.dcx");
                    }

                    if (Type == GameType.ArmoredCoreVI)
                    {
                        return GetAssetPath($@"map\{mapid.Substring(0, 3)}\{mapid}\{pathElements[i]}.mapbnd.dcx");
                    }

                    return GetAssetPath($@"map\{mapid}\{pathElements[i]}.mapbnd.dcx");
                }

                if (pathElements[i].Equals("hit"))
                {
                    i++;
                    var hittype = pathElements[i];
                    i++;
                    if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls)
                    {
                        bndpath = "";
                        return GetAssetPath($@"map\{mapid}\{pathElements[i]}");
                    }

                    if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        bndpath = "";
                        return GetAssetPath($@"model\map\h{mapid.Substring(1)}.hkxbhd");
                    }

                    if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
                    {
                        bndpath = "";
                        if (hittype == "lo")
                        {
                            return GetAssetPath($@"map\{mapid}\l{mapid.Substring(1)}.hkxbhd");
                        }

                        return GetAssetPath($@"map\{mapid}\h{mapid.Substring(1)}.hkxbhd");
                    }

                    bndpath = "";
                    return null;
                }

                if (pathElements[i].Equals("nav"))
                {
                    i++;
                    if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls ||
                        Type == GameType.DarkSoulsRemastered)
                    {
                        if (i < pathElements.Length)
                        {
                            bndpath = $@"{pathElements[i]}";
                        }
                        else
                        {
                            bndpath = "";
                        }

                        if (Type == GameType.DarkSoulsRemastered)
                        {
                            return GetAssetPath($@"map\{mapid}\{mapid}.nvmbnd.dcx");
                        }

                        return GetAssetPath($@"map\{mapid}\{mapid}.nvmbnd");
                    }

                    if (Type == GameType.DarkSoulsIII)
                    {
                        bndpath = "";
                        return GetAssetPath($@"map\{mapid}\{mapid}.nvmhktbnd.dcx");
                    }

                    bndpath = "";
                    return null;
                }
            }
        }
        else if (pathElements[i].Equals("chr"))
        {
            i++;
            var chrid = pathElements[i];
            i++;
            if (pathElements[i].Equals("model"))
            {
                bndpath = "";
                if (Type == GameType.DarkSoulsPTDE)
                {
                    return GetAssetPath($@"chr\{chrid}.chrbnd");
                }

                if (Type == GameType.DarkSoulsIISOTFS)
                {
                    return GetAssetPath($@"model\chr\{chrid}.bnd");
                }

                if (Type == GameType.DemonsSouls)
                {
                    return GetAssetPath($@"chr\{chrid}\{chrid}.chrbnd.dcx");
                }

                return GetAssetPath($@"chr\{chrid}.chrbnd.dcx");
            }

            if (pathElements[i].Equals("tex"))
            {
                bndpath = "";
                return GetChrTexturePath(chrid);
            }
        }
        else if (pathElements[i].Equals("obj"))
        {
            i++;
            var objid = pathElements[i];
            i++;
            if (pathElements[i].Equals("model") || pathElements[i].Equals("tex"))
            {
                bndpath = "";
                if (Type == GameType.DarkSoulsPTDE)
                {
                    return GetAssetPath($@"obj\{objid}.objbnd");
                }

                if (Type == GameType.DarkSoulsIISOTFS)
                {
                    return GetAssetPath($@"model\obj\{objid}.bnd");
                }

                if (Type == GameType.EldenRing)
                {
                    // Derive subfolder path from model name (all vanilla AEG are within subfolders)
                    if (objid.Length >= 6)
                    {
                        return GetAssetPath($@"asset\aeg\{objid.Substring(0, 6)}\{objid}.geombnd.dcx");
                    }

                    return null;
                }

                if (Type == GameType.ArmoredCoreVI)
                {
                    if (objid.Length >= 6)
                    {
                        return GetAssetPath($@"asset\environment\geometry\{objid}.geombnd.dcx");
                    }

                    return null;
                }

                return GetAssetPath($@"obj\{objid}.objbnd.dcx");
            }
        }
        else if (pathElements[i].Equals("parts"))
        {
            i++;
            var partsId = pathElements[i];
            i++;
            if (pathElements[i].Equals("model") || pathElements[i].Equals("tex"))
            {
                bndpath = "";
                if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered)
                {
                    return GetAssetPath($@"parts\{partsId}.partsbnd");
                }

                if (Type == GameType.DarkSoulsIISOTFS)
                {
                    var partType = "";
                    switch (partsId.Substring(0, 2))
                    {
                        case "as":
                            partType = "accessories";
                            break;
                        case "am":
                            partType = "arm";
                            break;
                        case "bd":
                            partType = "body";
                            break;
                        case "fa":
                        case "fc":
                        case "fg":
                            partType = "face";
                            break;
                        case "hd":
                            partType = "head";
                            break;
                        case "leg":
                            partType = "leg";
                            break;
                        case "sd":
                            partType = "shield";
                            break;
                        case "wp":
                            partType = "weapon";
                            break;
                    }

                    return GetAssetPath($@"model\parts\{partType}\{partsId}.bnd");
                }

                if (Type == GameType.EldenRing)
                {
                    return GetAssetPath($@"parts\{partsId}\{partsId}.partsbnd.dcx");
                }

                if (Type == GameType.ArmoredCoreVI && pathElements[i].Equals("tex"))
                {
                    string path;
                    if (partsId.Substring(0, 2) == "wp")
                    {
                        string id;
                        if (partsId.EndsWith("_l"))
                        {
                            id = partsId[..^2].Split("_").Last();
                            path = GetAssetPath($@"parts\wp_{id}_l.tpf.dcx");
                        }
                        else
                        {
                            id = partsId.Split("_").Last();
                            path = GetAssetPath($@"parts\wp_{id}.tpf.dcx");
                        }
                    }
                    else
                    {
                        path = GetAssetPath($@"parts\{partsId}_u.tpf.dcx");
                    }

                    return path;
                }

                return GetAssetPath($@"parts\{partsId}.partsbnd.dcx");
            }
        }

        bndpath = virtualPath;
        return null;
    }
}
