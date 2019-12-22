using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum PartType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Player = 4,
            Collision = 5,
            DummyObject = 9,
            DummyEnemy = 10,
            ConnectCollision = 11,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Instances of actual things in the map.
        /// </summary>
        public class PartsParam : Param<Part>, IMsbParam<IMsbPart>
        {
            /// <summary>
            /// All of the fixed visual geometry of the map.
            /// </summary>
            public List<Part.MapPiece> MapPieces { get; set; }

            /// <summary>
            /// Dynamic props and interactive things.
            /// </summary>
            public List<Part.Object> Objects { get; set; }

            /// <summary>
            /// All non-player characters.
            /// </summary>
            public List<Part.Enemy> Enemies { get; set; }

            /// <summary>
            /// These have something to do with player spawn points.
            /// </summary>
            public List<Part.Player> Players { get; set; }

            /// <summary>
            /// Invisible physical geometry of the map.
            /// </summary>
            public List<Part.Collision> Collisions { get; set; }

            /// <summary>
            /// Objects that don't appear normally; either unused, or used for cutscenes.
            /// </summary>
            public List<Part.DummyObject> DummyObjects { get; set; }

            /// <summary>
            /// Enemies that don't appear normally; either unused, or used for cutscenes.
            /// </summary>
            public List<Part.DummyEnemy> DummyEnemies { get; set; }

            /// <summary>
            /// Dummy parts that reference an actual collision and cause it to load another map.
            /// </summary>
            public List<Part.ConnectCollision> ConnectCollisions { get; set; }

            /// <summary>
            /// Creates an empty PartsParam with the given version.
            /// </summary>
            public PartsParam(int unk00 = 0x23) : base(unk00, "PARTS_PARAM_ST")
            {
                MapPieces = new List<Part.MapPiece>();
                Objects = new List<Part.Object>();
                Enemies = new List<Part.Enemy>();
                Players = new List<Part.Player>();
                Collisions = new List<Part.Collision>();
                DummyObjects = new List<Part.DummyObject>();
                DummyEnemies = new List<Part.DummyEnemy>();
                ConnectCollisions = new List<Part.ConnectCollision>();
            }

            internal override Part ReadEntry(BinaryReaderEx br)
            {
                PartType type = br.GetEnum32<PartType>(br.Position + 8);
                switch (type)
                {
                    case PartType.MapPiece:
                        var mapPiece = new Part.MapPiece(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case PartType.Object:
                        var obj = new Part.Object(br);
                        Objects.Add(obj);
                        return obj;

                    case PartType.Enemy:
                        var enemy = new Part.Enemy(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case PartType.Player:
                        var player = new Part.Player(br);
                        Players.Add(player);
                        return player;

                    case PartType.Collision:
                        var collision = new Part.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    case PartType.DummyObject:
                        var dummyObject = new Part.DummyObject(br);
                        DummyObjects.Add(dummyObject);
                        return dummyObject;

                    case PartType.DummyEnemy:
                        var dummyEnemy = new Part.DummyEnemy(br);
                        DummyEnemies.Add(dummyEnemy);
                        return dummyEnemy;

                    case PartType.ConnectCollision:
                        var connectCollision = new Part.ConnectCollision(br);
                        ConnectCollisions.Add(connectCollision);
                        return connectCollision;

                    default:
                        throw new NotImplementedException($"Unimplemented part type: {type}");
                }
            }

            /// <summary>
            /// Returns every Part in the order they'll be written.
            /// </summary>
            public override List<Part> GetEntries()
            {
                return SFUtil.ConcatAll<Part>(
                    MapPieces, Objects, Enemies, Players, Collisions,
                    DummyObjects, DummyEnemies, ConnectCollisions);
            }
            IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() => GetEntries();

            public void Add(IMsbPart item)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Common data for all types of part.
        /// </summary>
        public abstract class Part : Entry, IMsbPart
        {
            /// <summary>
            /// The specific type of this part.
            /// </summary>
            public abstract PartType Type { get; }

            internal abstract bool HasUnk1 { get; }
            internal abstract bool HasUnk2 { get; }
            internal abstract bool HasGparamConfig { get; }
            internal abstract bool HasUnk6 { get; }
            internal abstract bool HasUnk7 { get; }

            /// <summary>
            /// The model used by this part; requires an entry in ModelParam.
            /// </summary>
            public string ModelName { get; set; }
            private int ModelIndex;

            /// <summary>
            /// A path to a .sib file, presumably some kind of editor placeholder.
            /// </summary>
            public string Placeholder { get; set; }

            /// <summary>
            /// Location of the part.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the part.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Scale of the part; only works for map pieces and objects.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// Identifies the part in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE04 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE05 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE06 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte LanternID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte LodParamID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE09 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool IsPointLightShadowSrc { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE0B { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool IsShadowSrc { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte IsStaticShadowSrc { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte IsCascade3ShadowSrc { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE0F { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE10 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool IsShadowDest { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool IsShadowOnly { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DrawByReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DrawOnlyReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte EnableOnAboveShadow { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool DisablePointLightEffect { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UnkE17 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int UnkE18 { get; set; }

            /// <summary>
            /// Allows multiple parts to be identified by the same entity ID.
            /// </summary>
            public int[] EntityGroupIDs { get; private set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int UnkE3C { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int UnkE40 { get; set; }

            internal Part()
            {
                Name = "";
                Placeholder = "";
                Scale = Vector3.One;
                EntityID = -1;
                EntityGroupIDs = new int[8];
                for (int i = 0; i < 8; i++)
                    EntityGroupIDs[i] = -1;
            }

            internal Part(Part clone)
            {
                Name = clone.Name;
                ModelName = clone.ModelName;
                Placeholder = clone.Placeholder;
                Position = clone.Position;
                Rotation = clone.Rotation;
                Scale = clone.Scale;
                EntityID = clone.EntityID;
                UnkE04 = clone.UnkE04;
                UnkE05 = clone.UnkE05;
                UnkE06 = clone.UnkE06;
                LanternID = clone.LanternID;
                LodParamID = clone.LodParamID;
                UnkE09 = clone.UnkE09;
                IsPointLightShadowSrc = clone.IsPointLightShadowSrc;
                UnkE0B = clone.UnkE0B;
                IsShadowSrc = clone.IsShadowSrc;
                IsStaticShadowSrc = clone.IsStaticShadowSrc;
                IsCascade3ShadowSrc = clone.IsCascade3ShadowSrc;
                UnkE0F = clone.UnkE0F;
                UnkE10 = clone.UnkE10;
                IsShadowDest = clone.IsShadowDest;
                IsShadowOnly = clone.IsShadowOnly;
                DrawByReflectCam = clone.DrawByReflectCam;
                DrawOnlyReflectCam = clone.DrawOnlyReflectCam;
                EnableOnAboveShadow = clone.EnableOnAboveShadow;
                DisablePointLightEffect = clone.DisablePointLightEffect;
                UnkE17 = clone.UnkE17;
                UnkE18 = clone.UnkE18;
                EntityGroupIDs = (int[])clone.EntityGroupIDs.Clone();
                UnkE3C = clone.UnkE3C;
                UnkE40 = clone.UnkE40;
            }

            internal Part(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                ModelIndex = br.ReadInt32();
                br.AssertInt32(0);
                long sibOffset = br.ReadInt64();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();
                br.AssertInt32(-1);
                br.AssertInt32(-1);
                br.AssertInt32(0);
                long unkOffset1 = br.ReadInt64();
                long unkOffset2 = br.ReadInt64();
                long entityDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();
                long gparamOffset = br.ReadInt64();
                long unkOffset6 = br.ReadInt64();
                long unkOffset7 = br.ReadInt64();
                br.AssertInt64(0);
                br.AssertInt64(0);
                br.AssertInt64(0);

                Name = br.GetUTF16(start + nameOffset);
                Placeholder = br.GetUTF16(start + sibOffset);
                if (HasUnk1)
                {
                    br.Position = start + unkOffset1;
                    ReadUnk1(br);
                }
                if (HasUnk2)
                {
                    br.Position = start + unkOffset2;
                    ReadUnk2(br);
                }

                br.Position = start + entityDataOffset;
                EntityID = br.ReadInt32();
                UnkE04 = br.ReadByte();
                UnkE05 = br.ReadByte();
                UnkE06 = br.ReadByte();
                LanternID = br.ReadByte();
                LodParamID = br.ReadByte();
                UnkE09 = br.ReadByte();
                IsPointLightShadowSrc = br.ReadBoolean();
                UnkE0B = br.ReadByte();
                IsShadowSrc = br.ReadBoolean();
                IsStaticShadowSrc = br.ReadByte();
                IsCascade3ShadowSrc = br.ReadByte();
                UnkE0F = br.ReadByte();
                UnkE10 = br.ReadByte();
                IsShadowDest = br.ReadBoolean();
                IsShadowOnly = br.ReadBoolean();
                DrawByReflectCam = br.ReadBoolean();
                DrawOnlyReflectCam = br.ReadBoolean();
                EnableOnAboveShadow = br.ReadByte();
                DisablePointLightEffect = br.ReadBoolean();
                UnkE17 = br.ReadByte();
                UnkE18 = br.ReadInt32();
                EntityGroupIDs = br.ReadInt32s(8);
                UnkE3C = br.ReadInt32();
                UnkE40 = br.ReadInt32();
                br.AssertPattern(0x10, 0x00);

                if (HasGparamConfig)
                {
                    br.Position = start + gparamOffset;
                    ReadGparamConfig(br);
                }
                if (HasUnk6)
                {
                    br.Position = start + unkOffset6;
                    ReadUnk6(br);
                }
                if (HasUnk7)
                {
                    br.Position = start + unkOffset7;
                    ReadUnk7(br);
                }
                br.Position = start + typeDataOffset;
            }

            internal virtual void ReadUnk1(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 1 should not be read for parts with no unk struct 1.");
            }

            internal virtual void ReadUnk2(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 2 should not be read for parts with no unk struct 2.");
            }

            internal virtual void ReadGparamConfig(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 5 should not be read for parts with no unk struct 5.");
            }

            internal virtual void ReadUnk6(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 6 should not be read for parts with no unk struct 6.");
            }

            internal virtual void ReadUnk7(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Unk struct 7 should not be read for parts with no unk struct 7.");
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteInt32(ModelIndex);
                bw.WriteInt32(0);
                bw.ReserveInt64("SibOffset");
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);
                bw.WriteInt32(-1);
                bw.WriteInt32(-1);
                bw.WriteInt32(0);
                bw.ReserveInt64("UnkOffset1");
                bw.ReserveInt64("UnkOffset2");
                bw.ReserveInt64("EntityDataOffset");
                bw.ReserveInt64("TypeDataOffset");
                bw.ReserveInt64("GparamOffset");
                bw.ReserveInt64("UnkOffset6");
                bw.ReserveInt64("UnkOffset7");
                bw.WriteInt64(0);
                bw.WriteInt64(0);
                bw.WriteInt64(0);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
                bw.FillInt64("SibOffset", bw.Position - start);
                bw.WriteUTF16(Placeholder, true);
                bw.Pad(8);

                if (HasUnk1)
                {
                    bw.FillInt64("UnkOffset1", bw.Position - start);
                    WriteUnk1(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset1", 0);
                }

                if (HasUnk2)
                {
                    bw.FillInt64("UnkOffset2", bw.Position - start);
                    WriteUnk2(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset2", 0);
                }

                bw.FillInt64("EntityDataOffset", bw.Position - start);
                bw.WriteInt32(EntityID);
                bw.WriteByte(UnkE04);
                bw.WriteByte(UnkE05);
                bw.WriteByte(UnkE06);
                bw.WriteByte(LanternID);
                bw.WriteByte(LodParamID);
                bw.WriteByte(UnkE09);
                bw.WriteBoolean(IsPointLightShadowSrc);
                bw.WriteByte(UnkE0B);
                bw.WriteBoolean(IsShadowSrc);
                bw.WriteByte(IsStaticShadowSrc);
                bw.WriteByte(IsCascade3ShadowSrc);
                bw.WriteByte(UnkE0F);
                bw.WriteByte(UnkE10);
                bw.WriteBoolean(IsShadowDest);
                bw.WriteBoolean(IsShadowOnly);
                bw.WriteBoolean(DrawByReflectCam);
                bw.WriteBoolean(DrawOnlyReflectCam);
                bw.WriteByte(EnableOnAboveShadow);
                bw.WriteBoolean(DisablePointLightEffect);
                bw.WriteByte(UnkE17);
                bw.WriteInt32(UnkE18);
                bw.WriteInt32s(EntityGroupIDs);
                bw.WriteInt32(UnkE3C);
                bw.WriteInt32(UnkE40);
                bw.WritePattern(0x10, 0x00);
                bw.Pad(8);

                bw.FillInt64("TypeDataOffset", bw.Position - start);
                WriteTypeData(bw);

                if (HasGparamConfig)
                {
                    bw.FillInt64("GparamOffset", bw.Position - start);
                    WriteGparamConfig(bw);
                }
                else
                {
                    bw.FillInt64("GparamOffset", 0);
                }

                if (HasUnk6)
                {
                    bw.FillInt64("UnkOffset6", bw.Position - start);
                    WriteUnk6(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset6", 0);
                }

                if (HasUnk7)
                {
                    bw.FillInt64("UnkOffset7", bw.Position - start);
                    WriteUnk7(bw);
                }
                else
                {
                    bw.FillInt64("UnkOffset7", 0);
                }
            }

            internal abstract void WriteTypeData(BinaryWriterEx bw);

            internal virtual void WriteUnk1(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 1 should not be written for parts with no unk struct 1.");
            }

            internal virtual void WriteUnk2(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 2 should not be written for parts with no unk struct 2.");
            }

            internal virtual void WriteGparamConfig(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 5 should not be written for parts with no unk struct 5.");
            }

            internal virtual void WriteUnk6(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 6 should not be written for parts with no unk struct 6.");
            }

            internal virtual void WriteUnk7(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Unk struct 7 should not be written for parts with no unk struct 7.");
            }

            internal virtual void GetNames(MSBS msb, Entries entries)
            {
                ModelName = MSB.FindName(entries.Models, ModelIndex);
            }

            internal virtual void GetIndices(MSBS msb, Entries entries)
            {
                ModelIndex = MSB.FindIndex(entries.Models, ModelName);
            }

            /// <summary>
            /// Returns the type and name of the part as a string.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {Name}";
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class UnkStruct1
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public uint[] CollisionMask { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Condition1 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Condition2 { get; set; }

                /// <summary>
                /// Creates an UnkStruct1 with default values.
                /// </summary>
                public UnkStruct1()
                {
                    CollisionMask = new uint[48];
                    Condition1 = 0;
                    Condition2 = 0;
                }

                /// <summary>
                /// Clones an existing UnkStruct1.
                /// </summary>
                public UnkStruct1(UnkStruct1 clone)
                {
                    CollisionMask = (uint[])clone.CollisionMask.Clone();
                    Condition1 = clone.Condition1;
                    Condition2 = clone.Condition2;
                }

                internal UnkStruct1(BinaryReaderEx br)
                {
                    CollisionMask = br.ReadUInt32s(48);
                    Condition1 = br.ReadByte();
                    Condition2 = br.ReadByte();
                    br.AssertInt16(0);
                    br.AssertPattern(0xC0, 0x00);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteUInt32s(CollisionMask);
                    bw.WriteByte(Condition1);
                    bw.WriteByte(Condition2);
                    bw.WriteInt16(0);
                    bw.WritePattern(0xC0, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class UnkStruct2
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public int Condition { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int[] DispGroups { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk26 { get; set; }

                /// <summary>
                /// Creates an UnkStruct2 with default values.
                /// </summary>
                public UnkStruct2()
                {
                    DispGroups = new int[8];
                }

                internal UnkStruct2(BinaryReaderEx br)
                {
                    Condition = br.ReadInt32();
                    DispGroups = br.ReadInt32s(8);
                    Unk24 = br.ReadInt16();
                    Unk26 = br.ReadInt16();
                    br.AssertPattern(0x20, 0x00);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Condition);
                    bw.WriteInt32s(DispGroups);
                    bw.WriteInt16(Unk24);
                    bw.WriteInt16(Unk26);
                    bw.WritePattern(0x20, 0x00);
                }
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
                /// ID of the value set from LightScattering : ParamEditor to use.
                /// </summary>
                public int LightScatteringID { get; set; }

                /// <summary>
                /// ID of the value set from Env Map:Editor to use.
                /// </summary>
                public int EnvMapID { get; set; }

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
                    LightScatteringID = clone.LightScatteringID;
                    EnvMapID = clone.EnvMapID;
                }

                internal GparamConfig(BinaryReaderEx br)
                {
                    LightSetID = br.ReadInt32();
                    FogParamID = br.ReadInt32();
                    LightScatteringID = br.ReadInt32();
                    EnvMapID = br.ReadInt32();
                    br.AssertPattern(0x10, 0x00);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(LightSetID);
                    bw.WriteInt32(FogParamID);
                    bw.WriteInt32(LightScatteringID);
                    bw.WriteInt32(EnvMapID);
                    bw.WritePattern(0x10, 0x00);
                }

                /// <summary>
                /// Returns the four gparam values as a string.
                /// </summary>
                public override string ToString()
                {
                    return $"{LightSetID}, {FogParamID}, {LightScatteringID}, {EnvMapID}";
                }
            }

            /// <summary>
            /// Unknown; sceneGParam Struct according to Pav.
            /// </summary>
            public class UnkStruct6
            {
                /// <summary>
                /// Unknown; 4 bytes.
                /// </summary>
                public sbyte[] EventIDs { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float Unk40 { get; set; }

                /// <summary>
                /// Creates an UnkStruct6 with default values.
                /// </summary>
                public UnkStruct6()
                {
                    EventIDs = new sbyte[4];
                }

                internal UnkStruct6(BinaryReaderEx br)
                {
                    br.AssertPattern(0x3C, 0x00);
                    EventIDs = br.ReadSBytes(4);
                    Unk40 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WritePattern(0x3C, 0x00);
                    bw.WriteSBytes(EventIDs);
                    bw.WriteSingle(Unk40);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                /// <summary>
                /// Returns a string representation of the object.
                /// </summary>
                public override string ToString()
                {
                    return $"EventID[{EventIDs[0],2}][{EventIDs[1],2}][{EventIDs[2],2}][{EventIDs[3],2}] {Unk40:0.0}";
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class UnkStruct7
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
                /// ID in GrassTypeParam determining properties of dynamic grass on a map piece.
                /// </summary>
                public int GrassTypeParamID { get; set; }

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
                /// Creates an UnkStruct7 with default values.
                /// </summary>
                public UnkStruct7() { }

                internal UnkStruct7(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    GrassTypeParamID = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(GrassTypeParamID);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Fixed visual geometry.
            /// </summary>
            public class MapPiece : Part
            {
                /// <summary>
                /// PartType.MapPiece
                /// </summary>
                public override PartType Type => PartType.MapPiece;

                internal override bool HasUnk1 => true;
                internal override bool HasUnk2 => false;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk6 => false;
                internal override bool HasUnk7 => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct1 Unk1 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct7 Unk7 { get; set; }

                /// <summary>
                /// Creates a MapPiece with default values.
                /// </summary>
                public MapPiece() : base()
                {
                    Unk1 = new UnkStruct1();
                    Gparam = new GparamConfig();
                    Unk7 = new UnkStruct7();
                }

                internal MapPiece(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadUnk1(BinaryReaderEx br) => Unk1 = new UnkStruct1(br);
                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);
                internal override void ReadUnk7(BinaryReaderEx br) => Unk7 = new UnkStruct7(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteUnk1(BinaryWriterEx bw) => Unk1.Write(bw);
                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
                internal override void WriteUnk7(BinaryWriterEx bw) => Unk7.Write(bw);
            }

            /// <summary>
            /// A dynamic or interactible element in the map.
            /// </summary>
            public class Object : DummyObject
            {
                /// <summary>
                /// PartType.Object
                /// </summary>
                public override PartType Type => PartType.Object;

                internal override bool HasUnk1 => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct1 Unk1 { get; set; }

                /// <summary>
                /// Creates an Object with default values.
                /// </summary>
                public Object() : base()
                {
                    Unk1 = new UnkStruct1();
                }

                /// <summary>
                /// Clones an existing Object.
                /// </summary>
                public Object(Object clone) : base(clone)
                {
                    Unk1 = new UnkStruct1(clone.Unk1);
                }

                /// <summary>
                /// Clones an existing DummyObject.
                /// </summary>
                public Object(DummyObject clone) : base(clone)
                {
                    Unk1 = new UnkStruct1();
                }

                internal Object(BinaryReaderEx br) : base(br) { }

                internal override void ReadUnk1(BinaryReaderEx br) => Unk1 = new UnkStruct1(br);

                internal override void WriteUnk1(BinaryWriterEx bw) => Unk1.Write(bw);
            }

            /// <summary>
            /// Any non-player character.
            /// </summary>
            public class Enemy : DummyEnemy
            {
                /// <summary>
                /// PartType.Enemy
                /// </summary>
                public override PartType Type => PartType.Enemy;

                internal override bool HasUnk1 => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct1 Unk1 { get; set; }

                /// <summary>
                /// Creates an Enemy with default values.
                /// </summary>
                public Enemy() : base()
                {
                    Unk1 = new UnkStruct1();
                }

                /// <summary>
                /// Clones an existing Enemy.
                /// </summary>
                public Enemy(Enemy clone) : base(clone)
                {
                    Unk1 = new UnkStruct1(clone.Unk1);
                }

                /// <summary>
                /// Clones an existing DummyEnemy.
                /// </summary>
                public Enemy(DummyEnemy clone) : base(clone)
                {
                    Unk1 = new UnkStruct1();
                }

                internal Enemy(BinaryReaderEx br) : base(br) { }

                internal override void ReadUnk1(BinaryReaderEx br) => Unk1 = new UnkStruct1(br);

                internal override void WriteUnk1(BinaryWriterEx bw) => Unk1.Write(bw);
            }

            /// <summary>
            /// A spawn point for the player, or something.
            /// </summary>
            public class Player : Part
            {
                /// <summary>
                /// PartType.Player
                /// </summary>
                public override PartType Type => PartType.Player;

                internal override bool HasUnk1 => false;
                internal override bool HasUnk2 => false;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk6 => false;
                internal override bool HasUnk7 => false;

                /// <summary>
                /// Creates a Player with default values.
                /// </summary>
                public Player() : base() { }

                internal Player(BinaryReaderEx br) : base(br)
                {
                    br.AssertPattern(0x10, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WritePattern(0x10, 0x00);
                }
            }

            /// <summary>
            /// Invisible but physical geometry.
            /// </summary>
            public class Collision : Part
            {
                /// <summary>
                /// PartType.Collision
                /// </summary>
                public override PartType Type => PartType.Collision;

                internal override bool HasUnk1 => true;
                internal override bool HasUnk2 => true;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk6 => true;
                internal override bool HasUnk7 => false;

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct1 Unk1 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct2 Unk2 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct6 Unk6 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte HitFilterID { get; set; }

                /// <summary>
                /// Adds reverb to sounds while on this collision to simulate echoes.
                /// </summary>
                public byte SoundSpaceType { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float ReflectPlaneHeight { get; set; }

                /// <summary>
                /// Determines the text to display for map popups and save files.
                /// </summary>
                public short MapNameID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool DisableStart { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT17 { get; set; }

                /// <summary>
                /// If not -1, the bonfire with this ID will be disabled when enemies are on this collision.
                /// </summary>
                public int DisableBonfireEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT25 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT26 { get; set; }

                /// <summary>
                /// Should alter visibility while on this collision, but doesn't seem to do much.
                /// </summary>
                public byte MapVisibility { get; set; }

                /// <summary>
                /// Used to determine invasion eligibility.
                /// </summary>
                public int PlayRegionID { get; set; }

                /// <summary>
                /// Alters camera properties while on this collision.
                /// </summary>
                public short LockCamParamID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT3C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT40 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT44 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT48 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT4C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT50 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT54 { get; set; }

                /// <summary>
                /// Creates a Collision with default values.
                /// </summary>
                public Collision() : base()
                {
                    Unk1 = new UnkStruct1();
                    Unk2 = new UnkStruct2();
                    Gparam = new GparamConfig();
                    Unk6 = new UnkStruct6();
                    DisableBonfireEntityID = -1;
                }

                internal Collision(BinaryReaderEx br) : base(br)
                {
                    HitFilterID = br.ReadByte(); // Pav says Type, did it change?
                    SoundSpaceType = br.ReadByte();
                    br.AssertInt16(0);
                    ReflectPlaneHeight = br.ReadSingle();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    MapNameID = br.ReadInt16();
                    DisableStart = br.ReadBoolean();
                    UnkT17 = br.ReadByte();
                    DisableBonfireEntityID = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    UnkT24 = br.ReadByte();
                    UnkT25 = br.ReadByte();
                    UnkT26 = br.ReadByte();
                    MapVisibility = br.ReadByte();
                    PlayRegionID = br.ReadInt32();
                    LockCamParamID = br.ReadInt16();
                    br.AssertInt16(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    UnkT3C = br.ReadInt32();
                    UnkT40 = br.ReadInt32();
                    UnkT44 = br.ReadSingle();
                    UnkT48 = br.ReadSingle();
                    UnkT4C = br.ReadInt32();
                    UnkT50 = br.ReadSingle();
                    UnkT54 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadUnk1(BinaryReaderEx br) => Unk1 = new UnkStruct1(br);
                internal override void ReadUnk2(BinaryReaderEx br) => Unk2 = new UnkStruct2(br);
                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);
                internal override void ReadUnk6(BinaryReaderEx br) => Unk6 = new UnkStruct6(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteByte(HitFilterID);
                    bw.WriteByte(SoundSpaceType);
                    bw.WriteInt16(0);
                    bw.WriteSingle(ReflectPlaneHeight);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt16(MapNameID);
                    bw.WriteBoolean(DisableStart);
                    bw.WriteByte(UnkT17);
                    bw.WriteInt32(DisableBonfireEntityID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteByte(UnkT24);
                    bw.WriteByte(UnkT25);
                    bw.WriteByte(UnkT26);
                    bw.WriteByte(MapVisibility);
                    bw.WriteInt32(PlayRegionID);
                    bw.WriteInt16(LockCamParamID);
                    bw.WriteInt16(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT3C);
                    bw.WriteInt32(UnkT40);
                    bw.WriteSingle(UnkT44);
                    bw.WriteSingle(UnkT48);
                    bw.WriteInt32(UnkT4C);
                    bw.WriteSingle(UnkT50);
                    bw.WriteSingle(UnkT54);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteUnk1(BinaryWriterEx bw) => Unk1.Write(bw);
                internal override void WriteUnk2(BinaryWriterEx bw) => Unk2.Write(bw);
                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);
                internal override void WriteUnk6(BinaryWriterEx bw) => Unk6.Write(bw);
            }

            /// <summary>
            /// An object that either isn't used, or is used for a cutscene.
            /// </summary>
            public class DummyObject : Part
            {
                /// <summary>
                /// PartType.DummyObject
                /// </summary>
                public override PartType Type => PartType.DummyObject;

                internal override bool HasUnk1 => false;
                internal override bool HasUnk2 => false;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk6 => false;
                internal override bool HasUnk7 => false;

                /// <summary>
                /// Unknown.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// Reference to a map piece or collision; believed to determine when the object is loaded.
                /// </summary>
                public string ObjPartName1 { get; set; }
                private int ObjPartIndex1;

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool EnableObjAnimNetSyncStructure { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT0E { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool SetMainObjStructureBooleans { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short AnimID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT1A { get; set; }

                /// <summary>
                /// Reference to a collision; believed to be involved with loading when grappling to the object.
                /// </summary>
                public string ObjPartName2 { get; set; }
                private int ObjPartIndex2;

                /// <summary>
                /// Reference to a collision; believed to be involved with loading when grappling to the object.
                /// </summary>
                public string ObjPartName3 { get; set; }
                private int ObjPartIndex3;

                /// <summary>
                /// Creates a DummyObject with default values.
                /// </summary>
                public DummyObject() : base()
                {
                    Gparam = new GparamConfig();
                }

                /// <summary>
                /// Clones an existing DummyObject.
                /// </summary>
                public DummyObject(DummyObject clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    ObjPartName1 = clone.ObjPartName1;
                    UnkT0C = clone.UnkT0C;
                    EnableObjAnimNetSyncStructure = clone.EnableObjAnimNetSyncStructure;
                    UnkT0E = clone.UnkT0E;
                    SetMainObjStructureBooleans = clone.SetMainObjStructureBooleans;
                    AnimID = clone.AnimID;
                    UnkT18 = clone.UnkT18;
                    UnkT1A = clone.UnkT1A;
                    ObjPartName2 = clone.ObjPartName2;
                    ObjPartName3 = clone.ObjPartName3;
                }

                internal DummyObject(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ObjPartIndex1 = br.ReadInt32();
                    UnkT0C = br.ReadByte();
                    EnableObjAnimNetSyncStructure = br.ReadBoolean();
                    UnkT0E = br.ReadByte();
                    SetMainObjStructureBooleans = br.ReadBoolean();
                    AnimID = br.ReadInt16();
                    br.AssertInt16(-1);
                    br.AssertInt32(-1);
                    UnkT18 = br.ReadInt16();
                    UnkT1A = br.ReadInt16();
                    br.AssertInt32(-1);
                    ObjPartIndex2 = br.ReadInt32();
                    ObjPartIndex3 = br.ReadInt32();
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ObjPartIndex1);
                    bw.WriteByte(UnkT0C);
                    bw.WriteBoolean(EnableObjAnimNetSyncStructure);
                    bw.WriteByte(UnkT0E);
                    bw.WriteBoolean(SetMainObjStructureBooleans);
                    bw.WriteInt16(AnimID);
                    bw.WriteInt16(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt16(UnkT18);
                    bw.WriteInt16(UnkT1A);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(ObjPartIndex2);
                    bw.WriteInt32(ObjPartIndex3);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    ObjPartName1 = MSB.FindName(entries.Parts, ObjPartIndex1);
                    ObjPartName2 = MSB.FindName(entries.Parts, ObjPartIndex2);
                    ObjPartName3 = MSB.FindName(entries.Parts, ObjPartIndex3);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    ObjPartIndex1 = MSB.FindIndex(entries.Parts, ObjPartName1);
                    ObjPartIndex2 = MSB.FindIndex(entries.Parts, ObjPartName2);
                    ObjPartIndex3 = MSB.FindIndex(entries.Parts, ObjPartName3);
                }
            }

            /// <summary>
            /// An enemy that either isn't used, or is used for a cutscene.
            /// </summary>
            public class DummyEnemy : Part
            {
                /// <summary>
                /// PartType.DummyEnemy
                /// </summary>
                public override PartType Type => PartType.DummyEnemy;

                internal override bool HasUnk1 => false;
                internal override bool HasUnk2 => false;
                internal override bool HasGparamConfig => true;
                internal override bool HasUnk6 => false;
                internal override bool HasUnk7 => false;

                /// <summary>
                /// Unknown.
                /// </summary>
                public GparamConfig Gparam { get; set; }

                /// <summary>
                /// An ID in NPCThinkParam that determines the enemy's AI characteristics.
                /// </summary>
                public int ThinkParamID { get; set; }

                /// <summary>
                /// An ID in NPCParam that determines a variety of enemy properties.
                /// </summary>
                public int NPCParamID { get; set; }

                /// <summary>
                /// Unknown; previously talk ID, now always 0 or 1 except for the Memorial Mob in Senpou.
                /// </summary>
                public int UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short ChrManipulatorAllocationParameter { get; set; }

                /// <summary>
                /// An ID in CharaInitParam that determines a human's inventory and stats.
                /// </summary>
                public int CharaInitID { get; set; }

                /// <summary>
                /// Should reference the collision the enemy starts on.
                /// </summary>
                public string CollisionPartName { get; set; }
                private int CollisionPartIndex;

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT20 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT22 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int BackupEventAnimID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int EventFlagID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int EventFlagCompareState { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT48 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT4C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT50 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT78 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT84 { get; set; }

                /// <summary>
                /// Creates a DummyEnemy with default values.
                /// </summary>
                public DummyEnemy() : base()
                {
                    Gparam = new GparamConfig();
                    ThinkParamID = -1;
                    NPCParamID = -1;
                    UnkT10 = -1;
                    CharaInitID = -1;
                    BackupEventAnimID = -1;
                    EventFlagID = -1;
                }

                /// <summary>
                /// Clones an existing DummyEnemy.
                /// </summary>
                public DummyEnemy(DummyEnemy clone) : base(clone)
                {
                    Gparam = new GparamConfig(clone.Gparam);
                    ThinkParamID = clone.ThinkParamID;
                    NPCParamID = clone.NPCParamID;
                    UnkT10 = clone.UnkT10;
                    ChrManipulatorAllocationParameter = clone.ChrManipulatorAllocationParameter;
                    CharaInitID = clone.CharaInitID;
                    CollisionPartName = clone.CollisionPartName;
                    UnkT20 = clone.UnkT20;
                    UnkT22 = clone.UnkT22;
                    UnkT24 = clone.UnkT24;
                    BackupEventAnimID = clone.BackupEventAnimID;
                    EventFlagID = clone.EventFlagID;
                    EventFlagCompareState = clone.EventFlagCompareState;
                    UnkT48 = clone.UnkT48;
                    UnkT4C = clone.UnkT4C;
                    UnkT50 = clone.UnkT50;
                    UnkT78 = clone.UnkT78;
                    UnkT84 = clone.UnkT84;
                }

                internal DummyEnemy(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ThinkParamID = br.ReadInt32();
                    NPCParamID = br.ReadInt32();
                    UnkT10 = br.ReadInt32();
                    br.AssertInt16(0);
                    ChrManipulatorAllocationParameter = br.ReadInt16();
                    CharaInitID = br.ReadInt32();
                    CollisionPartIndex = br.ReadInt32();
                    UnkT20 = br.ReadInt16();
                    UnkT22 = br.ReadInt16();
                    UnkT24 = br.ReadInt32();
                    br.AssertPattern(0x10, 0xFF);
                    BackupEventAnimID = br.ReadInt32();
                    br.AssertInt32(-1);
                    EventFlagID = br.ReadInt32();
                    EventFlagCompareState = br.ReadInt32();
                    UnkT48 = br.ReadInt32();
                    UnkT4C = br.ReadInt32();
                    UnkT50 = br.ReadInt32();
                    br.AssertInt32(1);
                    br.AssertInt32(-1);
                    br.AssertInt32(1);
                    br.AssertPattern(0x18, 0x00);
                    UnkT78 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    UnkT84 = br.ReadSingle();
                    for (int i = 0; i < 5; i++)
                    {
                        br.AssertInt32(-1);
                        br.AssertInt16(-1);
                        br.AssertInt16(0xA);
                    }
                    br.AssertPattern(0x10, 0x00);
                }

                internal override void ReadGparamConfig(BinaryReaderEx br) => Gparam = new GparamConfig(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ThinkParamID);
                    bw.WriteInt32(NPCParamID);
                    bw.WriteInt32(UnkT10);
                    bw.WriteInt16(0);
                    bw.WriteInt16(ChrManipulatorAllocationParameter);
                    bw.WriteInt32(CharaInitID);
                    bw.WriteInt32(CollisionPartIndex);
                    bw.WriteInt16(UnkT20);
                    bw.WriteInt16(UnkT22);
                    bw.WriteInt32(UnkT24);
                    bw.WritePattern(0x10, 0xFF);
                    bw.WriteInt32(BackupEventAnimID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(EventFlagID);
                    bw.WriteInt32(EventFlagCompareState);
                    bw.WriteInt32(UnkT48);
                    bw.WriteInt32(UnkT4C);
                    bw.WriteInt32(UnkT50);
                    bw.WriteInt32(1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(1);
                    bw.WritePattern(0x18, 0x00);
                    bw.WriteInt32(UnkT78);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteSingle(UnkT84);
                    for (int i = 0; i < 5; i++)
                    {
                        bw.WriteInt32(-1);
                        bw.WriteInt16(-1);
                        bw.WriteInt16(0xA);
                    }
                    bw.WritePattern(0x10, 0x00);
                }

                internal override void WriteGparamConfig(BinaryWriterEx bw) => Gparam.Write(bw);

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionPartName = MSB.FindName(entries.Parts, CollisionPartIndex);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionPartIndex = MSB.FindIndex(entries.Parts, CollisionPartName);
                }
            }

            /// <summary>
            /// References an actual collision and causes another map to be loaded while on it.
            /// </summary>
            public class ConnectCollision : Part
            {
                /// <summary>
                /// PartType.ConnectCollision
                /// </summary>
                public override PartType Type => PartType.ConnectCollision;

                internal override bool HasUnk1 => false;
                internal override bool HasUnk2 => true;
                internal override bool HasGparamConfig => false;
                internal override bool HasUnk6 => false;
                internal override bool HasUnk7 => false;

                /// <summary>
                /// Unknown.
                /// </summary>
                public UnkStruct2 Unk2 { get; set; }

                /// <summary>
                /// The collision part to attach to.
                /// </summary>
                public string CollisionName { get; set; }
                private int CollisionIndex;

                /// <summary>
                /// The map to load when on this collision.
                /// </summary>
                public byte[] MapID { get; private set; }

                /// <summary>
                /// Creates a ConnectCollision with default values.
                /// </summary>
                public ConnectCollision() : base()
                {
                    Unk2 = new UnkStruct2();
                    MapID = new byte[4];
                }

                internal ConnectCollision(BinaryReaderEx br) : base(br)
                {
                    CollisionIndex = br.ReadInt32();
                    MapID = br.ReadBytes(4);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void ReadUnk2(BinaryReaderEx br) => Unk2 = new UnkStruct2(br);

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(CollisionIndex);
                    bw.WriteBytes(MapID);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void WriteUnk2(BinaryWriterEx bw) => Unk2.Write(bw);

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(msb.Parts.Collisions, CollisionIndex);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionIndex = MSB.FindIndex(msb.Parts.Collisions, CollisionName);
                }
            }
        }
    }
}
