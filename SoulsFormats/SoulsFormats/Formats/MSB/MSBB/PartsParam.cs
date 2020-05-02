using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBB
    {
        /// <summary>
        /// Instances of various "things" in this MSB.
        /// </summary>
        public class PartsParam : Section<Part>, IMsbParam<IMsbPart>
        {
            internal override string Type => "PARTS_PARAM_ST";

            /// <summary>
            /// Map pieces in the MSB.
            /// </summary>
            public List<Part.MapPiece> MapPieces;

            /// <summary>
            /// Objects in the MSB.
            /// </summary>
            public List<Part.Object> Objects;

            /// <summary>
            /// Enemies in the MSB.
            /// </summary>
            public List<Part.Enemy> Enemies;

            /// <summary>
            /// Players in the MSB.
            /// </summary>
            public List<Part.Player> Players;

            /// <summary>
            /// Collisions in the MSB.
            /// </summary>
            public List<Part.Collision> Collisions;

            /// <summary>
            /// Navimeshes in the MSB.
            /// </summary>
            public List<Part.Navimesh> Navimeshes;

            /// <summary>
            /// Dummy objects in the MSB.
            /// </summary>
            public List<Part.DummyObject> DummyObjects;

            /// <summary>
            /// Dummy enemies in the MSB.
            /// </summary>
            public List<Part.DummyEnemy> DummyEnemies;

            /// <summary>
            /// Connect collisions in the MSB.
            /// </summary>
            public List<Part.ConnectCollision> ConnectCollisions;

            /// <summary>
            /// Unknowns in the MSB.
            /// </summary>
            public List<Part.Unknown> Unknowns;

            /// <summary>
            /// Creates a new PartsSection with no parts.
            /// </summary>
            public PartsParam(int unk1 = 3) : base(unk1)
            {
                MapPieces = new List<Part.MapPiece>();
                Objects = new List<Part.Object>();
                Enemies = new List<Part.Enemy>();
                Players = new List<Part.Player>();
                Collisions = new List<Part.Collision>();
                Navimeshes = new List<Part.Navimesh>();
                DummyObjects = new List<Part.DummyObject>();
                DummyEnemies = new List<Part.DummyEnemy>();
                ConnectCollisions = new List<Part.ConnectCollision>();
                Unknowns = new List<Part.Unknown>();
            }

            /// <summary>
            /// Returns every part in the order they'll be written.
            /// </summary>
            public override List<Part> GetEntries()
            {
                return SFUtil.ConcatAll<Part>(
                    MapPieces, Objects, Enemies, Players, Collisions, Navimeshes, DummyObjects, DummyEnemies, ConnectCollisions, Unknowns);
            }
            IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() => GetEntries();

            internal override Part ReadEntry(BinaryReaderEx br)
            {
                PartsType type = br.GetEnum32<PartsType>(br.Position + 20);

                switch (type)
                {
                    case PartsType.MapPiece:
                        var mapPiece = new Part.MapPiece(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case PartsType.Object:
                        var obj = new Part.Object(br);
                        Objects.Add(obj);
                        return obj;
                        
                    case PartsType.Enemy:
                        var enemy = new Part.Enemy(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case PartsType.Player:
                        var player = new Part.Player(br);
                        Players.Add(player);
                        return player;

                    case PartsType.Collision:
                        var collision = new Part.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    case PartsType.Navmesh:
                        var navimesh = new Part.Navimesh(br);
                        Navimeshes.Add(navimesh);
                        return navimesh;

                    case PartsType.DummyObject:
                        var dummyObj = new Part.DummyObject(br);
                        DummyObjects.Add(dummyObj);
                        return dummyObj;

                    case PartsType.DummyEnemy:
                        var dummyEne = new Part.DummyEnemy(br);
                        DummyEnemies.Add(dummyEne);
                        return dummyEne;

                    case PartsType.ConnectCollision:
                        var connectColl = new Part.ConnectCollision(br);
                        ConnectCollisions.Add(connectColl);
                        return connectColl;

                    case PartsType.Unknown:
                        var unknown = new Part.Unknown(br);
                        Unknowns.Add(unknown);
                        return unknown;

                    default:
                        //return null;
                        throw new NotImplementedException($"Unsupported part type: {type}");
                }
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Part entry)
            {
                entry.Write(bw, id);
            }

            public void Add(IMsbPart item)
            {
                switch (item)
                {
                    case Part.MapPiece m:
                        MapPieces.Add(m);
                        break;
                    case Part.DummyObject m:
                        DummyObjects.Add(m);
                        break;
                    case Part.Object m:
                        Objects.Add(m);
                        break;
                    case Part.DummyEnemy m:
                        DummyEnemies.Add(m);
                        break;
                    case Part.Enemy m:
                        Enemies.Add(m);
                        break;
                    case Part.Player m:
                        Players.Add(m);
                        break;
                    case Part.Collision m:
                        Collisions.Add(m);
                        break;
                    case Part.ConnectCollision m:
                        ConnectCollisions.Add(m);
                        break;
                    case Part.Navimesh m:
                        Navimeshes.Add(m);
                        break;
                    case Part.Unknown m:
                        Unknowns.Add(m);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        internal enum PartsType : uint
        {
            MapPiece = 0x0,
            Object = 0x1,
            Enemy = 0x2,
            Item = 0x3,
            Player = 0x4,
            Collision = 0x5,
            NPCWander = 0x6,
            Protoboss = 0x7,
            Navmesh = 0x8,
            DummyObject = 0x9,
            DummyEnemy = 0xA,
            ConnectCollision = 0xB,
            Unknown = 0xFFFFFFFF,
        }

        /// <summary>
        /// Any instance of some "thing" in a map.
        /// </summary>
        public abstract class Part : Entry, IMsbPart
        {
            internal abstract PartsType Type { get; }

            internal abstract bool HasGparamConfig { get; }
            internal abstract bool HasUnk4 { get; }

            /// <summary>
            /// The name of this part.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// Not sure what this string is for
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// The placeholder model for this part.
            /// </summary>
            public string Placeholder { get; set; }

            /// <summary>
            /// Seems to be a local id for the parts of this model type
            /// </summary>
            public int ModelLocalID { get; set; }

            private int modelIndex;
            /// <summary>
            /// The name of this part's model.
            /// </summary>
            public string ModelName { get; set; }

            /// <summary>
            /// The center of the part.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// The rotation of the part.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// The scale of the part, which only really works right for map pieces.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] DrawGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] DispGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public uint[] BackreadGroups { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int UnkFA4 { get; set; }

            /// <summary>
            /// Used to identify the part in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte OldLightID { get; set; }
            public sbyte OldFogID { get; set; }
            public sbyte OldScatterID { get; set; }
            public sbyte OldLensFlareID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public sbyte OldLanternID { get; set; }
            public sbyte OldLodParamID { get; set; }
            public sbyte UnkB0E { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool OldIsShadowDest { get; set; }

            internal Part() { }

            internal Part(string name, long unkOffset1Delta, long unkOffset2Delta)
            {
                Name = name;
                ModelName = null;
                Position = Vector3.Zero;
                Rotation = Vector3.Zero;
                Scale = Vector3.One;
                DrawGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                DispGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                BackreadGroups = new uint[8] { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                UnkFA4 = 0;
                EntityID = -1;
                OldLightID = 0;
                OldFogID = 0;
                OldScatterID = 0;
                OldLensFlareID = 0;
                OldLanternID = 0;
                OldLodParamID = 0;
                UnkB0E = 0;
                OldIsShadowDest = false;
            }

            internal Part(Part clone)
            {
                Name = clone.Name;
                Description = clone.Description;
                Placeholder = clone.Placeholder;
                ModelLocalID = clone.ModelLocalID;
                ModelName = clone.ModelName;
                Position = clone.Position;
                Rotation = clone.Rotation;
                Scale = clone.Scale;
                DrawGroups = (uint[])clone.DrawGroups.Clone();
                DispGroups = (uint[])clone.DispGroups.Clone();
                BackreadGroups = (uint[])clone.BackreadGroups.Clone();
                UnkFA4 = clone.UnkFA4;
                EntityID = clone.EntityID;
                OldLightID = clone.OldLightID;
                OldFogID = clone.OldFogID;
                OldScatterID = clone.OldScatterID;
                OldLensFlareID = clone.OldLensFlareID;
                OldLanternID = clone.OldLanternID;
                OldLodParamID = clone.OldLodParamID;
                UnkB0E = clone.UnkB0E;
                OldIsShadowDest = clone.OldIsShadowDest;
            }

            internal Part(BinaryReaderEx br)
            {
                long start = br.Position;

                long descOffset = br.ReadInt64();
                long nameOffset = br.ReadInt64();
                ModelLocalID = br.ReadInt32();
                br.AssertUInt32((uint)Type);

                br.ReadInt32(); // ID

                modelIndex = br.ReadInt32();

                long placeholderOffset = br.ReadInt64();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();

                DrawGroups = br.ReadUInt32s(8);
                DispGroups = br.ReadUInt32s(8);
                BackreadGroups = br.ReadUInt32s(8);
                UnkFA4 = br.ReadInt32();

                long baseDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();
                long gparamOffset = br.ReadInt64();
                long unkOffset4 = br.ReadInt64();

                Description = br.GetUTF16(start + descOffset);
                Name = br.GetUTF16(start + nameOffset);
                if (placeholderOffset == 0)
                    Placeholder = "";
                else
                    Placeholder = br.GetUTF16(start + placeholderOffset);

                br.Position = start + baseDataOffset;
                EntityID = br.ReadInt32();

                OldLightID = br.ReadSByte();
                OldFogID = br.ReadSByte();
                OldScatterID = br.ReadSByte();
                OldLensFlareID = br.ReadSByte();

                br.AssertInt32(0);

                OldLanternID = br.ReadSByte();
                OldLodParamID = br.ReadSByte();
                UnkB0E = br.ReadSByte();
                OldIsShadowDest = br.ReadBoolean();

                br.Position = start + typeDataOffset;
                ReadTypeData(br);

                if (HasGparamConfig)
                {
                    br.Position = start + gparamOffset;
                    ReadGparamConfig(br);
                }

                if (HasUnk4)
                {
                    br.Position = start + unkOffset4;
                    ReadUnk4(br);
                }
            }

            internal abstract void ReadTypeData(BinaryReaderEx br);

            internal virtual void ReadGparamConfig(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Gparam config should not be read for parts with no gparam config.");
            }

            internal virtual void ReadUnk4(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 4 should not be read for parts with no unk struct 4.");
            }

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("DescOffset");
                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(ModelLocalID);
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteInt32(modelIndex);
                bw.ReserveInt64("PlaceholderOffset");
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);

                bw.WriteUInt32s(DrawGroups);
                bw.WriteUInt32s(DispGroups);
                bw.WriteUInt32s(BackreadGroups);
                bw.WriteInt32(UnkFA4);

                bw.ReserveInt64("BaseDataOffset");
                bw.ReserveInt64("TypeDataOffset");
                bw.ReserveInt64("GparamOffset");
                bw.ReserveInt64("UnkOffset4");

                var stringBase = bw.Position;
                bw.FillInt64("DescOffset", bw.Position - start);
                bw.WriteUTF16(Description, true);
                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(ReambiguateName(Name), true);
                bw.FillInt64("PlaceholderOffset", bw.Position - start);
                bw.WriteUTF16(Placeholder, true);

                // BB padding rules are truly A team's best work
                if (bw.Position - stringBase <= 0x38)
                {
                    bw.WritePattern(0x3C - (int)(bw.Position - stringBase), 0);
                }
                else
                {
                    bw.Pad(8);
                }

                bw.FillInt64("BaseDataOffset", bw.Position - start);
                bw.WriteInt32(EntityID);

                bw.WriteSByte(OldLightID);
                bw.WriteSByte(OldFogID);
                bw.WriteSByte(OldScatterID);
                bw.WriteSByte(OldLensFlareID);

                bw.WriteInt32(0);

                bw.WriteSByte(OldLanternID);
                bw.WriteSByte(OldLodParamID);
                bw.WriteSByte(UnkB0E);
                bw.WriteBoolean(OldIsShadowDest);

                // Fuck you From
                if (Type != PartsType.MapPiece)
                    bw.Pad(8);
                if (Type == PartsType.Unknown) // Some unused garbage meme
                    bw.FillInt64("TypeDataOffset", 0);
                else
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                WriteTypeData(bw);
                bw.Pad(8);
                if (HasGparamConfig)
                {
                    bw.FillInt64("GparamOffset", bw.Position - start);
                    WriteGparamConfig(bw);
                }
                else
                {
                    bw.FillInt64("GparamOffset", 0);
                }
                if (HasUnk4)
                {
                    bw.FillInt64("UnkOffset4", bw.Position - start);
                    WriteUnk4(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset4", 0);
                }
            }

            internal abstract void WriteTypeData(BinaryWriterEx bw);

            internal virtual void WriteGparamConfig(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Gparam config should not be written for parts with no gparam config.");
            }

            internal virtual void WriteUnk4(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 4 should not be written for parts with no unk struct 4.");
            }

            internal virtual void GetNames(MSBB msb, Entries entries)
            {
                ModelName = GetName(entries.Models, modelIndex);
            }

            internal virtual void GetIndices(MSBB msb, Entries entries)
            {
                modelIndex = GetIndex(entries.Models, ModelName);
            }

            /// <summary>
            /// Returns the type, ID, and name of this part.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }

            /// <summary>
            /// Gparam value IDs for various part types.
            /// </summary>
            public class GparamConfig
            {
                /// <summary>
                /// ID of the value set from LightSet ParamEditor to use.
                /// </summary>
                public int LightSetID { get; set; }

                /// <summary>
                /// ID of the value set from FogParamEditor to use.
                /// </summary>
                public int FogParamID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// ID of the value set from Env Map:Editor to use.
                /// </summary>
                public int EnvMapID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Creates a GparamConfig with default values.
                /// </summary>
                public GparamConfig() { }

                /// <summary>
                /// Clones an existing GparamConfig.
                /// </summary>
                public GparamConfig(GparamConfig clone)
                {
                    LightSetID = clone.LightSetID;
                    FogParamID = clone.FogParamID;
                    Unk08 = clone.Unk08;
                    EnvMapID = clone.EnvMapID;
                    Unk10 = clone.Unk08;
                }

                internal GparamConfig(BinaryReaderEx br)
                {
                    LightSetID = br.ReadInt32();
                    FogParamID = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    EnvMapID = br.ReadInt32();
                    //Unk10 = br.ReadInt32();
                    br.AssertPattern(0x10, 0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(LightSetID);
                    bw.WriteInt32(FogParamID);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(EnvMapID);
                    //bw.WriteInt32(Unk10);
                    bw.WritePattern(0x10, 0);
                }

                /// <summary>
                /// Returns the four gparam values as a string.
                /// </summary>
                public override string ToString()
                {
                    return $"{LightSetID}, {FogParamID}, {Unk08}, {EnvMapID}";
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class UnkStruct4
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk3C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float Unk40 { get; set; }

                /// <summary>
                /// Creates an UnkStruct4 with default values.
                /// </summary>
                public UnkStruct4() { }

                /// <summary>
                /// Clones an existing UnkStruct4.
                /// </summary>
                public UnkStruct4(UnkStruct4 clone)
                {
                    Unk00 = clone.Unk00;
                    Unk04 = clone.Unk04;
                    Unk08 = clone.Unk08;
                    Unk0C = clone.Unk0C;
                    Unk10 = clone.Unk10;
                    Unk14 = clone.Unk14;
                    Unk3C = clone.Unk3C;
                    Unk40 = clone.Unk40;
                }

                internal UnkStruct4(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    br.AssertPattern(0x24, 0);
                    Unk3C = br.ReadInt32();
                    Unk40 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WritePattern(0x24, 0);
                    bw.WriteInt32(Unk3C);
                    bw.WriteSingle(Unk40);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A static model making up the map.
            /// </summary>
            public class MapPiece : Part
            {
                internal override PartsType Type => PartsType.MapPiece;

                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this map piece.
                /// </summary>
                public GparamConfig Gparam { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT08 { get; set; }

                public MapPiece() { }

                /// <summary>
                /// Creates a new MapPiece with the given ID and name.
                /// </summary>
                public MapPiece(string name) : base(name, 8, 0)
                {
                    Gparam = new GparamConfig();
                    UnkT08 = 0;
                }

                /// <summary>
                /// Creates a new MapPiece with values copied from another.
                /// </summary>
                public MapPiece(MapPiece clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    UnkT08 = clone.UnkT08;
                }

                internal MapPiece(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    //UnkT08 = br.ReadInt32();
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    //bw.WriteInt32(UnkT08);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
            }

            /// <summary>
            /// Any dynamic object such as elevators, crates, ladders, etc.
            /// </summary>
            public class Object : Part
            {
                internal override PartsType Type => PartsType.Object;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this object.
                /// </summary>
                public GparamConfig Gparam { get; private set; }

                private int collisionPartIndex;
                /// <summary>
                /// Unknown.
                /// </summary>
                public string CollisionName { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT04 { get; set; }
                public int UnkT06 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT02a { get; set; }
                public short UnkT02b { get; set; }
                public short UnkT03a { get; set; }
                public short UnkT03b { get; set; }
                public short UnkT05a { get; set; }
                public short UnkT05b { get; set; }

                public Object() { }

                /// <summary>
                /// Creates a new Object with the given ID and name.
                /// </summary>
                public Object(string name) : base(name, 32, 0)
                {
                    CollisionName = null;
                    UnkT02a = 0;
                    UnkT02b = 0;
                    UnkT03a = 0;
                    UnkT03b = 0;
                    UnkT04 = 0;
                    UnkT05a = 0;
                    UnkT05b = 0;
                    UnkT06 = 0;
                }

                /// <summary>
                /// Creates a new Object with values copied from another.
                /// </summary>
                public Object(Object clone) : base(clone)
                {
                    CollisionName = clone.CollisionName;
                    UnkT02a = clone.UnkT02a;
                    UnkT02b = clone.UnkT02b;
                    UnkT03a = clone.UnkT03a;
                    UnkT03b = clone.UnkT03b;
                    UnkT04 = clone.UnkT04;
                    UnkT05a = clone.UnkT05a;
                    UnkT05b = clone.UnkT05b;
                    UnkT06 = clone.UnkT06;
                }

                internal Object(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    collisionPartIndex = br.ReadInt32();
                    UnkT02a = br.ReadInt16();
                    UnkT02b = br.ReadInt16();
                    UnkT03a = br.ReadInt16();
                    UnkT03b = br.ReadInt16();
                    UnkT04 = br.ReadInt32();
                    UnkT05a = br.ReadInt16();
                    UnkT05b = br.ReadInt16();
                    UnkT06 = br.ReadInt32();
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(collisionPartIndex);
                    bw.WriteInt16(UnkT02a);
                    bw.WriteInt16(UnkT02b);
                    bw.WriteInt16(UnkT03a);
                    bw.WriteInt16(UnkT03b);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt16(UnkT05a);
                    bw.WriteInt16(UnkT05b);
                    bw.WriteInt32(UnkT06);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(entries.Parts, collisionPartIndex);
                    //CollisionName = "";
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionPartIndex = GetIndex(entries.Parts, CollisionName);
                }
            }

            /// <summary>
            /// Any non-player character, not necessarily hostile.
            /// </summary>
            public class Enemy : Part
            {
                internal override PartsType Type => PartsType.Enemy;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => false;

                /// <summary>
                /// Gparam IDs for this enemy.
                /// </summary>
                public GparamConfig Gparam { get; private set; }

                private int collisionPartIndex;
                /// <summary>
                /// Unknown.
                /// </summary>
                public string CollisionName { get; set; }

                /// <summary>
                /// Controls enemy AI.
                /// </summary>
                public int ThinkParamID { get; set; }

                /// <summary>
                /// Controls enemy stats.
                /// </summary>
                public int NPCParamID { get; set; }

                /// <summary>
                /// Controls enemy speech.
                /// </summary>
                public int TalkID { get; set; }

                /// <summary>
                /// Controls enemy equipment.
                /// </summary>
                public int CharaInitID { get; set; }

                /// <summary>
                /// Unknown, probably more paramIDs.
                /// </summary>
                public int UnkT07 { get; set; }
                public int UnkT08 { get; set; }
                public int UnkT09 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT11 { get; set; }
                public int UnkT12 { get; set; }
                public int UnkT13 { get; set; }
                public int UnkT14 { get; set; }
                public int UnkT15 { get; set; }

                public Enemy() { }

                /// <summary>
                /// Creates a new Enemy with the given ID and name.
                /// </summary>
                public Enemy(string name) : base(name, 192, 0)
                {
                    ThinkParamID = 0;
                    NPCParamID = 0;
                    TalkID = 0;
                    CharaInitID = 0;
                    CollisionName = null;
                    UnkT07 = 0;
                    UnkT08 = 0;
                    UnkT09 = 0;
                    UnkT10 = 0;
                    UnkT11 = 0;
                    UnkT12 = 0;
                    UnkT13 = 0;
                    UnkT14 = 0;
                    UnkT15 = 0;
                }

                /// <summary>
                /// Creates a new Enemy with values copied from another.
                /// </summary>
                public Enemy(Enemy clone) : base(clone)
                {
                    ThinkParamID = clone.ThinkParamID;
                    NPCParamID = clone.NPCParamID;
                    TalkID = clone.TalkID;
                    UnkT07 = clone.UnkT07;
                    CharaInitID = clone.CharaInitID;
                    CollisionName = clone.CollisionName;
                    UnkT09 = clone.UnkT09;
                    UnkT10 = clone.UnkT10;
                    UnkT11 = clone.UnkT11;
                    UnkT12 = clone.UnkT12;
                    UnkT13 = clone.UnkT13;
                    UnkT14 = clone.UnkT14;
                    UnkT15 = clone.UnkT15;
                }

                internal Enemy(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ThinkParamID = br.ReadInt32();
                    NPCParamID = br.ReadInt32();
                    TalkID = br.ReadInt32();
                    CharaInitID = br.ReadInt32();
                    UnkT07 = br.ReadInt32();
                    collisionPartIndex = br.ReadInt32();
                    UnkT09 = br.ReadInt32();
                    br.AssertInt32(0);
                    UnkT10 = br.ReadInt32();
                    UnkT11 = br.ReadInt32();
                    UnkT12 = br.ReadInt32();
                    UnkT13 = br.ReadInt32();
                    UnkT14 = br.ReadInt32();
                    UnkT15 = br.ReadInt32();
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ThinkParamID);
                    bw.WriteInt32(NPCParamID);
                    bw.WriteInt32(TalkID);
                    bw.WriteInt32(CharaInitID);
                    bw.WriteInt32(UnkT07);
                    bw.WriteInt32(collisionPartIndex);
                    bw.WriteInt32(UnkT09);
                    bw.WriteInt32(0);
                    bw.WriteInt32(UnkT10);
                    bw.WriteInt32(UnkT11);
                    bw.WriteInt32(UnkT12);
                    bw.WriteInt32(UnkT13);
                    bw.WriteInt32(UnkT14);
                    bw.WriteInt32(UnkT15);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(entries.Parts, collisionPartIndex);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionPartIndex = GetIndex(entries.Parts, CollisionName);
                }
            }

            /// <summary>
            /// Unknown exactly what this is for.
            /// </summary>
            public class Player : Part
            {
                internal override PartsType Type => PartsType.Player;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                public Player() { }

                /// <summary>
                /// Creates a new Player with the given ID and name.
                /// </summary>
                public Player(string name) : base(name, 0, 0) { }

                /// <summary>
                /// Creates a new Player with values copied from another.
                /// </summary>
                public Player(Player clone) : base(clone) { }

                internal Player(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// An invisible collision mesh, also used for death planes.
            /// </summary>
            public class Collision : Part
            {
                /// <summary>
                /// Amount of reverb to apply to sounds.
                /// </summary>
                public enum SoundSpace : byte
                {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                    NoReverb = 0,
                    SmallReverbA = 1,
                    SmallReverbB = 2,
                    MiddleReverbA = 3,
                    MiddleReverbB = 4,
                    LargeReverbA = 5,
                    LargeReverbB = 6,
                    ExtraLargeReverbA = 7,
                    ExtraLargeReverbB = 8,
                    Unknown = 0xFF
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
                }

                internal override PartsType Type => PartsType.Collision;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk4 => true;

                /// <summary>
                /// Gparam IDs for this collision.
                /// </summary>
                public GparamConfig Gparam { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct4 Unk4 { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte HitFilterID { get; set; }

                /// <summary>
                /// Modifies sounds while the player is touching this collision.
                /// </summary>
                public SoundSpace SoundSpaceType { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short EnvLightMapSpotIndex { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float ReflectPlaneHeight { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short MapNameID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                //public bool DisableStart;
                public short UnkT08b { get; set; }

                /// <summary>
                /// Disables a bonfire with this entity ID when an enemy is touching this collision.
                /// </summary>
                public int DisableBonfireEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int PlayRegionID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short LockCamID1 { get; set; }
                public short LockCamID2 { get; set; }

                private int UnkHitIndex;
                /// <summary>
                /// Unknown. Always refers to another collision part.
                /// </summary>
                public string UnkHitName { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT14 { get; set; }

                public Collision() { }

                /// <summary>
                /// Creates a new Collision with the given ID and name.
                /// </summary>
                public Collision(string name) : base(name, 80, 112)
                {
                    HitFilterID = 0;
                    SoundSpaceType = SoundSpace.NoReverb;
                    EnvLightMapSpotIndex = 0;
                    ReflectPlaneHeight = 0;
                    MapNameID = -1;
                    UnkT08b = 0;
                    DisableBonfireEntityID = -1;
                    UnkHitName = null;
                    PlayRegionID = -1;
                    LockCamID1 = 0;
                    LockCamID2 = 0;
                    UnkT14 = 0;
                }

                /// <summary>
                /// Creates a new Collision with values copied from another.
                /// </summary>
                public Collision(Collision clone) : base(clone)
                {
                    HitFilterID = clone.HitFilterID;
                    SoundSpaceType = clone.SoundSpaceType;
                    EnvLightMapSpotIndex = clone.EnvLightMapSpotIndex;
                    ReflectPlaneHeight = clone.ReflectPlaneHeight;
                    MapNameID = clone.MapNameID;
                    UnkT08b = clone.UnkT08b;
                    DisableBonfireEntityID = clone.DisableBonfireEntityID;
                    UnkHitName = clone.UnkHitName;
                    PlayRegionID = clone.PlayRegionID;
                    LockCamID1 = clone.LockCamID1;
                    LockCamID2 = clone.LockCamID2;
                    UnkT14 = clone.UnkT14;
                }

                internal Collision(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    HitFilterID = br.ReadByte();
                    SoundSpaceType = br.ReadEnum8<SoundSpace>();
                    EnvLightMapSpotIndex = br.ReadInt16();
                    ReflectPlaneHeight = br.ReadSingle();
                    MapNameID = br.ReadInt16();
                    UnkT08b = br.ReadInt16();
                    DisableBonfireEntityID = br.ReadInt32();
                    LockCamID1 = br.ReadInt16();
                    LockCamID2 = br.ReadInt16();
                    UnkT14 = br.ReadInt32();
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);
                internal override void ReadUnk4(BinaryReaderEx br) => Unk4 = new UnkStruct4(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteByte(HitFilterID);
                    bw.WriteByte((byte)SoundSpaceType);
                    bw.WriteInt16(EnvLightMapSpotIndex);
                    bw.WriteSingle(ReflectPlaneHeight);
                    bw.WriteInt16(MapNameID);
                    bw.WriteInt16(UnkT08b);
                    bw.WriteInt32(DisableBonfireEntityID);
                    bw.WriteInt16(LockCamID1);
                    bw.WriteInt16(LockCamID2);
                    bw.WriteInt32(UnkT14);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
                internal override void WriteUnk4(BinaryWriterEx bw) => Unk4.Write(bw);

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    UnkHitName = GetName(entries.Parts, UnkHitIndex);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    UnkHitIndex = GetIndex(entries.Parts, UnkHitName);
                }
            }

            // Seemingly doesn't do anything?
            public class Navimesh : Part
            {
                internal override PartsType Type => PartsType.Navmesh;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                public Navimesh() { }

                /// <summary>
                /// Creates a new Unknown with the given ID and name.
                /// </summary>
                public Navimesh(string name) : base(name, 0, 0)
                {
                }

                /// <summary>
                /// Creates a new Unknown with values copied from another.
                /// </summary>
                public Navimesh(Unknown clone) : base(clone)
                {
                }

                internal Navimesh(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    br.AssertInt64(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt64(0);
                }
            }

            /// <summary>
            /// An object that is either unused, or used for a cutscene.
            /// </summary>
            public class DummyObject : Object
            {
                internal override PartsType Type => PartsType.DummyObject;

                public DummyObject() { }

                /// <summary>
                /// Creates a new DummyObject with the given ID and name.
                /// </summary>
                public DummyObject(string name) : base(name) { }

                /// <summary>
                /// Creates a new DummyObject with values copied from another.
                /// </summary>
                public DummyObject(DummyObject clone) : base(clone) { }

                internal DummyObject(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// An enemy that is either unused, or used for a cutscene.
            /// </summary>
            public class DummyEnemy : Enemy
            {
                internal override PartsType Type => PartsType.DummyEnemy;

                public DummyEnemy() { }

                /// <summary>
                /// Creates a new DummyEnemy with the given ID and name.
                /// </summary>
                public DummyEnemy(string name) : base(name) { }

                /// <summary>
                /// Creates a new DummyEnemy with values copied from another.
                /// </summary>
                public DummyEnemy(DummyEnemy clone) : base(clone) { }

                internal DummyEnemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Determines which collision parts load other maps.
            /// </summary>
            public class ConnectCollision : Part
            {
                internal override PartsType Type => PartsType.ConnectCollision;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                private int collisionIndex;
                /// <summary>
                /// The name of the associated collision part.
                /// </summary>
                public string CollisionName { get; set; }

                /// <summary>
                /// A map ID in format mXX_XX_XX_XX.
                /// </summary>
                public byte MapID1 { get; set; }
                public byte MapID2 { get; set; }
                public byte MapID3 { get; set; }
                public byte MapID4 { get; set; }

                public ConnectCollision() { }

                /// <summary>
                /// Creates a new ConnectCollision with the given ID and name.
                /// </summary>
                public ConnectCollision(string name) : base(name, 0, 0)
                {
                    CollisionName = null;
                    MapID1 = 0;
                    MapID2 = 0;
                    MapID3 = 0;
                    MapID4 = 0;
                }

                /// <summary>
                /// Creates a new ConnectCollision with values copied from another.
                /// </summary>
                public ConnectCollision(ConnectCollision clone) : base(clone)
                {
                    CollisionName = clone.CollisionName;
                    MapID1 = clone.MapID1;
                    MapID2 = clone.MapID2;
                    MapID3 = clone.MapID3;
                    MapID4 = clone.MapID4;
                }

                internal ConnectCollision(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    collisionIndex = br.ReadInt32();
                    MapID1 = br.ReadByte();
                    MapID2 = br.ReadByte();
                    MapID3 = br.ReadByte();
                    MapID4 = br.ReadByte();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(collisionIndex);
                    bw.WriteByte(MapID1);
                    bw.WriteByte(MapID2);
                    bw.WriteByte(MapID3);
                    bw.WriteByte(MapID4);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = GetName(msb.Parts.Collisions, collisionIndex);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    collisionIndex = GetIndex(msb.Parts.Collisions, CollisionName);
                }
            }

            public class Unknown : Part
            {
                internal override PartsType Type => PartsType.Unknown;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk4 => false;

                public Unknown() { }

                /// <summary>
                /// Creates a new Unknown with the given ID and name.
                /// </summary>
                public Unknown(string name) : base(name, 0, 0)
                {
                }

                /// <summary>
                /// Creates a new Unknown with values copied from another.
                /// </summary>
                public Unknown(Unknown clone) : base(clone)
                {
                }

                internal Unknown(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                }
            }
        }
    }
}
