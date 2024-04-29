﻿using SoulsFormats;
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
    public static Project ActiveProject { get; set; }
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
    public GameType Type => Locator.ActiveProject.Type;

    /// <summary>
    ///     The game interroot where all the game assets are
    /// </summary>
    public string GameRootDirectory => Locator.ActiveProject.ParentProject.RootDirectory;

    /// <summary>
    ///     An optional override mod directory where modded files are stored
    /// </summary>
    public string GameModDirectory => Locator.ActiveProject.RootDirectory;

    public string GetAssetPath(string relpath) => Locator.ActiveProject.GetAssetPath(relpath);

    public void SetFromProjectSettings(ProjectSettings settings, string moddir)
    {
        Locator.ActiveProject = new Project(settings, moddir);
    }

    public bool CreateRecoveryProject() => Locator.ActiveProject.CreateRecoveryProject() != null;

    /// <summary>
    ///     Gets the full list of maps in the game (excluding chalice dungeons). Basically if there's an msb for it,
    ///     it will be in this list.
    /// </summary>
    /// <returns></returns>
    public List<string> GetFullMapList() => Locator.ActiveProject.GetFullMapList();

    public AssetDescription GetMapMSB(string mapid, bool writemode = false) => Locator.ActiveProject.GetMapMSB(mapid, writemode);

    public List<AssetDescription> GetMapBTLs(string mapid, bool writemode = false) => Locator.ActiveProject.GetMapBTLs(mapid, writemode);

    public AssetDescription GetMapNVA(string mapid, bool writemode = false) => Locator.ActiveProject.GetMapNVA(mapid, writemode);

    /// <summary>
    ///     Get folders with msgbnds used in-game
    /// </summary>
    /// <returns>Dictionary with language name and path</returns>
    public Dictionary<string, string> GetMsgLanguages() => Locator.ActiveProject.GetMsgLanguages();

    /// <summary>
    ///     Get path of item.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetItemMsgbnd(string langFolder, bool writemode = false) => Locator.ActiveProject.GetItemMsgbnd(langFolder, writemode);

    /// <summary>
    ///     Get path of menu.msgbnd (english by default)
    /// </summary>
    public AssetDescription GetMenuMsgbnd(string langFolder, bool writemode = false) => Locator.ActiveProject.GetMenuMsgbnd(langFolder, writemode);

    public AssetDescription GetMsgbnd(string msgBndType, string langFolder, bool writemode = false) => Locator.ActiveProject.GetMsgbnd(msgBndType, langFolder, writemode);

    public string GetGameIDForDir() => AssetUtils.GetGameIDForDir(Locator.ActiveProject.Type);

    public string GetScriptAssetsCommonDir() => Locator.ActiveProject.GetScriptAssetsCommonDir();

    public string GetScriptAssetsDir() => Locator.ActiveProject.GetScriptAssetsDir();

    public string GetUpgraderAssetsDir() => Locator.ActiveProject.GetUpgraderAssetsDir();

    public string GetGameOffsetsAssetsDir() => Locator.ActiveProject.GetGameOffsetsAssetsDir();

    public string GetParamdefDir() => Locator.ActiveProject.GetParamdefDir();

    public string GetTentativeParamTypePath() => Locator.ActiveProject.GetTentativeParamTypePath();

    public string GetParammetaDir() => Locator.ActiveProject.GetParammetaDir();

    public string GetParamNamesDir() => Locator.ActiveProject.GetParamNamesDir();

    public string GetStrippedRowNamesPath(string paramName) => Locator.ActiveProject.GetStrippedRowNamesPath(paramName);

    public PARAMDEF GetParamdefForParam(string paramType) => Locator.ActiveProject.GetParamdefForParam(paramType);

    public AssetDescription GetDS2GeneratorParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2GeneratorParam(mapid, writemode);

    public AssetDescription GetDS2GeneratorLocationParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2GeneratorLocationParam(mapid, writemode);

    public AssetDescription GetDS2GeneratorRegistParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2GeneratorRegistParam(mapid, writemode);

    public AssetDescription GetDS2EventParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2EventParam(mapid, writemode);

    public AssetDescription GetDS2EventLocationParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2EventLocationParam(mapid, writemode);

    public AssetDescription GetDS2ObjInstanceParam(string mapid, bool writemode = false) => Locator.ActiveProject.GetDS2ObjInstanceParam(mapid, writemode);

    public List<AssetDescription> GetMapModels(string mapid) => Locator.ActiveProject.GetMapModels(mapid);

    public string MapModelNameToAssetName(string mapid, string modelname) => Locator.ActiveProject.MapModelNameToAssetName(mapid, modelname);

    /// <summary>
    ///     Gets the adjusted map ID that contains all the map assets
    /// </summary>
    /// <param name="mapid">The msb map ID to adjust</param>
    /// <returns>The map ID for the purpose of asset storage</returns>
    public string GetAssetMapID(string mapid) => Locator.ActiveProject.GetAssetMapID(mapid);

    public AssetDescription GetMapModel(string mapid, string model) => Locator.ActiveProject.GetMapModel(mapid, model);

    public AssetDescription GetMapCollisionModel(string mapid, string model, bool hi = true) => Locator.ActiveProject.GetMapCollisionModel(mapid, model, hi);

    public List<AssetDescription> GetMapTextures(string mapid) => Locator.ActiveProject.GetMapTextures(mapid);

    public List<string> GetEnvMapTextureNames(string mapid) => Locator.ActiveProject.GetEnvMapTextureNames(mapid);

    public AssetDescription GetChrTextures(string chrid) => Locator.ActiveProject.GetChrTextures(chrid);

    public AssetDescription GetMapNVMModel(string mapid, string model) => Locator.ActiveProject.GetMapNVMModel(mapid, model);

    public AssetDescription GetHavokNavmeshes(string mapid) => Locator.ActiveProject.GetHavokNavmeshes(mapid);

    public AssetDescription GetHavokNavmeshModel(string mapid, string model) => Locator.ActiveProject.GetHavokNavmeshModel(mapid, model);

    public List<string> GetChrModels() => Locator.ActiveProject.GetChrModels();

    public AssetDescription GetChrModel(string chr) => Locator.ActiveProject.GetChrModel(chr);

    public List<string> GetObjModels() => Locator.ActiveProject.GetObjModels();

    public AssetDescription GetObjModel(string obj) => Locator.ActiveProject.GetObjModel(obj);

    public AssetDescription GetObjTexture(string obj) => Locator.ActiveProject.GetObjTexture(obj);

    public AssetDescription GetAetTexture(string aetid) => Locator.ActiveProject.GetAetTexture(aetid);

    public List<string> GetPartsModels() => Locator.ActiveProject.GetPartsModels();

    public AssetDescription GetPartsModel(string part) => Locator.ActiveProject.GetPartsModel(part);

    public AssetDescription GetPartTextures(string partsId) => Locator.ActiveProject.GetPartTextures(partsId);

    /// <summary>
    ///     Converts a virtual path to an actual filesystem path. Only resolves virtual paths up to the bnd level,
    ///     which the remaining string is output for additional handling
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    public string VirtualToRealPath(string virtualPath, out string bndpath) => Locator.ActiveProject.VirtualToRealPath(virtualPath, out bndpath);
}
