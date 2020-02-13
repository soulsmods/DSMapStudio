using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB1
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum PartType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Player = 4,
            Collision = 5,
            Navmesh = 8,
            DummyObject = 9,
            DummyEnemy = 10,
            ConnectCollision = 11,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// All instances of concrete things in the map.
        /// </summary>
        public class PartsParam : Param<Part>, IMsbParam<IMsbPart>
        {
            internal override string Name => "PARTS_PARAM_ST";

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
            /// AI navigation meshes.
            /// </summary>
            public List<Part.Navmesh> Navmeshes { get; set; }

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
            /// Creates an empty PartsParam.
            /// </summary>
            public PartsParam() : base()
            {
                MapPieces = new List<Part.MapPiece>();
                Objects = new List<Part.Object>();
                Enemies = new List<Part.Enemy>();
                Players = new List<Part.Player>();
                Collisions = new List<Part.Collision>();
                Navmeshes = new List<Part.Navmesh>();
                DummyObjects = new List<Part.DummyObject>();
                DummyEnemies = new List<Part.DummyEnemy>();
                ConnectCollisions = new List<Part.ConnectCollision>();
            }

            internal override Part ReadEntry(BinaryReaderEx br)
            {
                PartType type = br.GetEnum32<PartType>(br.Position + 4);
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

                    case PartType.Navmesh:
                        var navmesh = new Part.Navmesh(br);
                        Navmeshes.Add(navmesh);
                        return navmesh;

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
                    Navmeshes, DummyObjects, DummyEnemies, ConnectCollisions);
            }
            IReadOnlyList<IMsbPart> IMsbParam<IMsbPart>.GetEntries() => GetEntries();

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
                    case Part.Navmesh m:
                        Navmeshes.Add(m);
                        break;
                    case Part.ConnectCollision m:
                        ConnectCollisions.Add(m);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        /// <summary>
        /// Common information for all concrete entities.
        /// </summary>
        public abstract class Part : Entry, IMsbPart
        {
            /// <summary>
            /// The type of the Part.
            /// </summary>
            public abstract PartType Type { get; }

            /// <summary>
            /// The model of the Part, corresponding to an entry in the ModelParam.
            /// </summary>
            public string ModelName { get; set; }
            private int ModelIndex;

            /// <summary>
            /// A path to a .sib file, presumed to be some kind of editor placeholder.
            /// </summary>
            public string Placeholder { get; set; }

            /// <summary>
            /// Location of the part.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the part, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Scale of the part, only meaningful for map pieces and objects.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// Controls when the part is visible.
            /// </summary>
            public uint[] DrawGroups { get; private set; }

            /// <summary>
            /// Controls when the part is visible.
            /// </summary>
            public uint[] DispGroups { get; private set; }

            /// <summary>
            /// Identifies the part in external files.
            /// </summary>
            public int EntityID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte LightID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte FogID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte ScatterID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte LensFlareID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte ShadowID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte DofID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte ToneMapID { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte ToneCorrectID { get; set; }

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
            public byte IsShadowSrc { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte IsShadowDest { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte IsShadowOnly { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte DrawByReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte DrawOnlyReflectCam { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte UseDepthBiasFloat { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte DisablePointLightEffect { get; set; }

            internal Part()
            {
                Name = "";
                Placeholder = "";
                Scale = Vector3.One;
                DrawGroups = new uint[4] {
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                DispGroups = new uint[4] {
                    0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                EntityID = -1;
            }

            internal Part(BinaryReaderEx br)
            {
                long start = br.Position;
                int nameOffset = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                ModelIndex = br.ReadInt32();
                int sibOffset = br.ReadInt32();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();
                DrawGroups = br.ReadUInt32s(4);
                DispGroups = br.ReadUInt32s(4);
                int entityDataOffset = br.ReadInt32();
                int typeDataOffset = br.ReadInt32();
                br.AssertInt32(0);

                Name = br.GetShiftJIS(start + nameOffset);
                Placeholder = br.GetShiftJIS(start + sibOffset);

                br.Position = start + entityDataOffset;
                EntityID = br.ReadInt32();
                LightID = br.ReadByte();
                FogID = br.ReadByte();
                ScatterID = br.ReadByte();
                LensFlareID = br.ReadByte();
                ShadowID = br.ReadByte();
                DofID = br.ReadByte();
                ToneMapID = br.ReadByte();
                ToneCorrectID = br.ReadByte();
                LanternID = br.ReadByte();
                LodParamID = br.ReadByte();
                br.AssertByte(0);
                IsShadowSrc = br.ReadByte();
                IsShadowDest = br.ReadByte();
                IsShadowOnly = br.ReadByte();
                DrawByReflectCam = br.ReadByte();
                DrawOnlyReflectCam = br.ReadByte();
                UseDepthBiasFloat = br.ReadByte();
                DisablePointLightEffect = br.ReadByte();
                br.AssertByte(0);
                br.AssertByte(0);

                br.Position = start + typeDataOffset;
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt32("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteInt32(ModelIndex);
                bw.ReserveInt32("SibOffset");
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);
                bw.WriteUInt32s(DrawGroups);
                bw.WriteUInt32s(DispGroups);
                bw.ReserveInt32("EntityDataOffset");
                bw.ReserveInt32("TypeDataOffset");
                bw.WriteInt32(0);

                long stringsStart = bw.Position;
                bw.FillInt32("NameOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(MSB.ReambiguateName(Name), true);
                bw.FillInt32("SibOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Placeholder, true);
                bw.Pad(4);
                if (bw.Position - stringsStart < 0x14)
                    bw.WritePattern((int)(0x14 - (bw.Position - stringsStart)), 0x00);

                bw.FillInt32("EntityDataOffset", (int)(bw.Position - start));
                bw.WriteInt32(EntityID);
                bw.WriteByte(LightID);
                bw.WriteByte(FogID);
                bw.WriteByte(ScatterID);
                bw.WriteByte(LensFlareID);
                bw.WriteByte(ShadowID);
                bw.WriteByte(DofID);
                bw.WriteByte(ToneMapID);
                bw.WriteByte(ToneCorrectID);
                bw.WriteByte(LanternID);
                bw.WriteByte(LodParamID);
                bw.WriteByte(0);
                bw.WriteByte(IsShadowSrc);
                bw.WriteByte(IsShadowDest);
                bw.WriteByte(IsShadowOnly);
                bw.WriteByte(DrawByReflectCam);
                bw.WriteByte(DrawOnlyReflectCam);
                bw.WriteByte(UseDepthBiasFloat);
                bw.WriteByte(DisablePointLightEffect);
                bw.WriteByte(0);
                bw.WriteByte(0);

                bw.FillInt32("TypeDataOffset", (int)(bw.Position - start));
            }

            internal virtual void GetNames(MSB1 msb, Entries entries)
            {
                ModelName = MSB.FindName(entries.Models, ModelIndex);
            }

            internal virtual void GetIndices(MSB1 msb, Entries entries)
            {
                ModelIndex = MSB.FindIndex(entries.Models, ModelName);
            }

            /// <summary>
            /// A visible but not physical model making up the map.
            /// </summary>
            public class MapPiece : Part
            {
                /// <summary>
                /// PartType.MapPiece
                /// </summary>
                public override PartType Type => PartType.MapPiece;

                /// <summary>
                /// Creates a MapPiece with default values.
                /// </summary>
                public MapPiece() : base() { }

                internal MapPiece(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A dynamic or interactible part of the map.
            /// </summary>
            public class Object : Part
            {
                /// <summary>
                /// PartType.Object
                /// </summary>
                public override PartType Type => PartType.Object;

                /// <summary>
                /// Collision that controls loading of the object.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Collision))]
                public string CollisionName { get; set; }
                private int CollisionIndex;

                /// <summary>
                /// Unknown.
                /// </summary>
                [Order]
                public int UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [Order]
                public short UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [Order]
                public short UnkT0E { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [Order]
                public int UnkT10 { get; set; }

                /// <summary>
                /// Creates an Object with default values.
                /// </summary>
                public Object() : base() { }

                internal Object(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    CollisionIndex = br.ReadInt32();
                    UnkT08 = br.ReadInt32();
                    UnkT0C = br.ReadInt16();
                    UnkT0E = br.ReadInt16();
                    UnkT10 = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(0);
                    bw.WriteInt32(CollisionIndex);
                    bw.WriteInt32(UnkT08);
                    bw.WriteInt16(UnkT0C);
                    bw.WriteInt16(UnkT0E);
                    bw.WriteInt32(UnkT10);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(entries.Parts, CollisionIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionIndex = MSB.FindIndex(entries.Parts, CollisionName);
                }
            }

            /// <summary>
            /// Any living entity besides the player character.
            /// </summary>
            public class Enemy : Part
            {
                /// <summary>
                /// PartType.Enemy
                /// </summary>
                public override PartType Type => PartType.Enemy;

                /// <summary>
                /// ID in NPCThinkParam determining AI properties.
                /// </summary>
                [MSBParamReference(ParamName = "NpcThinkParam")]
                public int ThinkParamID { get; set; }

                /// <summary>
                /// ID in NPCParam determining character properties.
                /// </summary>
                [MSBParamReference(ParamName = "NpcParam")]
                public int NPCParamID { get; set; }

                /// <summary>
                /// ID of a talk ESD used by the character.
                /// </summary>
                public int TalkID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }

                /// <summary>
                /// ID in CharaInitParam determining equipment and stats for humans.
                /// </summary>
                [MSBParamReference(ParamName = "CharaInitParam")]
                public int CharaInitID { get; set; }

                /// <summary>
                /// Collision that controls loading of the enemy.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Collision))]
                public string CollisionName { get; set; }
                private int CollisionIndex;

                /// <summary>
                /// Regions for the enemy to patrol.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string[] MovePointNames { get; private set; }
                private short[] MovePointIndices;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT38 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT3C { get; set; }

                /// <summary>
                /// Creates an Enemy with default values.
                /// </summary>
                public Enemy() : base()
                {
                    ThinkParamID = -1;
                    NPCParamID = -1;
                    TalkID = -1;
                    CharaInitID = -1;
                    MovePointNames = new string[8];
                }

                internal Enemy(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    ThinkParamID = br.ReadInt32();
                    NPCParamID = br.ReadInt32();
                    TalkID = br.ReadInt32();
                    UnkT14 = br.ReadSingle();
                    CharaInitID = br.ReadInt32();
                    CollisionIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    MovePointIndices = br.ReadInt16s(8);
                    UnkT38 = br.ReadInt32();
                    UnkT3C = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ThinkParamID);
                    bw.WriteInt32(NPCParamID);
                    bw.WriteInt32(TalkID);
                    bw.WriteSingle(UnkT14);
                    bw.WriteInt32(CharaInitID);
                    bw.WriteInt32(CollisionIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt16s(MovePointIndices);
                    bw.WriteInt32(UnkT38);
                    bw.WriteInt32(UnkT3C);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(entries.Parts, CollisionIndex);

                    MovePointNames = new string[MovePointIndices.Length];
                    for (int i = 0; i < MovePointIndices.Length; i++)
                        MovePointNames[i] = MSB.FindName(entries.Regions, MovePointIndices[i]);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionIndex = MSB.FindIndex(entries.Parts, CollisionName);

                    MovePointIndices = new short[MovePointNames.Length];
                    for (int i = 0; i < MovePointNames.Length; i++)
                        MovePointIndices[i] = (short)MSB.FindIndex(entries.Regions, MovePointNames[i]);
                }
            }

            /// <summary>
            /// Unknown exactly what these do.
            /// </summary>
            public class Player : Part
            {
                /// <summary>
                /// PartType.Player
                /// </summary>
                public override PartType Type => PartType.Player;

                /// <summary>
                /// Creates a Player with default values.
                /// </summary>
                public Player() : base() { }

                internal Player(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
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

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte HitFilterID { get; set; }

                /// <summary>
                /// Causes sounds to be modulated when standing on the collision.
                /// </summary>
                public byte SoundSpaceType { get; set; }

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
                public uint[] NvmGroups { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int[] VagrantEntityIDs { get; private set; }

                /// <summary>
                /// Controls displays of the map name on screen or the loading menu.
                /// </summary>
                public short MapNameID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short DisableStart { get; set; }

                /// <summary>
                /// If set, disables a bonfire when any enemy is on the collision.
                /// </summary>
                public int DisableBonfireEntityID { get; set; }

                /// <summary>
                /// An ID used for multiplayer eligibility.
                /// </summary>
                public int PlayRegionID { get; set; }

                /// <summary>
                /// ID in LockCamParam determining camera properties.
                /// </summary>
                [MSBParamReference(ParamName = "LockCamParam")]
                public short LockCamParamID1 { get; set; }

                /// <summary>
                /// ID in LockCamParam determining camera properties.
                /// </summary>
                [MSBParamReference(ParamName = "LockCamParam")]
                public short LockCamParamID2 { get; set; }

                /// <summary>
                /// Creates a Collision with default values.
                /// </summary>
                public Collision() : base()
                {
                    NvmGroups = new uint[4]{
                        0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                    VagrantEntityIDs = new int[3] { -1, -1, -1 };
                    MapNameID = -1;
                    DisableBonfireEntityID = -1;
                    LockCamParamID1 = -1;
                    LockCamParamID2 = -1;
                }

                internal Collision(BinaryReaderEx br) : base(br)
                {
                    HitFilterID = br.ReadByte();
                    SoundSpaceType = br.ReadByte();
                    EnvLightMapSpotIndex = br.ReadInt16();
                    ReflectPlaneHeight = br.ReadSingle();
                    NvmGroups = br.ReadUInt32s(4);
                    VagrantEntityIDs = br.ReadInt32s(3);
                    MapNameID = br.ReadInt16();
                    DisableStart = br.ReadInt16();
                    DisableBonfireEntityID = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    PlayRegionID = br.ReadInt32();
                    LockCamParamID1 = br.ReadInt16();
                    LockCamParamID2 = br.ReadInt16();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteByte(HitFilterID);
                    bw.WriteByte(SoundSpaceType);
                    bw.WriteInt16(EnvLightMapSpotIndex);
                    bw.WriteSingle(ReflectPlaneHeight);
                    bw.WriteUInt32s(NvmGroups);
                    bw.WriteInt32s(VagrantEntityIDs);
                    bw.WriteInt16(MapNameID);
                    bw.WriteInt16(DisableStart);
                    bw.WriteInt32(DisableBonfireEntityID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(PlayRegionID);
                    bw.WriteInt16(LockCamParamID1);
                    bw.WriteInt16(LockCamParamID2);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// An AI navigation mesh.
            /// </summary>
            public class Navmesh : Part
            {
                /// <summary>
                /// PartType.Navmesh
                /// </summary>
                public override PartType Type => PartType.Navmesh;

                /// <summary>
                /// Unknown.
                /// </summary>
                public uint[] NvmGroups { get; private set; }

                /// <summary>
                /// Creates a Navmesh with default values.
                /// </summary>
                public Navmesh() : base()
                {
                    NvmGroups = new uint[4] {
                        0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF };
                }

                internal Navmesh(BinaryReaderEx br) : base(br)
                {
                    NvmGroups = br.ReadUInt32s(4);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteUInt32s(NvmGroups);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A normally invisible object, either unused or for a cutscene.
            /// </summary>
            public class DummyObject : Object
            {
                /// <summary>
                /// PartType.DummyObject
                /// </summary>
                public override PartType Type => PartType.DummyObject;

                /// <summary>
                /// Creates a DummyObject with default values.
                /// </summary>
                public DummyObject() : base() { }

                internal DummyObject(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A normally invisible enemy, either unused or for a cutscene.
            /// </summary>
            public class DummyEnemy : Enemy
            {
                /// <summary>
                /// PartType.DummyEnemy
                /// </summary>
                public override PartType Type => PartType.DummyEnemy;

                /// <summary>
                /// Creates a DummyEnemy with default values.
                /// </summary>
                public DummyEnemy() : base() { }

                internal DummyEnemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Attaches to an actual Collision and causes another map to be loaded when standing on it.
            /// </summary>
            public class ConnectCollision : Part
            {
                /// <summary>
                /// PartType.ConnectCollision
                /// </summary>
                public override PartType Type => PartType.ConnectCollision;

                /// <summary>
                /// The collision which will load another map.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Collision))]
                public string CollisionName { get; set; }
                private int CollisionIndex;

                /// <summary>
                /// Four bytes specifying the map ID to load.
                /// </summary>
                public byte[] MapID { get; private set; }

                /// <summary>
                /// Creates a ConnectCollision with default values.
                /// </summary>
                public ConnectCollision() : base()
                {
                    MapID = new byte[4] { 10, 2, 0, 0 };
                }

                internal ConnectCollision(BinaryReaderEx br) : base(br)
                {
                    CollisionIndex = br.ReadInt32();
                    MapID = br.ReadBytes(4);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(CollisionIndex);
                    bw.WriteBytes(MapID);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    CollisionName = MSB.FindName(msb.Parts.Collisions, CollisionIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    CollisionIndex = MSB.FindIndex(msb.Parts.Collisions, CollisionName);
                }
            }
        }
    }
}
