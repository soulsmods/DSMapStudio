﻿using Octokit;
using StudioCore.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using DotNext.Text;

namespace StudioCore.Banks.AliasBank;

/// <summary>
/// An alias bank holds naming information, allowing for user-readable notes to be appended to raw identifiers (e.g. c0000 becomes c0000 <Player>)
/// An alias bank has 2 sources: DSMS, and the local project. 
/// Entries in the local project version will supercede the DSMS entries.
/// </summary>
public class AliasBank
{
    public AliasContainer _loadedAliasBank { get; set; }

    public bool IsLoadingAliases { get; set; }
    public bool mayReloadAliasBank { get; set; }

    private string TemplateName = "Template.json";

    private string ProgramDirectory = ".dsms";

    private string AliasDirectory = "";

    private string FileName = "";

    private bool IsAssetFileType = false;

    private string AliasName = "";

    private AliasType aliasType;

    public Dictionary<string, string> MapNames;

    public AliasBank(AliasType _aliasType)
    {
        mayReloadAliasBank = false;

        aliasType = _aliasType;

        if (aliasType is AliasType.Model)
        {
            AliasName = "Models";
            AliasDirectory = "Models";
            FileName = "";
            IsAssetFileType = true;
        }

        if (aliasType is AliasType.Map)
        {
            AliasName = "Maps";
            AliasDirectory = "Maps";
            FileName = "Maps";
            IsAssetFileType = false;
        }
    }

    public AliasContainer AliasNames
    {
        get
        {
            if (IsLoadingAliases)
                return null;

            return _loadedAliasBank;
        }
    }

    public void ReloadAliasBank()
    {
        TaskManager.Run(new TaskManager.LiveTask($"Alias Bank - Load {AliasName}", TaskManager.RequeueType.None, false,
        () =>
        {
            _loadedAliasBank = new AliasContainer();
            IsLoadingAliases = true;

            if (Locator.AssetLocator.Type != GameType.Undefined)
            {
                try
                {
                    _loadedAliasBank = new AliasContainer(aliasType, Locator.AssetLocator.GetGameIDForDir(), Locator.AssetLocator.GameModDirectory);
                }
                catch (Exception e)
                {
                    TaskLogs.AddLog($"FAILED LOAD: {e.Message}");
                }

                IsLoadingAliases = false;
            }
            else
            {
                IsLoadingAliases = false;
            }

            UpdateMapNames();
        }));
    }

    public AliasResource LoadTargetAliasBank(string path)
    {
        var newResource = new AliasResource();

        if (File.Exists(path))
        {
            using (var stream = File.OpenRead(path))
            {
                newResource = JsonSerializer.Deserialize(stream, AliasResourceSerializationContext.Default.AliasResource);
            }
        }

        return newResource;
    }

    public void WriteTargetAliasBank(AliasResource targetBank, string assetType)
    {
        var resourcePath = Locator.AssetLocator.GameModDirectory + $"\\{ProgramDirectory}\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";

        if (CFG.Current.AliasBank_EditorMode)
        {
            resourcePath = AppContext.BaseDirectory + $"\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";
        }

        var resourceFilePath = $"{resourcePath}\\{FileName}.json";

        if (IsAssetFileType)
        {
            resourceFilePath = $"{resourcePath}\\{assetType}.json";
        }

        if (File.Exists(resourceFilePath))
        {
            string jsonString = JsonSerializer.Serialize(targetBank, typeof(AliasResource), AliasResourceSerializationContext.Default);
            
            try
            {
                var fs = new FileStream(resourceFilePath, System.IO.FileMode.Create);
                var data = Encoding.ASCII.GetBytes(jsonString);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                TaskLogs.AddLog($"{ex}");
            }
        }
    }

