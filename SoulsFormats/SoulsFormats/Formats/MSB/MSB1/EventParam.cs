using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB1
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum EventType : uint
        {
            Light = 0,
            Sound = 1,
            SFX = 2,
            WindSFX = 3,
            Treasure = 4,
            Generator = 5,
            Message = 6,
            ObjAct = 7,
            SpawnPoint = 8,
            MapOffset = 9,
            Navmesh = 10,
            Environment = 11,
            PseudoMultiplayer = 12,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Contains abstract entities that control various dynamic elements in the map.
        /// </summary>
        public class EventParam : Param<Event>, IMsbParam<IMsbEvent>
        {
            internal override string Name => "EVENT_PARAM_ST";

            /// <summary>
            /// Fixed point light sources.
            /// </summary>
            public List<Event.Light> Lights { get; set; }

            /// <summary>
            /// Background music and area-based sounds.
            /// </summary>
            public List<Event.Sound> Sounds { get; set; }

            /// <summary>
            /// Particle effects.
            /// </summary>
            public List<Event.SFX> SFXs { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.WindSFX> WindSFXs { get; set; }

            /// <summary>
            /// Item pickups in the open or in chests.
            /// </summary>
            public List<Event.Treasure> Treasures { get; set; }

            /// <summary>
            /// Repeated enemy spawners.
            /// </summary>
            public List<Event.Generator> Generators { get; set; }

            /// <summary>
            /// Static soapstone messages.
            /// </summary>
            public List<Event.Message> Messages { get; set; }

            /// <summary>
            /// Controllers for object interactions.
            /// </summary>
            public List<Event.ObjAct> ObjActs { get; set; }

            /// <summary>
            /// Unknown exactly what this is for.
            /// </summary>
            public List<Event.SpawnPoint> SpawnPoints { get; set; }

            /// <summary>
            /// Represents the origin of the map; already accounted for in MSB positions.
            /// </summary>
            public List<Event.MapOffset> MapOffsets { get; set; }

            /// <summary>
            /// Unknown, interacts with navmeshes somehow.
            /// </summary>
            public List<Event.Navmesh> Navmeshes { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Environment> Environments { get; set; }

            /// <summary>
            /// Controls the player being summoned to an NPC's world.
            /// </summary>
            public List<Event.PseudoMultiplayer> PseudoMultiplayers { get; set; }

            /// <summary>
            /// Creates an empty EventParam.
            /// </summary>
            public EventParam() : base()
            {
                Lights = new List<Event.Light>();
                Sounds = new List<Event.Sound>();
                SFXs = new List<Event.SFX>();
                WindSFXs = new List<Event.WindSFX>();
                Treasures = new List<Event.Treasure>();
                Generators = new List<Event.Generator>();
                Messages = new List<Event.Message>();
                ObjActs = new List<Event.ObjAct>();
                SpawnPoints = new List<Event.SpawnPoint>();
                MapOffsets = new List<Event.MapOffset>();
                Navmeshes = new List<Event.Navmesh>();
                Environments = new List<Event.Environment>();
                PseudoMultiplayers = new List<Event.PseudoMultiplayer>();
            }

            /// <summary>
            /// Returns a list of every event in the order they'll be written.
            /// </summary>
            public override List<Event> GetEntries()
            {
                return SFUtil.ConcatAll<Event>(
                    Lights, Sounds, SFXs, WindSFXs, Treasures,
                    Generators, Messages, ObjActs, SpawnPoints, MapOffsets,
                    Navmeshes, Environments, PseudoMultiplayers);
            }
            IReadOnlyList<IMsbEvent> IMsbParam<IMsbEvent>.GetEntries() => GetEntries();

            internal override Event ReadEntry(BinaryReaderEx br)
            {
                EventType type = br.GetEnum32<EventType>(br.Position + 8);
                switch (type)
                {
                    case EventType.Light:
                        var light = new Event.Light(br);
                        Lights.Add(light);
                        return light;

                    case EventType.Sound:
                        var sound = new Event.Sound(br);
                        Sounds.Add(sound);
                        return sound;

                    case EventType.SFX:
                        var sfx = new Event.SFX(br);
                        SFXs.Add(sfx);
                        return sfx;

                    case EventType.WindSFX:
                        var windSFX = new Event.WindSFX(br);
                        WindSFXs.Add(windSFX);
                        return windSFX;

                    case EventType.Treasure:
                        var treasure = new Event.Treasure(br);
                        Treasures.Add(treasure);
                        return treasure;

                    case EventType.Generator:
                        var generator = new Event.Generator(br);
                        Generators.Add(generator);
                        return generator;

                    case EventType.Message:
                        var message = new Event.Message(br);
                        Messages.Add(message);
                        return message;

                    case EventType.ObjAct:
                        var objAct = new Event.ObjAct(br);
                        ObjActs.Add(objAct);
                        return objAct;

                    case EventType.SpawnPoint:
                        var spawnPoint = new Event.SpawnPoint(br);
                        SpawnPoints.Add(spawnPoint);
                        return spawnPoint;

                    case EventType.MapOffset:
                        var mapOffset = new Event.MapOffset(br);
                        MapOffsets.Add(mapOffset);
                        return mapOffset;

                    case EventType.Navmesh:
                        var navmesh = new Event.Navmesh(br);
                        Navmeshes.Add(navmesh);
                        return navmesh;

                    case EventType.Environment:
                        var environment = new Event.Environment(br);
                        Environments.Add(environment);
                        return environment;

                    case EventType.PseudoMultiplayer:
                        var pseudoMultiplayer = new Event.PseudoMultiplayer(br);
                        PseudoMultiplayers.Add(pseudoMultiplayer);
                        return pseudoMultiplayer;

                    default:
                        throw new NotImplementedException($"Unsupported event type: {type}");
                }
            }

            public void Add(IMsbEvent item)
            {
                switch (item)
                {
                    case Event.Light e:
                        Lights.Add(e);
                        break;
                    case Event.Sound e:
                        Sounds.Add(e);
                        break;
                    case Event.SFX e:
                        SFXs.Add(e);
                        break;
                    case Event.WindSFX e:
                        WindSFXs.Add(e);
                        break;
                    case Event.Treasure e:
                        Treasures.Add(e);
                        break;
                    case Event.Generator e:
                        Generators.Add(e);
                        break;
                    case Event.Message e:
                        Messages.Add(e);
                        break;
                    case Event.ObjAct e:
                        ObjActs.Add(e);
                        break;
                    case Event.SpawnPoint e:
                        SpawnPoints.Add(e);
                        break;
                    case Event.MapOffset e:
                        MapOffsets.Add(e);
                        break;
                    case Event.Navmesh e:
                        Navmeshes.Add(e);
                        break;
                    case Event.Environment e:
                        Environments.Add(e);
                        break;
                    case Event.PseudoMultiplayer e:
                        PseudoMultiplayers.Add(e);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        /// <summary>
        /// Common data for all dynamic events.
        /// </summary>
        public abstract class Event : Entry, IMsbEvent
        {
            /// <summary>
            /// Unknown, should be unique.
            /// </summary>
            public int EventID { get; set; }

            /// <summary>
            /// The type of this event.
            /// </summary>
            public abstract EventType Type { get; }

            /// <summary>
            /// Part referenced by the event.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Part))]
            public string PartName { get; set; }
            private int PartIndex;

            /// <summary>
            /// Region referenced by the event.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Region))]
            public string RegionName { get; set; }
            private int RegionIndex;

            /// <summary>
            /// Identifies the event in external files.
            /// </summary>
            public int EntityID { get; set; }

            internal Event()
            {
                Name = "";
                EventID = -1;
                EntityID = -1;
            }

            internal Event(BinaryReaderEx br)
            {
                long start = br.Position;
                int nameOffset = br.ReadInt32();
                EventID = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                int baseDataOffset = br.ReadInt32();
                int typeDataOffset = br.ReadInt32();
                br.AssertInt32(0);

                Name = br.GetShiftJIS(start + nameOffset);

                br.Position = start + baseDataOffset;
                PartIndex = br.ReadInt32();
                RegionIndex = br.ReadInt32();
                EntityID = br.ReadInt32();
                br.AssertInt32(0);

                br.Position = start + typeDataOffset;
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt32("NameOffset");
                bw.WriteInt32(EventID);
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.ReserveInt32("BaseDataOffset");
                bw.ReserveInt32("TypeDataOffset");
                bw.WriteInt32(0);

                bw.FillInt32("NameOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Name, true);
                bw.Pad(4);

                bw.FillInt32("BaseDataOffset", (int)(bw.Position - start));
                bw.WriteInt32(PartIndex);
                bw.WriteInt32(RegionIndex);
                bw.WriteInt32(EntityID);
                bw.WriteInt32(0);

                bw.FillInt32("TypeDataOffset", (int)(bw.Position - start));
            }

            internal virtual void GetNames(MSB1 msb, Entries entries)
            {
                PartName = MSB.FindName(entries.Parts, PartIndex);
                RegionName = MSB.FindName(entries.Regions, RegionIndex);
            }

            internal virtual void GetIndices(MSB1 msb, Entries entries)
            {
                PartIndex = MSB.FindIndex(entries.Parts, PartName);
                RegionIndex = MSB.FindIndex(entries.Regions, RegionName);
            }

            /// <summary>
            /// Returns the type and name of the event.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {Name}";
            }

            /// <summary>
            /// A fixed point light.
            /// </summary>
            public class Light : Event
            {
                /// <summary>
                /// EventType.Light
                /// </summary>
                public override EventType Type => EventType.Light;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a Light with default values.
                /// </summary>
                public Light() : base() { }

                internal Light(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(UnkT00);
                }
            }

            /// <summary>
            /// An area-based music or sound effect.
            /// </summary>
            public class Sound : Event
            {
                /// <summary>
                /// EventType.Sound
                /// </summary>
                public override EventType Type => EventType.Sound;

                /// <summary>
                /// Category of sound.
                /// </summary>
                public int SoundType { get; set; }

                /// <summary>
                /// ID of the sound file in the FSBs.
                /// </summary>
                public int SoundID { get; set; }

                /// <summary>
                /// Creates a Sound with default values.
                /// </summary>
                public Sound() : base() { }

                internal Sound(BinaryReaderEx br) : base(br)
                {
                    SoundType = br.ReadInt32();
                    SoundID = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(SoundType);
                    bw.WriteInt32(SoundID);
                }
            }

            /// <summary>
            /// A fixed particle effect.
            /// </summary>
            public class SFX : Event
            {
                /// <summary>
                /// EventType.SFX
                /// </summary>
                public override EventType Type => EventType.SFX;

                /// <summary>
                /// ID of the effect in the ffxbnds.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// Creates an SFX with default values.
                /// </summary>
                public SFX() : base() { }

                internal SFX(BinaryReaderEx br) : base(br)
                {
                    FFXID = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(FFXID);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class WindSFX : Event
            {
                /// <summary>
                /// EventType.WindSFX
                /// </summary>
                public override EventType Type => EventType.WindSFX;

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT1C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT20 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT28 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT2C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT30 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT34 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT38 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT3C { get; set; }

                /// <summary>
                /// Creates a WindSFX with default values.
                /// </summary>
                public WindSFX() : base() { }

                internal WindSFX(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadSingle();
                    UnkT04 = br.ReadSingle();
                    UnkT08 = br.ReadSingle();
                    UnkT0C = br.ReadSingle();
                    UnkT10 = br.ReadSingle();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    UnkT1C = br.ReadSingle();
                    UnkT20 = br.ReadSingle();
                    UnkT24 = br.ReadSingle();
                    UnkT28 = br.ReadSingle();
                    UnkT2C = br.ReadSingle();
                    UnkT30 = br.ReadSingle();
                    UnkT34 = br.ReadSingle();
                    UnkT38 = br.ReadSingle();
                    UnkT3C = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteSingle(UnkT00);
                    bw.WriteSingle(UnkT04);
                    bw.WriteSingle(UnkT08);
                    bw.WriteSingle(UnkT0C);
                    bw.WriteSingle(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteSingle(UnkT18);
                    bw.WriteSingle(UnkT1C);
                    bw.WriteSingle(UnkT20);
                    bw.WriteSingle(UnkT24);
                    bw.WriteSingle(UnkT28);
                    bw.WriteSingle(UnkT2C);
                    bw.WriteSingle(UnkT30);
                    bw.WriteSingle(UnkT34);
                    bw.WriteSingle(UnkT38);
                    bw.WriteSingle(UnkT3C);
                }
            }

            /// <summary>
            /// A pick-uppable item.
            /// </summary>
            public class Treasure : Event
            {
                /// <summary>
                /// EventType.Treasure
                /// </summary>
                public override EventType Type => EventType.Treasure;

                /// <summary>
                /// The part that the treasure is attached to, such as an item corpse.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string TreasurePartName { get; set; }
                private int TreasurePartIndex;

                /// <summary>
                /// Five ItemLotParam IDs.
                /// </summary>
                [MSBParamReference(ParamName = "ItemLotParam")]
                public int[] ItemLots { get; private set; }

                /// <summary>
                /// Whether the treasure is inside a container.
                /// </summary>
                public bool InChest { get; set; }

                /// <summary>
                /// Whether the treasure should be initially hidden, used for items in breakable objects.
                /// </summary>
                public bool StartDisabled { get; set; }

                /// <summary>
                /// Creates a Treasure with default values.
                /// </summary>
                public Treasure() : base()
                {
                    ItemLots = new int[5] { -1, -1, -1, -1, -1 };
                }

                internal Treasure(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    TreasurePartIndex = br.ReadInt32();
                    ItemLots = new int[5];
                    for (int i = 0; i < 5; i++)
                    {
                        ItemLots[i] = br.ReadInt32();
                        br.AssertInt32(-1);
                    }
                    InChest = br.ReadBoolean();
                    StartDisabled = br.ReadBoolean();
                    br.AssertInt16(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(0);
                    bw.WriteInt32(TreasurePartIndex);
                    for (int i = 0; i < 5; i++)
                    {
                        bw.WriteInt32(ItemLots[i]);
                        bw.WriteInt32(-1);
                    }
                    bw.WriteBoolean(InChest);
                    bw.WriteBoolean(StartDisabled);
                    bw.WriteInt16(0);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    TreasurePartName = MSB.FindName(entries.Parts, TreasurePartIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    TreasurePartIndex = MSB.FindIndex(entries.Parts, TreasurePartName);
                }
            }

            /// <summary>
            /// A repeating enemy spawner.
            /// </summary>
            public class Generator : Event
            {
                /// <summary>
                /// EventType.Generator
                /// </summary>
                public override EventType Type => EventType.Generator;

                /// <summary>
                /// Unknown.
                /// </summary>
                public short MaxNum { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short LimitNum { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short MinGenNum { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short MaxGenNum { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float MinInterval { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float MaxInterval { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int InitialSpawnCount { get; set; }

                /// <summary>
                /// Points that enemies may be spawned at.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string[] SpawnPointNames { get; private set; }
                private int[] SpawnPointIndices;

                /// <summary>
                /// Enemies to be respawned.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string[] SpawnPartNames { get; private set; }
                private int[] SpawnPartIndices;

                /// <summary>
                /// Creates a Generator with default values.
                /// </summary>
                public Generator() : base()
                {
                    SpawnPointNames = new string[4];
                    SpawnPartNames = new string[32];
                }

                internal Generator(BinaryReaderEx br) : base(br)
                {
                    MaxNum = br.ReadInt16();
                    LimitNum = br.ReadInt16();
                    MinGenNum = br.ReadInt16();
                    MaxGenNum = br.ReadInt16();
                    MinInterval = br.ReadSingle();
                    MaxInterval = br.ReadSingle();
                    InitialSpawnCount = br.ReadInt32();
                    br.AssertPattern(0x1C, 0x00);
                    SpawnPointIndices = br.ReadInt32s(4);
                    SpawnPartIndices = br.ReadInt32s(32);
                    br.AssertPattern(0x40, 0x00);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt16(MaxNum);
                    bw.WriteInt16(LimitNum);
                    bw.WriteInt16(MinGenNum);
                    bw.WriteInt16(MaxGenNum);
                    bw.WriteSingle(MinInterval);
                    bw.WriteSingle(MaxInterval);
                    bw.WriteInt32(InitialSpawnCount);
                    bw.WritePattern(0x1C, 0x00);
                    bw.WriteInt32s(SpawnPointIndices);
                    bw.WriteInt32s(SpawnPartIndices);
                    bw.WritePattern(0x40, 0x00);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    SpawnPointNames = MSB.FindNames(entries.Regions, SpawnPointIndices);
                    SpawnPartNames = MSB.FindNames(entries.Parts, SpawnPartIndices);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    SpawnPointIndices = MSB.FindIndices(entries.Regions, SpawnPointNames);
                    SpawnPartIndices = MSB.FindIndices(entries.Parts, SpawnPartNames);
                }
            }

            /// <summary>
            /// A fixed orange soapstone message.
            /// </summary>
            public class Message : Event
            {
                /// <summary>
                /// EventType.Message
                /// </summary>
                public override EventType Type => EventType.Message;

                /// <summary>
                /// FMG text ID to display.
                /// </summary>
                public short MessageID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT02 { get; set; }

                /// <summary>
                /// Whether the Message requires Seek Guidance to see.
                /// </summary>
                public bool Hidden { get; set; }

                /// <summary>
                /// Creates a Message with default values.
                /// </summary>
                public Message() : base() { }

                internal Message(BinaryReaderEx br) : base(br)
                {
                    MessageID = br.ReadInt16();
                    UnkT02 = br.ReadInt16();
                    Hidden = br.ReadBoolean();
                    br.AssertByte(0);
                    br.AssertInt16(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt16(MessageID);
                    bw.WriteInt16(UnkT02);
                    bw.WriteBoolean(Hidden);
                    bw.WriteByte(0);
                    bw.WriteInt16(0);
                }
            }

            /// <summary>
            /// Represents an interaction with an object.
            /// </summary>
            public class ObjAct : Event
            {
                /// <summary>
                /// EventType.ObjAct
                /// </summary>
                public override EventType Type => EventType.ObjAct;

                /// <summary>
                /// Unknown how this differs from the Event EntityID.
                /// </summary>
                public int ObjActEntityID { get; set; }

                /// <summary>
                /// The object that the ObjAct controls.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string ObjActPartName { get; set; }
                private int ObjActPartIndex;

                /// <summary>
                /// ID in ObjActParam that configures the ObjAct.
                /// </summary>
                [MSBParamReference(ParamName = "ObjActParam")]
                public short ObjActParamID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT0A { get; set; }

                /// <summary>
                /// Unknown, probably enables or disables the ObjAct.
                /// </summary>
                public int EventFlagID { get; set; }

                /// <summary>
                /// Creates an ObjAct with default values.
                /// </summary>
                public ObjAct() : base()
                {
                    ObjActEntityID = -1;
                    ObjActParamID = -1;
                    EventFlagID = -1;
                }

                internal ObjAct(BinaryReaderEx br) : base(br)
                {
                    ObjActEntityID = br.ReadInt32();
                    ObjActPartIndex = br.ReadInt32();
                    ObjActParamID = br.ReadInt16();
                    UnkT0A = br.ReadInt16();
                    EventFlagID = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(ObjActEntityID);
                    bw.WriteInt32(ObjActPartIndex);
                    bw.WriteInt16(ObjActParamID);
                    bw.WriteInt16(UnkT0A);
                    bw.WriteInt32(EventFlagID);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    ObjActPartName = MSB.FindName(entries.Parts, ObjActPartIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    ObjActPartIndex = MSB.FindIndex(entries.Parts, ObjActPartName);
                }
            }

            /// <summary>
            /// Unknown what this accomplishes beyond just having the region.
            /// </summary>
            public class SpawnPoint : Event
            {
                /// <summary>
                /// EventType.SpawnPoint
                /// </summary>
                public override EventType Type => EventType.SpawnPoint;

                /// <summary>
                /// Point for the SpawnPoint to spawn at.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string SpawnPointName { get; set; }
                private int SpawnPointIndex;

                /// <summary>
                /// Creates a SpawnPoint with default values.
                /// </summary>
                public SpawnPoint() : base() { }

                internal SpawnPoint(BinaryReaderEx br) : base(br)
                {
                    SpawnPointIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(SpawnPointIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    SpawnPointName = MSB.FindName(entries.Regions, SpawnPointIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    SpawnPointIndex = MSB.FindIndex(entries.Regions, SpawnPointName);
                }
            }

            /// <summary>
            /// The origin of the map, already accounted for in MSB positions.
            /// </summary>
            public class MapOffset : Event
            {
                /// <summary>
                /// EventType.MapOffset
                /// </summary>
                public override EventType Type => EventType.MapOffset;

                /// <summary>
                /// Position of the map.
                /// </summary>
                public Vector3 Position { get; set; }

                /// <summary>
                /// Rotation of the map.
                /// </summary>
                public float Degree { get; set; }

                /// <summary>
                /// Creates a MapOffset with default values.
                /// </summary>
                public MapOffset() : base() { }

                internal MapOffset(BinaryReaderEx br) : base(br)
                {
                    Position = br.ReadVector3();
                    Degree = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteVector3(Position);
                    bw.WriteSingle(Degree);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Navmesh : Event
            {
                /// <summary>
                /// EventType.Navmesh
                /// </summary>
                public override EventType Type => EventType.Navmesh;

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string NavmeshRegionName { get; set; }
                private int NavmeshRegionIndex;

                /// <summary>
                /// Creates a Navmesh with default values.
                /// </summary>
                public Navmesh() : base() { }

                internal Navmesh(BinaryReaderEx br) : base(br)
                {
                    NavmeshRegionIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(NavmeshRegionIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB1 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    NavmeshRegionName = MSB.FindName(entries.Regions, NavmeshRegionIndex);
                }

                internal override void GetIndices(MSB1 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    NavmeshRegionIndex = MSB.FindIndex(entries.Regions, NavmeshRegionName);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Environment : Event
            {
                /// <summary>
                /// EventType.Environment
                /// </summary>
                public override EventType Type => EventType.Environment;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }

                /// <summary>
                /// Creates an Environment with default values.
                /// </summary>
                public Environment() : base() { }

                internal Environment(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadSingle();
                    UnkT08 = br.ReadSingle();
                    UnkT0C = br.ReadSingle();
                    UnkT10 = br.ReadSingle();
                    UnkT14 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(UnkT00);
                    bw.WriteSingle(UnkT04);
                    bw.WriteSingle(UnkT08);
                    bw.WriteSingle(UnkT0C);
                    bw.WriteSingle(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A fake multiplayer session where you enter an NPC's world.
            /// </summary>
            public class PseudoMultiplayer : Event
            {
                /// <summary>
                /// EventType.PseudoMultiplayer
                /// </summary>
                public override EventType Type => EventType.PseudoMultiplayer;

                /// <summary>
                /// The NPC whose world you're entering.
                /// </summary>
                public int HostEntityID { get; set; }

                /// <summary>
                /// Set when inside the event's region, unset when outside it.
                /// </summary>
                public int EventFlagID { get; set; }

                /// <summary>
                /// ID of a goods item that is used to trigger the event.
                /// </summary>
                public int ActivateGoodsID { get; set; }

                /// <summary>
                /// Creates a PseudoMultiplayer with default values.
                /// </summary>
                public PseudoMultiplayer() : base()
                {
                    HostEntityID = -1;
                    EventFlagID = -1;
                }

                internal PseudoMultiplayer(BinaryReaderEx br) : base(br)
                {
                    HostEntityID = br.ReadInt32();
                    EventFlagID = br.ReadInt32();
                    ActivateGoodsID = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int id)
                {
                    base.Write(bw, id);
                    bw.WriteInt32(HostEntityID);
                    bw.WriteInt32(EventFlagID);
                    bw.WriteInt32(ActivateGoodsID);
                    bw.WriteInt32(0);
                }
            }
        }
    }
}
