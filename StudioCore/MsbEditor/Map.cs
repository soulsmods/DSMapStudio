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

        // This keeps all models that exist when loading a map, so that saves
        // can be byte perfect
        private HashSet<string> LoadedModels = new HashSet<string>();

        public Map(string mapid)
        {
            MapId = mapid;
            var t = new TransformNode(mapid);
            RootObject = new MapObject(t, MapObject.ObjectType.TypeMapRoot);
        }

        public void LoadMSB(IMsb msb)
        {
            foreach (var m in msb.Models.GetEntries())
            {
                LoadedModels.Add(m.Name);
            }

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

            foreach (var p in msb.Events.GetEntries())
            {
                var n = new MapObject(p, MapObject.ObjectType.TypeEvent);
                MapObjects.Add(n);
                RootObject.AddChild(n);
            }
        }

        private void AddModelDS1(IMsb m, MSB1.ModelType typ, string name)
        {
            var model = new MSB1.Model();
            model.Name = name;
            model.Type = typ;
            if (typ == MSB1.ModelType.MapPiece)
            {
                model.Placeholder = $@"N:\FRPG\data\Model\map\{MapId}\sib\{name}.sib";
            }
            else if (typ == MSB1.ModelType.Object)
            {
                model.Placeholder = $@"N:\FRPG\data\Model\obj\{name}\sib\{name}.sib";
            }
            else if (typ == MSB1.ModelType.Enemy)
            {
                model.Placeholder = $@"N:\FRPG\data\Model\chr\{name}\sib\{name}.sib";
            }
            else if (typ == MSB1.ModelType.Collision)
            {
                model.Placeholder = $@"N:\FRPG\data\Model\map\{MapId}\hkxwin\{name}.hkxwin";
            }
            else if (typ == MSB1.ModelType.Navmesh)
            {
                model.Placeholder = $@"N:\FRPG\data\Model\map\{MapId}\navimesh\{name}.sib";
            }
            m.Models.Add(model);
        }

        private void AddModel<T>(IMsb m, string name) where T : IMsbModel, new()
        {
            var model = new T();
            model.Name = name;
            m.Models.Add(model);
        }

        public void SerializeToMSB(IMsb msb)
        {
            foreach (var m in MapObjects)
            {
                if (m.MsbObject != null && m.MsbObject is IMsbPart p)
                {
                    msb.Parts.Add(p);
                    LoadedModels.Add(p.ModelName);
                }
                else if (m.MsbObject != null && m.MsbObject is IMsbRegion r)
                {
                    msb.Regions.Add(r);
                }
                else if (m.MsbObject != null && m.MsbObject is IMsbEvent e)
                {
                    msb.Events.Add(e);
                }
            }

            foreach (var m in LoadedModels.OrderBy(q => q))
            {
                if (m.StartsWith("m"))
                {
                    AddModelDS1(msb, MSB1.ModelType.MapPiece, m);
                }
                if (m.StartsWith("h"))
                {
                    AddModelDS1(msb, MSB1.ModelType.Collision, m);
                }
                if (m.StartsWith("o"))
                {
                    AddModelDS1(msb, MSB1.ModelType.Object, m);
                }
                if (m.StartsWith("c"))
                {
                    AddModelDS1(msb, MSB1.ModelType.Enemy, m);
                }
                if (m.StartsWith("n"))
                {
                    AddModelDS1(msb, MSB1.ModelType.Navmesh, m);
                }
            }
        }

        public void Clear()
        {
            MapObjects.Clear();
        }
    }
}