    public void AddToLocalAliasBank(string assetType, string refID, string refName, string refTags)
    {
        var templateResource = AppContext.BaseDirectory + $"\\Assets\\Aliases\\{TemplateName}";

        var resourcePath = Locator.AssetLocator.GameModDirectory + $"\\{ProgramDirectory}\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";

        if (CFG.Current.AliasBank_EditorMode)
        {
            resourcePath = AppContext.BaseDirectory + $"\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";
        }

        var resourceFilePath = $"{resourcePath}\\{FileName}.json";

        if (IsAssetFileType)
        {
            resourceFilePath = $"{resourcePath}\\{assetType}.json";
        }

        // Create directory/file if they don't exist
        if (!Directory.Exists(resourcePath))
        {
            Directory.CreateDirectory(resourcePath);
        }

        if (!File.Exists(resourceFilePath))
        {
            File.Copy(templateResource, resourceFilePath);
        }

        if (File.Exists(resourceFilePath))
        {
            // Load up the target local model alias bank.
            var targetResource = LoadTargetAliasBank(resourceFilePath);

            var doesExist = false;

            // If it exists within the mod local file, update the contents
            foreach (var entry in targetResource.list)
            {
                if (entry.id == refID)
                {
                    doesExist = true;

                    entry.name = refName;

                    if (refTags.Contains(","))
                    {
                        var newTags = new List<string>();
                        var tagList = refTags.Split(",");
                        foreach (var tag in tagList)
                            newTags.Add(tag);
                        entry.tags = newTags;
                    }
                    else
                    {
                        entry.tags = new List<string> { refTags };
                    }
                }
            }

            // If it doesn't exist in the mod local file, add it in
            if (!doesExist)
            {
                var entry = new AliasReference();
                entry.id = refID;
                entry.name = refName;
                entry.tags = new List<string>();

                if (refTags.Contains(","))
                {
                    var newTags = new List<string>();
                    var tagList = refTags.Split(",");
                    foreach (var tag in tagList)
                        newTags.Add(tag);
                    entry.tags = newTags;
                }
                else
                    entry.tags.Add(refTags);

                targetResource.list.Add(entry);
            }

            WriteTargetAliasBank(targetResource, assetType);
        }
    }

    /// <summary>
    /// Removes specified reference from local model alias bank.
    /// </summary>
    public void RemoveFromLocalAliasBank(string assetType, string refID)
    {
        var resourcePath = Locator.AssetLocator.GameModDirectory + $"\\{ProgramDirectory}\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";

        if (CFG.Current.AliasBank_EditorMode)
        {
            resourcePath = AppContext.BaseDirectory + $"\\Assets\\Aliases\\{AliasDirectory}\\{Locator.AssetLocator.GetGameIDForDir()}\\";
        }

        var resourceFilePath = $"{resourcePath}\\{FileName}.json";

        if (IsAssetFileType)
        {
            resourceFilePath = $"{resourcePath}\\{assetType}.json";
        }

        if (File.Exists(resourceFilePath))
        {
            // Load up the target local model alias bank. 
            var targetResource = LoadTargetAliasBank(resourceFilePath);

            // Remove the specified reference from the local model alias bank.
            for (var i = 0; i <= targetResource.list.Count - 1; i++)
            {
                var entry = targetResource.list[i];
                if (entry.id == refID)
                {
                    targetResource.list.Remove(entry);
                    break;
                }
            }

            WriteTargetAliasBank(targetResource, assetType);
        }
    }

    public void UpdateMapNames()
    {
        if (aliasType is AliasType.Map)
        {
            var _mapNames = new Dictionary<string, string>();

            foreach (var entry in AliasNames.GetEntries("Maps"))
            {
                if (!CFG.Current.MapAliases_ShowUnusedNames)
                {
                    if (entry.tags[0] != "unused")
                    {
                        _mapNames.Add(entry.id, entry.name);
                    }
                    else
                    {
                        _mapNames.Add(entry.id, entry.name);
                    }
                }
            }

            MapNames = _mapNames;
        }
    }
}
