using SoulsFormats;
using StudioCore.Editor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore;

public static class Locator
{
    public static AssetLocator AssetLocator { get; set; }
}

/// <summary>
///     Generic asset description for a generic game asset
/// </summary>
public class AssetDescription
{
    public string AssetArchiveVirtualPath;

    /// <summary>
    ///     Where applicable, the numeric asset ID. Usually applies to chrs, objs, and various map pieces
    /// </summary>
    public int AssetID;

    /// <summary>
    ///     Pretty UI friendly name for an asset. Usually the file name without an extention i.e. c1234
    /// </summary>
    public string AssetName;

    /// <summary>
    ///     Absolute path of where the full asset is located. If this asset exists in a mod override directory,
    ///     then this path points to that instead of the base game asset.
    /// </summary>
    public string AssetPath;

    /// <summary>
    ///     Virtual friendly path for this asset to use with the resource manager
    /// </summary>
    public string AssetVirtualPath;

    public override int GetHashCode()
    {
        if (AssetVirtualPath != null)
        {
            return AssetVirtualPath.GetHashCode();
        }

        if (AssetPath != null)
        {
            return AssetPath.GetHashCode();
        }

        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is AssetDescription ad)
        {
            if (AssetVirtualPath != null)
            {
                return AssetVirtualPath.Equals(ad.AssetVirtualPath);
            }

            if (AssetPath != null)
            {
                return AssetPath.Equals(ad.AssetPath);
            }
        }

        return base.Equals(obj);
    }
}

/// <summary>
///     Exposes an interface to retrieve game assets from the various souls games. Also allows layering
///     of an additional mod directory on top of the game assets.
/// </summary>
public class AssetLocator
{
    public GameType Type => Project.ActiveProject.Type;

    /// <summary>
    ///     The game interroot where all the game assets are
    /// </summary>
    public string GameRootDirectory => Project.ActiveProject.ParentProject.RootDirectory;

    /// <summary>
    ///     An optional override mod directory where modded files are stored
    /// </summary>
    public string GameModDirectory => Project.ActiveProject.RootDirectory;

    public string GetAssetPath(string relpath) => Project.ActiveProject.GetAssetPath(relpath);

    public void SetFromProjectSettings(ProjectSettings settings, string moddir)
    {
        Project.ActiveProject = new Project(settings, moddir);
    }

    public bool CreateRecoveryProject() => Project.ActiveProject.CreateRecoveryProject() != null;

    /// <summary>
    ///     Gets the full list of maps in the game (excluding chalice dungeons). Basically if there's an msb for it,
    ///     it will be in this list.
    /// </summary>
    /// <returns></returns>
    public List<string> GetFullMapList() => Project.ActiveProject.GetFullMapList();

    public AssetDescription GetMapMSB(string mapid, bool writemode = false) => Project.ActiveProject.GetMapMSB(mapid, writemode);

    public List<AssetDescription> GetMapBTLs(string mapid, bool writemode = false) => Project.ActiveProject.GetMapBTLs(mapid, writemode);

    public AssetDescription GetMapNVA(string mapid, bool writemode = false) => Project.ActiveProject.GetMapNVA(mapid, writemode);

    /// <summary>
    ///     Get folders with msgbnds used in-game
    /// </summary>
    /// <returns>Dictionary with language name and path</returns>
    public Dictionary<string, string> GetMsgLanguages() => Project.ActiveProject.GetMsgLanguages();

    /// <summary>
    ///     Get path of item.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetItemMsgbnd(string langFolder, bool writemode = false) => Project.ActiveProject.GetItemMsgbnd(langFolder, writemode);

    /// <summary>
    ///     Get path of menu.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetMenuMsgbnd(string langFolder, bool writemode = false) => Project.ActiveProject.GetMenuMsgbnd(langFolder, writemode);

    public AssetDescription GetMsgbnd(string msgBndType, string langFolder, bool writemode = false) => Project.ActiveProject.GetMsgbnd(msgBndType, langFolder, writemode);

    public string GetGameIDForDir() => AssetUtils.GetGameIDForDir(Project.ActiveProject.Type);

    public string GetScriptAssetsCommonDir() => Project.ActiveProject.GetScriptAssetsCommonDir();

    public string GetScriptAssetsDir() => Project.ActiveProject.GetScriptAssetsDir();

