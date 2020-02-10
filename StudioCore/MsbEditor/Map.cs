using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public Universe Universe { get; private set; }

        /// <summary>
        /// The map offset used to transform light and ds2 generators
        /// </summary>
        public Transform MapOffset { get; set; } = Transform.Default;

        // This keeps all models that exist when loading a map, so that saves
        // can be byte perfect
        private Dictionary<string, IMsbModel> LoadedModels = new Dictionary<string, IMsbModel>();

        public Map(Universe u, string mapid)
        {
            MapId = mapid;
            Universe = u;
            var t = new TransformNode(mapid);
            RootObject = new MapObject(this, t, MapObject.ObjectType.TypeMapRoot);
        }

        public void LoadMSB(IMsb msb)
        {
            foreach (var m in msb.Models.GetEntries())
            {
                LoadedModels.Add(m.Name, m);
            }

            foreach (var p in msb.Parts.GetEntries())
            {
                var n = new MapObject(this, p, MapObject.ObjectType.TypePart);
                MapObjects.Add(n);
                RootObject.AddChild(n);
            }

            foreach (var p in msb.Regions.GetEntries())
            {
                var n = new MapObject(this, p, MapObject.ObjectType.TypeRegion);
                MapObjects.Add(n);
                RootObject.AddChild(n);
            }

            foreach (var p in msb.Events.GetEntries())
            {
                var n = new MapObject(this, p, MapObject.ObjectType.TypeEvent);
                MapObjects.Add(n);
                RootObject.AddChild(n);
            }
        }

        public void AddObject(MapObject obj)
        {
            MapObjects.Add(obj);
            RootObject.AddChild(obj);
        }

        private void AddModelDS1(IMsb m, MSB1.ModelType typ, string name)
        {
            if (LoadedModels[name] != null)
            {
                m.Models.Add(LoadedModels[name]);
                return;
            }
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

        private void AddModelDS2(IMsb m, MSB2.ModelType typ, string name)
        {
            if (LoadedModels[name] != null)
            {
                m.Models.Add(LoadedModels[name]);
                return;
            }
            var model = new MSB2.Model();
            model.Name = name;
            model.Type = typ;
            m.Models.Add(model);
        }

        private void AddModelBB(IMsb m, MSBB.Model model, string name)
        {
            if (LoadedModels[name] != null)
            {
                m.Models.Add(LoadedModels[name]);
                return;
            }
            var a = $@"A{MapId.Substring(1, 2)}";
            model.Name = name;
            if (model is MSBB.Model.MapPiece)
            {
                model.Placeholder = $@"N:\SPRJ\data\Model\map\{MapId}\sib\{name}{a}.sib";
            }
            else if (model is MSBB.Model.Object)
            {
                model.Placeholder = $@"N:\SPRJ\data\Model\obj\{name.Substring(0, 3)}\{name}\sib\{name}.sib";
            }
            else if (model is MSBB.Model.Enemy)
            {
                // Not techincally required but doing so means that unedited bloodborne maps
                // will write identical to the original byte for byte
                if (name == "c0000")
                {
                    model.Placeholder = $@"N:\SPRJ\data\Model\chr\{name}\sib\{name}.SIB";
                }
                else
                {
                    model.Placeholder = $@"N:\SPRJ\data\Model\chr\{name}\sib\{name}.sib";
                }
            }
            else if (model is MSBB.Model.Collision)
            {
                model.Placeholder = $@"N:\SPRJ\data\Model\map\{MapId}\hkt\{name}{a}.hkt";
            }
            else if (model is MSBB.Model.Navmesh)
            {
                model.Placeholder = $@"N:\SPRJ\data\Model\map\{MapId}\navimesh\{name}{a}.sib";
            }
            else if (model is MSBB.Model.Other)
            {
                model.Placeholder = $@"";
            }
            m.Models.Add(model);
        }

        private void AddModelDS3(IMsb m, MSB3.Model model, string name)
        {
            if (LoadedModels[name] != null)
            {
                m.Models.Add(LoadedModels[name]);
                return;
            }
            model.Name = name;
            if (model is MSB3.Model.MapPiece)
            {
                model.Placeholder = $@"N:\FDP\data\Model\map\{MapId}\sib\{name}.sib";
            }
            else if (model is MSB3.Model.Object)
            {
                model.Placeholder = $@"N:\FDP\data\Model\obj\{name}\sib\{name}.sib";
            }
            else if (model is MSB3.Model.Enemy)
            {
                model.Placeholder = $@"N:\FDP\data\Model\chr\{name}\sib\{name}.sib";
            }
            else if (model is MSB3.Model.Collision)
            {
                model.Placeholder = $@"N:\FDP\data\Model\map\{MapId}\hkt\{name}.hkt";
            }
            else if (model is MSB3.Model.Other)
            {
                model.Placeholder = $@"";
            }
            m.Models.Add(model);
        }

        private void AddModel<T>(IMsb m, string name) where T : IMsbModel, new()
        {
            var model = new T();
            model.Name = name;
            m.Models.Add(model);
        }

        private void AddModelsDS1(IMsb msb)
        {
            foreach (var mk in LoadedModels.OrderBy(q => q.Key))
            {
                var m = mk.Key;
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

        private void AddModelsDS2(IMsb msb)
        {
            foreach (var mk in LoadedModels.OrderBy(q => q.Key))
            {
                var m = mk.Key;
                if (m.StartsWith("m"))
                {
                    AddModelDS2(msb, MSB2.ModelType.MapPiece, m);
                }
                if (m.StartsWith("h"))
                {
                    AddModelDS2(msb, MSB2.ModelType.Collision, m);
                }
                if (m.StartsWith("o"))
                {
                    AddModelDS2(msb, MSB2.ModelType.Object, m);
                }
                if (m.StartsWith("n"))
                {
                    AddModelDS2(msb, MSB2.ModelType.Navmesh, m);
                }
            }
        }

        private void AddModelsBB(IMsb msb)
        {
            foreach (var mk in LoadedModels.OrderBy(q => q.Key))
            {
                var m = mk.Key;
                if (m.StartsWith("m"))
                {
                    AddModelBB(msb, new MSBB.Model.MapPiece(m), m);
                }
                if (m.StartsWith("h"))
                {
                    AddModelBB(msb, new MSBB.Model.Collision(m), m);
                }
                if (m.StartsWith("o"))
                {
                    AddModelBB(msb, new MSBB.Model.Object(m), m);
                }
                if (m.StartsWith("c"))
                {
                    AddModelBB(msb, new MSBB.Model.Enemy(m), m);
                }
                if (m.StartsWith("n"))
                {
                    AddModelBB(msb, new MSBB.Model.Navmesh(m), m);
                }
            }
        }

        private void AddModelsDS3(IMsb msb)
        {
            foreach (var mk in LoadedModels.OrderBy(q => q.Key))
            {
                var m = mk.Key;
                if (m.StartsWith("m"))
                {
                    AddModelDS3(msb, new MSB3.Model.MapPiece(m), m);
                }
                if (m.StartsWith("h"))
                {
                    AddModelDS3(msb, new MSB3.Model.Collision(m), m);
                }
                if (m.StartsWith("o"))
                {
                    AddModelDS3(msb, new MSB3.Model.Object(m), m);
                }
                if (m.StartsWith("c"))
                {
                    AddModelDS3(msb, new MSB3.Model.Enemy(m), m);
                }
            }
        }

        public void SerializeToMSB(IMsb msb, GameType game)
        {
            foreach (var m in MapObjects)
            {
                if (m.MsbObject != null && m.MsbObject is IMsbPart p)
                {
                    msb.Parts.Add(p);
                    if (!LoadedModels.ContainsKey(p.ModelName))
                    {
                        LoadedModels.Add(p.ModelName, null);
                    }
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

            if (game == GameType.DarkSoulsPTDE)
            {
                AddModelsDS1(msb);
            }
            else if (game == GameType.DarkSoulsIISOTFS)
            {
                AddModelsDS2(msb);
            }
            else if (game == GameType.Bloodborne)
            {
                AddModelsBB(msb);
            }
            else if (game == GameType.DarkSoulsIII)
            {
                AddModelsDS3(msb);
            }
        }

        public bool SerializeDS2Generators(PARAM locations, PARAM generators)
        {
            HashSet<long> ids = new HashSet<long>();
            foreach (var m in MapObjects)
            {
                if (m.Type == MapObject.ObjectType.TypeDS2Generator && m.MsbObject is MergedParamRow mp)
                {
                    if (!ids.Contains(mp.ID))
                    {
                        ids.Add(mp.ID);
                    }
                    else
                    {
                        MessageBox.Show($@"{mp.Name} has an ID that's already used. Please change it to something unique and save again.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    var loc = mp.GetRow("generator-loc");
                    if (loc != null)
                    {
                        // Adjust the location to be relative to the mapoffset
                        var newloc = new PARAM.Row(loc);
                        newloc["PositionX"].Value = (float)loc["PositionX"].Value - MapOffset.Position.X;
                        newloc["PositionY"].Value = (float)loc["PositionY"].Value - MapOffset.Position.Y;
                        newloc["PositionZ"].Value = (float)loc["PositionZ"].Value - MapOffset.Position.Z;
                        locations.Rows.Add(newloc);
                    }
                    var gen = mp.GetRow("generator");
                    if (gen != null)
                    {
                        generators.Rows.Add(gen);
                    }
                }
            }
            return true;
        }

        public bool SerializeDS2Regist(PARAM regist)
        {
            HashSet<long> ids = new HashSet<long>();
            foreach (var m in MapObjects)
            {
                if (m.Type == MapObject.ObjectType.TypeDS2GeneratorRegist && m.MsbObject is PARAM.Row mp)
                {
                    if (!ids.Contains(mp.ID))
                    {
                        ids.Add(mp.ID);
                    }
                    else
                    {
                        MessageBox.Show($@"{mp.Name} has an ID that's already used. Please change it to something unique and save again.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    regist.Rows.Add(mp);
                }
            }
            return true;
        }

        public bool SerializeDS2Events(PARAM evs)
        {
            HashSet<long> ids = new HashSet<long>();
            foreach (var m in MapObjects)
            {
                if (m.Type == MapObject.ObjectType.TypeDS2Event && m.MsbObject is PARAM.Row mp)
                {
                    if (!ids.Contains(mp.ID))
                    {
                        ids.Add(mp.ID);
                    }
                    else
                    {
                        MessageBox.Show($@"{mp.Name} has an ID that's already used. Please change it to something unique and save again.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    var newloc = new PARAM.Row(mp);
                    evs.Rows.Add(newloc);
                }
            }
            return true;
        }

        public bool SerializeDS2EventLocations(PARAM locs)
        {
            HashSet<long> ids = new HashSet<long>();
            foreach (var m in MapObjects)
            {
                if (m.Type == MapObject.ObjectType.TypeDS2EventLocation && m.MsbObject is PARAM.Row mp)
                {
                    if (!ids.Contains(mp.ID))
                    {
                        ids.Add(mp.ID);
                    }
                    else
                    {
                        MessageBox.Show($@"{mp.Name} has an ID that's already used. Please change it to something unique and save again.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    // Adjust the location to be relative to the mapoffset
                    var newloc = new PARAM.Row(mp);
                    newloc["PositionX"].Value = (float)mp["PositionX"].Value - MapOffset.Position.X;
                    newloc["PositionY"].Value = (float)mp["PositionY"].Value - MapOffset.Position.Y;
                    newloc["PositionZ"].Value = (float)mp["PositionZ"].Value - MapOffset.Position.Z;
                    locs.Rows.Add(newloc);
                }
            }
            return true;
        }

        public void Clear()
        {
            MapObjects.Clear();
        }

        public byte GetNextUnique(string prop, byte value)
        {
            HashSet<byte> usedvals = new HashSet<byte>();
            foreach (var obj in MapObjects)
            {
                if (obj.GetPropertyValue(prop) != null)
                {
                    byte val = obj.GetPropertyValue<byte>(prop);
                    usedvals.Add(val);
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (!usedvals.Contains((byte)((value + i) % 256)))
                {
                    return (byte)((value + i) % 256);
                }
            }
            return value;
        }
    }
}
