using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;
using System.Xml.Serialization;
using SoulsFormats;
using StudioCore.Scene;
using System.Diagnostics;
using FSParam;
using StudioCore.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace StudioCore.MsbEditor
{

    public class AssetPrefab
    {
        public string PrefabName = "";
        public string Separator = "[]";
        public GameType GameType = GameType.EldenRing;

        /// <summary>
        /// Bytes of the MSB that stores prefab data.
        /// </summary>
        public byte[] AssetContainerBytes { get; set; }

        /// <summary>
        /// List of AssetInfo derived from MSB AssetContainerBytes.
        /// </summary>
        [JsonIgnore]
        public List<AssetInfo> AssetInfoChildren = new();

        /// <summary>
        /// List of Map Entities derived from AssetInfoChildren.
        /// </summary>
        [JsonIgnore]
        public List<MapEntity> MapEntityChildren = new();


        // JsonExtensionData stores fields json that are not present in class in order to retain data between versions.
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        public AssetPrefab()
        { }

        public AssetPrefab(HashSet<MapEntity> entities)
        {
            foreach (var ent in entities)
            {
                if (ent.WrappedObject is MSBE.Part.Asset asset)
                {
                    AssetInfoChildren.Add(new AssetInfo(this, asset));
                }
                else if (ent.WrappedObject is MSBE.Region.Other region)
                {
                    AssetInfoChildren.Add(new AssetInfo(this, region));
                }
            }
        }

        /*
        // In progress. System to store meta information in the name of the msb entity.
        public class AssetPrefabTags
        {
            public const string TagStart = "[";
            public const string TagEnd = "]";
            public const string TagDelimiter = "|";
            public const string ValueDelimiter = "&";
            public const string IndexReferenceTag = "IREF";
        }
        */

        public class AssetInfo
        {
            public AssetPrefab Parent;

            public AssetInfoDataType DataType = AssetInfoDataType.None;
            public enum AssetInfoDataType
            { 
                None = -1,
                Part = 1000,
                Region = 2000,
            }

            public object InnerObject = null;

            public AssetInfo(AssetPrefab parent, MSBE.Part.Asset asset)
            {
                InnerObject = asset.DeepCopy();
                DataType = AssetInfoDataType.Part;
                Parent = parent;
                ClearIndexReferences();
            }
            public AssetInfo(AssetPrefab parent, MSBE.Region.Other region)
            {
                InnerObject = region.DeepCopy();
                DataType = AssetInfoDataType.Region;
                Parent = parent;
                ClearIndexReferences();
            }

            public void ClearIndexReferences()
            {
                if (InnerObject is MSBE.Part.Asset asset)
                {
                    Array.Clear(asset.UnkPartNames);
                }
            }

            public void AddNamePrefix(string prefix)
            {
                var prop = InnerObject.GetType().GetProperty("Name");
                if (prop == null)
                {
                    throw new InvalidDataException($"AssetPrefab operation failed, {InnerObject.GetType()} does not contain Name property.");
                }
                var name = prop.GetValue(InnerObject);
                name = $"{prefix}{Parent.Separator}{name}";
                prop.SetValue(InnerObject, name);
            }

            public void StripNamePrefix()
            {
                var prop = InnerObject.GetType().GetProperty("Name");
                if (prop == null)
                {
                    throw new InvalidDataException($"AssetPrefab operation failed, {InnerObject.GetType()} does not contain Name property.");
                }
                string name = (string)prop.GetValue(InnerObject);
                try
                {
                    name = name.Split(Parent.Separator)[1];
                }
                catch
                { }
                prop.SetValue(InnerObject, name);
            }
        }

        public List<MapEntity> GenerateMapEntities(Map targetMap)
        {
            List<MapEntity> ents = new();
            foreach (var assetInfo in AssetInfoChildren)
            {
                // TODO: For prefab scene tree support:
                // * Make a map entity of the prefab
                // * Add that to ents list
                // * Make the asset objects children of that
                // * Modify scenetree to handle AssetPrefabs.

                MapEntity ent = new(targetMap, assetInfo.InnerObject);
                switch (assetInfo.DataType)
                {
                    case AssetInfo.AssetInfoDataType.Part:
                        ent.Type = MapEntity.MapEntityType.Part;
                        break;
                    case AssetInfo.AssetInfoDataType.Region:
                        ent.Type = MapEntity.MapEntityType.Region;
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported AssetInfoDataType {assetInfo.DataType}");
                }
                ents.Add(ent);

                MapEntityChildren.Add(ent);
            }
            return ents;
        }

        /// <summary>
        /// Exports AssetPrefab to json file.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Write(string path)
        {
            try
            {
                MSBE map = new();
                foreach (var assetInfo in AssetInfoChildren)
                {
                    assetInfo.StripNamePrefix();
                    if (assetInfo.InnerObject is MSBE.Part.Asset asset)
                    {
                        map.Parts.Assets.Add(asset);
                        // Needs a model in place to write MSBE freely.
                        MSBE.Model.Asset model = new();
                        model.Name = asset.ModelName;
                        map.Models.Assets.Add(model);
                    }
                    else if (assetInfo.InnerObject is MSBE.Region.Other region)
                    {
                        map.Regions.Others.Add(region);
                    }
                }

                AssetContainerBytes = map.Write();
                
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Unable to export Asset Prefab due to the following error:\n\n{e.Message}\n{e.StackTrace}",
                    "Asset Prefab export error",
                    System.Windows.Forms.MessageBoxButtons.OK);
                return false;
            }
        }

        /// <summary>
        /// Imports AssetPrefab info from json file.
        /// </summary>
        /// <returns>Asset Prefab if successful, null otherwise.</returns>
        public static AssetPrefab? ImportJson(string path)
        {
            try
            {
                var settings = new JsonSerializerSettings();
                AssetPrefab prefab = JsonConvert.DeserializeObject<AssetPrefab>(File.ReadAllText(path), settings);

                MSBE pseudoMap = MSBE.Read(prefab.AssetContainerBytes);
                foreach (var asset in pseudoMap.Parts.Assets)
                {
                    AssetInfo info = new(prefab, asset);
                    info.AddNamePrefix(prefab.PrefabName);
                    prefab.AssetInfoChildren.Add(info);
                }
                foreach (var region in pseudoMap.Regions.Others)
                {
                    AssetInfo info = new(prefab, region);
                    info.AddNamePrefix(prefab.PrefabName);
                    prefab.AssetInfoChildren.Add(info);
                }

                return prefab;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Unable to import AssetPrefab due to the following error:" +
                    $"\n\n{e.Message}"
                    , "Asset prefab import error"
                    , System.Windows.Forms.MessageBoxButtons.OK);
                
                return null;
            }
        }
    }

}
