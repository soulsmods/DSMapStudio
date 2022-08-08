using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Business logic for cross-map connections in a game.
    /// </summary>
    internal class SpecialMapConnections
    {
        public static Transform? GetEldenMapTransform(
            string mapid,
            IReadOnlyDictionary<string, ObjectContainer> loadedMaps)
        {
            if (!TryInitializeEldenOffsets())
            {
                return null;
            }
            if (!TryParseMap(mapid, out byte[] target) || !ToEldenGlobalCoords(target, Vector3.Zero, 0, 0, out Vector3 targetGlobal))
            {
                return null;
            }
            (int originX, int originZ) = GetClosestTile(targetGlobal, 0, 0);
            // Recenter target in terms of closest tile center, for maximum precision
            if (!ToEldenGlobalCoords(target, Vector3.Zero, originX, originZ, out targetGlobal))
            {
                return null;
            }

            float closestDistSq = float.PositiveInfinity;
            Vector3 closestOriginGlobal = Vector3.Zero;
            ObjectContainer closestMap = null;
            foreach (KeyValuePair<string, ObjectContainer> entry in loadedMaps)
            {
                if (entry.Value == null
                    || !entry.Value.RootObject.HasTransform
                    || !TryParseMap(entry.Key, out byte[] origin)
                    || !ToEldenGlobalCoords(origin, Vector3.Zero, originX, originZ, out Vector3 originGlobal))
                {
                    continue;
                }
                float distSq = Vector3.DistanceSquared(targetGlobal, originGlobal);
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    closestOriginGlobal = originGlobal;
                    closestMap = entry.Value;
                }
            }
            if (closestMap == null)
            {
                return null;
            }

            Vector3 targetOffset = targetGlobal - closestOriginGlobal;
            return closestMap.RootObject.GetLocalTransform() + targetOffset;
        }

        private class DungeonOffset
        {
            public int TileX { get; set; }
            public int TileZ { get; set; }
            public Vector3 TileOffset { get; set; }
        }

        private static Dictionary<string, DungeonOffset> eldenRingOffsets;
        private static bool TryInitializeEldenOffsets()
        {
            if (eldenRingOffsets != null)
            {
                return eldenRingOffsets.Count > 0;
            }
            IReadOnlyDictionary<string, PARAM> loadedParams = ParamEditor.ParamBank.Params;
            // Do not explicitly check ParamBank's game type here, but fail gracefully if the param does not exist
            if (loadedParams == null || !loadedParams.TryGetValue("WorldMapLegacyConvParam", out PARAM convParam))
            {
                return false;
            }

            // Now, attempt to populate the offset dictionary. This relies on param field names matching official Paramdex names.
            List<string> srcPartFields = new List<string> { "srcAreaNo", "srcGridXNo", "srcGridZNo" };
            List<string> dstPartFields = new List<string> { "dstAreaNo", "dstGridXNo", "dstGridZNo" };
            // Some maps have multiple incompatible connections with no clear distinguishing characteristics,
            // so these are the authoritative connections, based on manual testing.
            Dictionary<string, string> correctConnects = new Dictionary<string, string>
            {
                // Farum Azula from Bestial Sanctum - not Forge of the Giants (m60_54_53_00) oddly enough
                ["m13_00_00_00"] = "m60_51_43_00",
                // Haligtree from Ordina
                ["m15_00_00_00"] = "m60_48_57_00",
            };
            Dictionary<string, DungeonOffset> dungeonOffsets = new Dictionary<string, DungeonOffset>();
            foreach (PARAM.Row row in convParam.Rows)
            {
                // Dungeon -> World conversions
                // Calculating source (legacy) in terms of destination (overworld)
                if ((byte)row["isBasePoint"].Value == 0)
                {
                    continue;
                }
                byte[] dstParts = GetRowMapParts(row, dstPartFields).ToArray();
                if (dstParts[0] != 60)
                {
                    continue;
                }
                string srcId = FormatMap(GetRowMapParts(row, srcPartFields));
                string dstId = FormatMap(dstParts);
                if (dungeonOffsets.ContainsKey(srcId)
                    || (correctConnects.TryGetValue(srcId, out string trueConnect) && dstId != trueConnect))
                {
                    continue;
                }
                Vector3 srcPos = GetRowPosition(row, "srcPos");
                Vector3 dstPos = GetRowPosition(row, "dstPos");
                dungeonOffsets[srcId] = new DungeonOffset
                {
                    TileX = dstParts[1],
                    TileZ = dstParts[2],
                    TileOffset = dstPos - srcPos,
                };
            }
            foreach (PARAM.Row row in convParam.Rows)
            {
                // Dungeon -> Dungeon
                // Calculating destination (legacy) in terms of source (already legacy)
                // Only one iteration of this appears to be needed.
                byte[] dstParts = GetRowMapParts(row, dstPartFields).ToArray();
                if (dstParts[0] == 60)
                {
                    continue;
                }
                string srcId = FormatMap(GetRowMapParts(row, srcPartFields));
                string dstId = FormatMap(dstParts);
                if (!dungeonOffsets.ContainsKey(dstId) && dungeonOffsets.TryGetValue(srcId, out DungeonOffset val))
                {
                    Vector3 srcPos = GetRowPosition(row, "srcPos");
                    Vector3 dstPos = GetRowPosition(row, "dstPos");
                    dungeonOffsets[dstId] = new DungeonOffset
                    {
                        TileX = val.TileX,
                        TileZ = val.TileZ,
                        TileOffset = val.TileOffset + srcPos - dstPos,
                    };
                }
            }
            // Custom case, as this map is only ever loaded from m11_05/m19_00, so assumes the same origin.
            dungeonOffsets["m11_71_00_00"] = dungeonOffsets["m11_05_00_00"];
            // m60_00_00_99's origin matches m60_10_08_02
            dungeonOffsets["m60_00_00_00"] = new DungeonOffset
            {
                TileX = 40,
                TileZ = 32,
                TileOffset = new Vector3(384, 0, 384),
            };

            eldenRingOffsets = dungeonOffsets;
            return true;
        }

        private static bool ToEldenGlobalCoords(IList<byte> mapId, Vector3 local, int originTileX, int originTileZ, out Vector3 global)
        {
            int tileX, tileZ;
            if (mapId[3] == 99)
            {
                // Treat skybox maps same as their originals
                mapId = mapId.ToArray();
                mapId[3] = 0;
            }
            if (mapId[0] == 60 && mapId[1] > 0 && mapId[2] > 0)
            {
                int scale = mapId[3] % 10;
                int scaleFactor = 1;
                if (scale == 1)
                {
                    scaleFactor = 2;
                    local += new Vector3(128, 0, 128);
                }
                else if (scale == 2)
                {
                    scaleFactor = 4;
                    local += new Vector3(384, 0, 384);
                }
                tileX = mapId[1] * scaleFactor;
                tileZ = mapId[2] * scaleFactor;
            }
            else
            {
                string mapIdStr = FormatMap(mapId);
                if (!eldenRingOffsets.TryGetValue(mapIdStr, out DungeonOffset offset))
                {
                    global = default;
                    return false;
                }
                local += offset.TileOffset;
                tileX = offset.TileX;
                tileZ = offset.TileZ;
            }
            global = local + new Vector3((tileX - originTileX) * 256, 0, (tileZ - originTileZ) * 256);
            return true;
        }

        private static (int, int) GetClosestTile(Vector3 global, int originTileX, int originTileZ)
        {
            return ((int)Math.Round(global.X / 256) + originTileX, (int)Math.Round(global.Z / 256) + originTileZ);
        }

        private static bool TryParseMap(string map, out byte[] parts)
        {
            try
            {
                parts = map.TrimStart('m').Split('_').Select(p => byte.Parse(p)).ToArray();
                if (parts.Length == 4)
                {
                    return true;
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
            }
            parts = null;
            return false;
        }

        private static string FormatMap(IEnumerable<byte> bytes) =>
            "m" + string.Join("_", bytes.Select(b => b == 0xFF ? "XX" : $"{b:d2}"));

        private static List<byte> GetRowMapParts(PARAM.Row row, List<string> fields)
        {
            List<byte> bytes = fields.Select(f => (byte)row[f].Value).ToList();
            while (bytes.Count < 4)
            {
                bytes.Add(0);
            }
            return bytes;
        }

        private static Vector3 GetRowPosition(PARAM.Row row, string type)
        {
            return new Vector3((float)row[$"{type}X"].Value, (float)row[$"{type}Y"].Value, (float)row[$"{type}Z"].Value);
        }
    }
}
