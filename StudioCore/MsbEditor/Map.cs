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
        public string MapId { get; private set; }
        public List<MapObject> MapObjects = new List<MapObject>();
        public MapObject RootObject { get; private set; }

        public Map(string mapid)
        {
            MapId = mapid;
            var t = new TransformNode(mapid);
            RootObject = new MapObject(t, MapObject.ObjectType.TypeEditor);
        }

        public void LoadMSB(IMsb msb)
        {
            foreach (var p in msb.Parts.GetEntries())
            {
                var n = new MapObject(p, MapObject.ObjectType.TypePart);
                MapObjects.Add(n);
                RootObject.AddChild(n);
            }

            foreach (var p in msb.Regions.GetEntries())
            {
                var n = new MapObject(p, MapObject.ObjectType.TypeRegion);
                MapObjects.Add(n);
                RootObject.AddChild(n);
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
