using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// High level class that stores a single map (msb) and can serialize/
    /// deserialize it. This is the logical portion of the map and does not
    /// handle tasks like rendering or loading associated assets with it.
    /// </summary>
    public class Map
    {
        public List<MapObject> MapObjects = new List<MapObject>();

        public Map()
        {

        }

        public void LoadMSB(IMsb msb)
        {
            foreach (var p in msb.Parts.GetEntries())
            {
                MapObjects.Add(new MapObject(p, MapObject.ObjectType.TypePart));
            }

            foreach (var p in msb.Regions.GetEntries())
            {
                MapObjects.Add(new MapObject(p, MapObject.ObjectType.TypeRegion));
            }

            /*foreach (var p in msb.Events.GetEntries())
            {
                MapObjects.Add(new MapObject(p, MapObject.ObjectType.TypeEvent));
            }*/
        }

        public void Clear()
        {
            MapObjects.Clear();
        }
    }
}
