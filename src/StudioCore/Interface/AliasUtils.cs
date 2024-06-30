using Andre.Formats;
using Octokit;
using StudioCore.Banks;
using StudioCore.Banks.AliasBank;
using StudioCore.Editors.AssetBrowser;
using StudioCore.MsbEditor;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Andre.Native.ImGuiBindings;

namespace StudioCore.Interface;
public static class AliasUtils
{
    public static void DisplayAlias(string aliasName)
    {
        if (aliasName != "")
        {
            ImGui.SameLine();
            ImGui.PushTextWrapPos(0);
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), @$"{aliasName}");
            ImGui.PopTextWrapPos();
        }
    }
    public static void DisplayTagAlias(string aliasName)
    {
        if (aliasName != "")
        {
            ImGui.SameLine();
            ImGui.PushTextWrapPos(0);
            ImGui.TextColored(new Vector4(0.0f, 1.0f, 0.0f, 1.0f), @$"[{aliasName}]");
            ImGui.PopTextWrapPos();
        }
    }

    public static string GetTagListString(List<string> refTagList)
    {
        var tagListStr = "";

        if (refTagList.Count > 0)
        {
            var tagStr = refTagList[0];
            foreach (var entry in refTagList.Skip(1))
            {
                tagStr = $"{tagStr},{entry}";
            }
            tagListStr = tagStr;
        }
        else
        {
            tagListStr = "";
        }

        return tagListStr;
    }

    public static string GetAliasFromCache(string name, List<AliasReference> referenceList)
    {
        foreach (var alias in referenceList)
        {
            if (name == alias.id)
            {
                return alias.name;
            }
        }

        return "";
    }

    public static List<String> GetTagList(string tags)
    {
        var list = new List<string>();

        if (tags.Contains(","))
        {
            list = tags.Split(',').ToList();
        }
        else
        {
            list.Add(tags);
        }

        return list;
    }

    // Asset Browser
    public static List<AliasReference> GetAliasReferenceList(AssetCategoryType category)
    {
        switch (category)
        {
            case AssetCategoryType.Character:
                return ModelAliasBank.Bank.AliasNames.GetEntries("Characters");
            case AssetCategoryType.Asset:
                return ModelAliasBank.Bank.AliasNames.GetEntries("Objects");
            case AssetCategoryType.Part:
                return ModelAliasBank.Bank.AliasNames.GetEntries("Parts");
            case AssetCategoryType.MapPiece:
                return ModelAliasBank.Bank.AliasNames.GetEntries("MapPieces");
        }

        return null;
    }

    public static string GetAssetBrowserAliasName(AssetCategoryType category, string rawName)
    {
        var aliasName = rawName;

        return aliasName;
    }

    public static string GetMapNameAlias(string mapId)
    {
        if (MapAliasBank.Bank.MapNames == null)
            return "";

        if (MapAliasBank.Bank.MapNames.ContainsKey(mapId))
        {
            return MapAliasBank.Bank.MapNames[mapId];
        }

        return "";
    }

    // Map Editor
    public static string GetEntityAliasName(Entity e)
    {
        var aliasName = "";
        var modelName = "";

        // Early returns if the show X vars are disabled
        if (!CFG.Current.MapEditor_MapObjectList_ShowCharacterNames && (e.IsPartEnemy() || e.IsPartDummyEnemy()))
            return aliasName;

        if (!CFG.Current.MapEditor_MapObjectList_ShowAssetNames && (e.IsPartAsset() || e.IsPartDummyAsset()))
            return aliasName;

        if (!CFG.Current.MapEditor_MapObjectList_ShowMapPieceNames && e.IsPartMapPiece())
            return aliasName;

        if (!CFG.Current.MapEditor_MapObjectList_ShowTreasureNames && e.IsEventTreasure())
            return aliasName;

        if (e.IsPart())
        {
            modelName = e.GetPropertyValue<string>("ModelName");
            if (modelName == null)
            {
                return "";
            }

            modelName = modelName.ToLower();
        }

        // Only grab the alias once, then refer to the cachedName within the entity
        if (e.CachedAliasName == null)
        {
            if (CFG.Current.MapEditor_MapObjectList_ShowCharacterNames && (e.IsPartEnemy() || e.IsPartDummyEnemy()))
            {
                aliasName = GetAliasFromCache(modelName, ModelAliasBank.Bank.AliasNames.GetEntries("Characters"));
                aliasName = $"{aliasName}";
            }

            if (CFG.Current.MapEditor_MapObjectList_ShowAssetNames && (e.IsPartAsset() || e.IsPartDummyAsset()))
            {
                aliasName = GetAliasFromCache(modelName, ModelAliasBank.Bank.AliasNames.GetEntries("Objects"));
                aliasName = $"{aliasName}";
            }

            if (CFG.Current.MapEditor_MapObjectList_ShowMapPieceNames && e.IsPartMapPiece())
            {
                aliasName = GetAliasFromCache(modelName, ModelAliasBank.Bank.AliasNames.GetEntries("MapPieces"));
                aliasName = $"{aliasName}";
            }

            // Player/System Characters: peek in param/fmg for name
            if (CFG.Current.MapEditor_MapObjectList_ShowCharacterNames && (e.IsPartEnemy() || e.IsPartDummyEnemy()))
            {
                if (modelName == "c0000")
                {
                    aliasName = FindPlayerCharacterName(e, modelName);
                }

                if (modelName == "c0100" || modelName == "c0110" || modelName == "c0120" || modelName == "c1000")
                {
                    aliasName = FindSystemCharacterName(e, modelName);
                }
            }

            // Treasure: show itemlot row name
            if (CFG.Current.MapEditor_MapObjectList_ShowTreasureNames && e.IsEventTreasure())
            {
                aliasName = FindTreasureName(e);
            }

            e.CachedAliasName = aliasName;
        }
        else
        {
            aliasName = e.CachedAliasName;
        }

        return aliasName;
    }

    public static string FindPlayerCharacterName(Entity e, string modelName)
    {
        var aliasName = "";

        int npcId = e.GetPropertyValue<int>("NPCParamID");
        try
        {
            var param = ParamBank.PrimaryBank.GetParamFromName("NpcParam");
            if (param != null)
            {
                Param.Row row = param[npcId];

                if (row != null)
                {
                    bool nameSucces = false;

                    // Try Name ID first
                    Param.Cell? cq = row["nameId"];
                    if (cq != null)
                    {
                        Param.Cell c = cq.Value;
                        var term = c.Value.ToParamEditorString();
                        var result = term;

                        if (Locator.ActiveProject.FMGBank.IsLoaded)
                        {
                            var matchingFmgInfo = Locator.ActiveProject.FMGBank.FmgInfoBank.First(x => x.Name.Contains("Character"));

                            if (matchingFmgInfo != null)
                            {
                                foreach (var entry in matchingFmgInfo.Fmg.Entries)
                                {
                                    if (entry.ID == int.Parse(term))
                                    {
                                        result = entry.Text;
                                        nameSucces = true;
                                        break;
                                    }
                                }
                            }
                        }

                        aliasName = $"{result}";
                    }

                    // Try Row Name instead if Name ID is not used
                    if (!nameSucces)
                    {
                        aliasName = $"{row.Name}";
                    }
                }
            }
        }
        catch { }

        return aliasName;
    }

    public static string FindSystemCharacterName(Entity e, string modelName)
    {
        var aliasName = "";

        int npcId = e.GetPropertyValue<int>("NPCParamID");
        try
        {
            var param = ParamBank.PrimaryBank.GetParamFromName("NpcParam");
            if (param != null)
            {
                Param.Row row = param[npcId];

                aliasName = $"{row.Name}";
            }
        }
        catch { }

        return aliasName;
    }

    public static string FindTreasureName(Entity e)
    {
        var aliasName = "";

        int itemlotId = e.GetPropertyValue<int>("ItemLotID");

        try
        {
            var paramName = "ItemLotParam";

            if (Locator.AssetLocator.Type == GameType.EldenRing)
            {
                paramName = "ItemLotParam_map";
            }

            var param = ParamBank.PrimaryBank.GetParamFromName(paramName);
            if (param != null)
            {
                Param.Row row = param[itemlotId];

                if (row != null)
                {
                    aliasName = $"{row.Name}";
                }
            }
        }
        catch { }

        return aliasName;
    }
}
