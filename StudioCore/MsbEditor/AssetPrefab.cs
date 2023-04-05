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
        public GameType GameType = GameType.EldenRing;

        /// <summary>
        /// List of AssetInfo derived from AssetContainer.
        /// </summary>
        [JsonIgnore]
        public List<AssetInfo> Assets = new();

        /// <summary>
        /// MSB bytes that stores asset bytes.
        /// </summary>
        public byte[] AssetContainerBytes { get; set; }

        // JsonExtensionData stores fields json that are not present in class in order to retain data between versions.
        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        [JsonIgnore]
        public List<MSBE.Part.Asset> MSBE_Assets
        {
            get
            {
                if (GameType is GameType.EldenRing)
                {
                    List<MSBE.Part.Asset> objs = new();
                    foreach (var asset in Assets)
                    {
                        if (asset.MSBE_Asset != null)
                        {
                            objs.Add(asset.MSBE_Asset);
                        }
                    }
                    return objs;
                }
                throw new NotSupportedException();
            }
        }

        public AssetPrefab()
        { }

        public class AssetInfo
        {
            // JsonExtensionData stores fields json that are not present in class in order to retain data between versions.
            [JsonExtensionData]
            private IDictionary<string, JToken> _additionalData;

            public MSBE.Part.Asset MSBE_Asset;
            //public int ID;

            public AssetInfo(MSBE.Part.Asset asset)
            {
                MSBE_Asset = (MSBE.Part.Asset)asset.DeepCopy();
                MSBE_Asset_ClearIndexReferences(MSBE_Asset);
            }

            public void MSBE_Asset_ClearIndexReferences(MSBE.Part.Asset asset)
            {
                Array.Clear(asset.UnkPartNames);
            }

            public void AddNamePrefixToAsset(string prefix)
            {
                var prop = MSBE_Asset.GetType().GetProperty("Name");
                if (prop == null)
                {
                    throw new InvalidDataException($"AssetPrefab operation failed, {MSBE_Asset.GetType()} does not contain Name property.");
                }
                var name = prop.GetValue(MSBE_Asset);
                name = $"{prefix}_{name}";
                prop.SetValue(MSBE_Asset, name);
            }
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
                foreach (var asset in Assets)
                {
                    map.Parts.Assets.Add(asset.MSBE_Asset);

                    // Needs a model in place to write MSBE freely.
                    MSBE.Model.Asset model = new();
                    model.Name = asset.MSBE_Asset.ModelName;
                    map.Models.Assets.Add(model);
                }
                AssetContainerBytes = map.Write();
                
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json);
                /*
                System.Windows.Forms.MessageBox.Show(
                    "Saved successfully.",
                    "Asset prefab export",
                    System.Windows.Forms.MessageBoxButtons.OK);
                */
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Unable to export AssetPrefab due to the following error:\n\n{e.Message}\n{e.StackTrace}",
                    "AssetPrefab export error",
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
                    AssetInfo info = new(asset);
                    prefab.Assets.Add(asset);
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