    public string GetUpgraderAssetsDir() => Project.ActiveProject.GetUpgraderAssetsDir();

    public string GetGameOffsetsAssetsDir() => Project.ActiveProject.GetGameOffsetsAssetsDir();

    public string GetParamdefDir() => Project.ActiveProject.GetParamdefDir();

    public string GetTentativeParamTypePath() => Project.ActiveProject.GetTentativeParamTypePath();

    public string GetParammetaDir() => Project.ActiveProject.GetParammetaDir();

    public string GetParamNamesDir() => Project.ActiveProject.GetParamNamesDir();

    public string GetStrippedRowNamesPath(string paramName) => Project.ActiveProject.GetStrippedRowNamesPath(paramName);

    public PARAMDEF GetParamdefForParam(string paramType) => Project.ActiveProject.GetParamdefForParam(paramType);

    public AssetDescription GetDS2GeneratorParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2GeneratorParam(mapid, writemode);

    public AssetDescription GetDS2GeneratorLocationParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2GeneratorLocationParam(mapid, writemode);

    public AssetDescription GetDS2GeneratorRegistParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2GeneratorRegistParam(mapid, writemode);

    public AssetDescription GetDS2EventParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2EventParam(mapid, writemode);

    public AssetDescription GetDS2EventLocationParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2EventLocationParam(mapid, writemode);

    public AssetDescription GetDS2ObjInstanceParam(string mapid, bool writemode = false) => Project.ActiveProject.GetDS2ObjInstanceParam(mapid, writemode);

    public List<AssetDescription> GetMapModels(string mapid) => Project.ActiveProject.GetMapModels(mapid);

    public string MapModelNameToAssetName(string mapid, string modelname) => Project.ActiveProject.MapModelNameToAssetName(mapid, modelname);

    /// <summary>
    ///     Gets the adjusted map ID that contains all the map assets
    /// </summary>
    /// <param name="mapid">The msb map ID to adjust</param>
    /// <returns>The map ID for the purpose of asset storage</returns>
    public string GetAssetMapID(string mapid) => Project.ActiveProject.GetAssetMapID(mapid);

    public AssetDescription GetMapModel(string mapid, string model) => Project.ActiveProject.GetMapModel(mapid, model);

    public AssetDescription GetMapCollisionModel(string mapid, string model, bool hi = true) => Project.ActiveProject.GetMapCollisionModel(mapid, model, hi);

    public List<AssetDescription> GetMapTextures(string mapid) => Project.ActiveProject.GetMapTextures(mapid);

    public List<string> GetEnvMapTextureNames(string mapid) => Project.ActiveProject.GetEnvMapTextureNames(mapid);

    public AssetDescription GetChrTextures(string chrid) => Project.ActiveProject.GetChrTextures(chrid);

    public AssetDescription GetMapNVMModel(string mapid, string model) => Project.ActiveProject.GetMapNVMModel(mapid, model);

    public AssetDescription GetHavokNavmeshes(string mapid) => Project.ActiveProject.GetHavokNavmeshes(mapid);

    public AssetDescription GetHavokNavmeshModel(string mapid, string model) => Project.ActiveProject.GetHavokNavmeshModel(mapid, model);

    public List<string> GetChrModels() => Project.ActiveProject.GetChrModels();

    public AssetDescription GetChrModel(string chr) => Project.ActiveProject.GetChrModel(chr);

    public List<string> GetObjModels() => Project.ActiveProject.GetObjModels();

    public AssetDescription GetObjModel(string obj) => Project.ActiveProject.GetObjModel(obj);

    public AssetDescription GetObjTexture(string obj) => Project.ActiveProject.GetObjTexture(obj);

    public AssetDescription GetAetTexture(string aetid) => Project.ActiveProject.GetAetTexture(aetid);

    public List<string> GetPartsModels() => Project.ActiveProject.GetPartsModels();

    public AssetDescription GetPartsModel(string part) => Project.ActiveProject.GetPartsModel(part);

    public AssetDescription GetPartTextures(string partsId) => Project.ActiveProject.GetPartTextures(partsId);

    /// <summary>
    ///     Converts a virtual path to an actual filesystem path. Only resolves virtual paths up to the bnd level,
    ///     which the remaining string is output for additional handling
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    public string VirtualToRealPath(string virtualPath, out string bndpath) => Project.ActiveProject.VirtualToRealPath(virtualPath, out bndpath);
}
