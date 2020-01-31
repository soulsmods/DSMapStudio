using System;
using System.Collections.Generic;
using System.Text;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Navigation context for a map. Stores all the meta navigation information for a DS1
    /// map (i.e. mcg and mcp stuff).
    /// </summary>
    public class MapNavigationDS1
    {
        public Map Map { get; private set; } = null;

        public List<NavRegion> Regions { get; private set; } = null;

        public MapNavigationDS1(Map map, MCP mcp, MCG mcg)
        {
            Map = map;

            foreach (var r in mcp.Rooms)
            {
                Regions.Add(new NavRegion(Map, r));
            }
        }
    }
}
