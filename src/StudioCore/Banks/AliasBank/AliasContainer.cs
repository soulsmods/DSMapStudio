using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;

namespace StudioCore.Banks.AliasBank;

public class AliasContainer
{
    private Dictionary<string, AliasResource> aliasMap = new Dictionary<string, AliasResource>();

    private AliasType aliasType;

    private string gametype;

    private string gameModDirectory;

    private string ProgramDirectory = ".dsms";

    public AliasContainer()
    {
        aliasMap = null;
        aliasType = AliasType.None;
    }
    public AliasContainer(AliasType _aliasType, string _gametype, string _gameModDirectory)
    {
        aliasType = _aliasType;
        gametype = _gametype;
        gameModDirectory = _gameModDirectory;

        if (aliasType is AliasType.Model)
        {
            aliasMap.Add("Characters", LoadJSON("Chr"));
            aliasMap.Add("Objects", LoadJSON("Obj"));
            aliasMap.Add("Parts", LoadJSON("Part"));
            aliasMap.Add("MapPieces", LoadJSON("MapPiece"));
        }

        if (aliasType is AliasType.Map)
        {
            aliasMap.Add("Maps", LoadJSON("Maps"));
        }
    }
    private AliasResource LoadJSON(string filename)
    {
        var baseResource = new AliasResource();
        var modResource = new AliasResource();

        if (aliasType is AliasType.None)
            return null;

        var baseResourcePath = AppContext.BaseDirectory + $"\\Assets\\Aliases\\{GetAliasTypeDir()}\\{gametype}\\{filename}.json";

        if (File.Exists(baseResourcePath))
        {
            using (var stream = File.OpenRead(baseResourcePath))
            {
                baseResource = JsonSerializer.Deserialize(stream, AliasResourceSerializationContext.Default.AliasResource);   
            }
        }

        var modResourcePath = gameModDirectory + $"\\{ProgramDirectory}\\Assets\\Aliases\\{GetAliasTypeDir()}\\{gametype}\\{filename}.json";

        // If path does not exist, use baseResource only
        if (File.Exists(modResourcePath))
        {
            using (var stream = File.OpenRead(modResourcePath))
            {
                modResource = JsonSerializer.Deserialize(stream, AliasResourceSerializationContext.Default.AliasResource);
            }

            // Replace baseResource entries with those from modResource if there are ID matches
            foreach (var bEntry in baseResource.list)
            {
                var baseId = bEntry.id;
                var baseName = bEntry.name;
                var baseTags = bEntry.tags;

                TaskLogs.AddLog($"baseId {baseId}");
                TaskLogs.AddLog($"baseName {baseName}");

                foreach (var mEntry in modResource.list)
                {
                    var modId = mEntry.id;
                    var modName = mEntry.name;
                    var modTags = mEntry.tags;

                    TaskLogs.AddLog($"modId {modId}");
                    TaskLogs.AddLog($"modName {modName}");

                    // Mod override exists
                    if (baseId == modId)
                    {
                        TaskLogs.AddLog($"Override with {modId}");
                        TaskLogs.AddLog($"Override with {modName}");

                        bEntry.id = modId;
                        bEntry.name = modName;
                        bEntry.tags = modTags;
                    }
                }
            }

            // Add mod local unique rentries
            foreach (var mEntry in modResource.list)
            {
                var modId = mEntry.id;

                var isUnique = true;

                foreach (var bEntry in baseResource.list)
                {
                    var baseId = bEntry.id;

                    // Mod override exists
                    if (baseId == modId)
                        isUnique = false;
                }

                if (isUnique)
                    baseResource.list.Add(mEntry);
            }
        }

        return baseResource;
    }

    private string GetAliasTypeDir()
    {
        var typDir = "";

        if (aliasType is AliasType.Model)
        {
            typDir = "Models";
        }

        if (aliasType is AliasType.Map)
        {
            typDir = "Maps";
        }

        return typDir;
    }

    public List<AliasReference> GetEntries(string name)
    {
        return aliasMap[name].list;
    }
}
