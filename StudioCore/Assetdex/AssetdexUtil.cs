using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    public class AssetdexUtil
    {
        public static Dictionary<string, AssetReference> assetReferenceDict_Chr = new Dictionary<string, AssetReference>();
        public static Dictionary<string, AssetReference> assetReferenceDict_Obj = new Dictionary<string, AssetReference>();
        public static Dictionary<string, AssetReference> assetReferenceDict_Part = new Dictionary<string, AssetReference>();
        public static Dictionary<string, AssetReference> assetReferenceDict_MapPiece = new Dictionary<string, AssetReference>();

        /// <summary>
        /// Refresh the asset reference dictionaries for editor usage.
        /// </summary>
        public static void UpdateAssetReferences(GameReference game)
        {
            assetReferenceDict_Chr.Clear();
            assetReferenceDict_Obj.Clear();
            assetReferenceDict_Part.Clear();
            assetReferenceDict_MapPiece.Clear();

            foreach (AssetReference entry in game.chrList)
            {
                if (!assetReferenceDict_Chr.ContainsKey(entry.id))
                    assetReferenceDict_Chr.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.objList)
            {
                if (!assetReferenceDict_Obj.ContainsKey(entry.id))
                    assetReferenceDict_Obj.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.partList)
            {
                if (!assetReferenceDict_Part.ContainsKey(entry.id))
                    assetReferenceDict_Part.Add(entry.id, entry);
            }
            foreach (AssetReference entry in game.mapPieceList)
            {
                if (!assetReferenceDict_MapPiece.ContainsKey(entry.id))
                    assetReferenceDict_MapPiece.Add(entry.id, entry);
            }
        }
    }
}
