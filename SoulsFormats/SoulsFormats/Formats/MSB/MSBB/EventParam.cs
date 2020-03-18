using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBB
    {
        /// <summary>
        /// Events controlling various interactive or dynamic features in the map.
        /// </summary>
        public class EventParam : Section<Event>, IMsbParam<IMsbEvent>
        {
            internal override string Type => "EVENT_PARAM_ST";

            /// <summary>
            /// Sounds in the MSB.
            /// </summary>
            public List<Event.Sound> Sounds;

            /// <summary>
            /// SFX in the MSB.
            /// </summary>
            public List<Event.SFX> SFXs;

            /// <summary>
            /// Treasures in the MSB.
            /// </summary>
            public List<Event.Treasure> Treasures;

            /// <summary>
            /// Generators in the MSB.
            /// </summary>
            public List<Event.Generator> Generators;

            /// <summary>
            /// Messages in the MSB
            /// </summary>
            public List<Event.Message> Messages;

            /// <summary>
            /// Object actions in the MSB.
            /// </summary>
            public List<Event.ObjAct> ObjActs;

            /// <summary>
            /// Spawn points in the MSB.
            /// </summary>
            public List<Event.SpawnPoint> SpawnPoints;

            /// <summary>
            /// Map offsets in the MSB.
            /// </summary>
            public List<Event.MapOffset> MapOffsets;

            /// <summary>
            /// Navimeshes in the MSB.
            /// </summary>
            public List<Event.Navimesh> Navimeshes;

            /// <summary>
            /// Environments in the MSB.
            /// </summary>
            public List<Event.Environment> Environments;

            /// <summary>
            /// Mysteries in the MSB.
            /// </summary>
            public List<Event.Wind> Mysteries;

            /// <summary>
            /// Invasions in the MSB.
            /// </summary>
            public List<Event.Invasion> Invasions;

            /// <summary>
            /// Walk routes in the MSB.
            /// </summary>
            public List<Event.WalkRoute> WalkRoutes;

            /// <summary>
            /// Unknowns in the MSB.
            /// </summary>
            public List<Event.Unknown> Unknowns;

            /// <summary>
            /// Group tours in the MSB.
            /// </summary>
            public List<Event.GroupTour> GroupTours;

            /// <summary>
            /// Multi Summoning Points in the MSB.
            /// </summary>
            public List<Event.MultiSummoningPoint> MultiSummoningPoints;

            /// <summary>
            /// Other events in the MSB.
            /// </summary>
            public List<Event.Other> Others;

            /// <summary>
            /// Creates a new EventSection with no events.
            /// </summary>
            public EventParam(int unk1 = 3) : base(unk1)
            {
                Sounds = new List<Event.Sound>();
                SFXs = new List<Event.SFX>();
                Treasures = new List<Event.Treasure>();
                Generators = new List<Event.Generator>();
                Messages = new List<Event.Message>();
                ObjActs = new List<Event.ObjAct>();
                SpawnPoints = new List<Event.SpawnPoint>();
                MapOffsets = new List<Event.MapOffset>();
                Navimeshes = new List<Event.Navimesh>();
                Environments = new List<Event.Environment>();
                Invasions = new List<Event.Invasion>();
                Mysteries = new List<Event.Wind>();
                WalkRoutes = new List<Event.WalkRoute>();
                Unknowns = new List<Event.Unknown>();
                GroupTours = new List<Event.GroupTour>();
                MultiSummoningPoints = new List<Event.MultiSummoningPoint>();
                Others = new List<Event.Other>();
            }

            /// <summary>
            /// Returns every Event in the order they'll be written.
            /// </summary>
            public override List<Event> GetEntries()
            {
                return SFUtil.ConcatAll<Event>(
                    Sounds, SFXs, Treasures, Generators, Messages, ObjActs, SpawnPoints, MapOffsets, Navimeshes, Environments, Invasions, Mysteries, WalkRoutes, Unknowns, GroupTours, MultiSummoningPoints, Others);
            }
            IReadOnlyList<IMsbEvent> IMsbParam<IMsbEvent>.GetEntries() => GetEntries();

            internal override Event ReadEntry(BinaryReaderEx br)
            {
                EventType type = br.GetEnum32<EventType>(br.Position + 0xC);

                switch (type)
                {
                    case EventType.Sound:
                        var sound = new Event.Sound(br);
                        Sounds.Add(sound);
                        return sound;

                    case EventType.SFX:
                        var sfx = new Event.SFX(br);
                        SFXs.Add(sfx);
                        return sfx;

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
                        var spawnpoint = new Event.SpawnPoint(br);
                        SpawnPoints.Add(spawnpoint);
                        return spawnpoint;

                    case EventType.MapOffset:
                        var mapOffset = new Event.MapOffset(br);
                        MapOffsets.Add(mapOffset);
                        return mapOffset;

                    case EventType.Navimesh:
                        var navimesh = new Event.Navimesh(br);
                        Navimeshes.Add(navimesh);
                        return navimesh;

                    case EventType.Environment:
                        var environment = new Event.Environment(br);
                        Environments.Add(environment);
                        return environment;

                    case EventType.PseudoMultiplayer:
                        var invasion = new Event.Invasion(br);
                        Invasions.Add(invasion);
                        return invasion;

                    case EventType.Wind:
                        var Wind = new Event.Wind(br);
                        Mysteries.Add(Wind);
                        return Wind;

                    case EventType.WalkRoute:
                        var walkRoute = new Event.WalkRoute(br);
                        WalkRoutes.Add(walkRoute);
                        return walkRoute;

                    case EventType.Unknown:
                        var unknown = new Event.Unknown(br);
                        Unknowns.Add(unknown);
                        return unknown;

                    case EventType.GroupTour:
                        var groupTour = new Event.GroupTour(br);
                        GroupTours.Add(groupTour);
                        return groupTour;

                    case EventType.MultiSummoningPoint:
                        var sp = new Event.MultiSummoningPoint(br);
                        MultiSummoningPoints.Add(sp);
                        return sp;

                    case EventType.Other:
                        var other = new Event.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unsupported event type: {type}");
                }
            }

            public void Add(IMsbEvent item)
            {
                switch (item)
                {
                    case Event.Sound e:
                        Sounds.Add(e);
                        break;
                    case Event.SFX e:
                        SFXs.Add(e);
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
                    case Event.Navimesh e:
                        Navimeshes.Add(e);
                        break;
                    case Event.Environment e:
                        Environments.Add(e);
                        break;
                    case Event.Invasion e:
                        Invasions.Add(e);
                        break;
                    case Event.Wind e:
                        Mysteries.Add(e);
                        break;
                    case Event.WalkRoute e:
                        WalkRoutes.Add(e);
                        break;
                    case Event.Unknown e:
                        Unknowns.Add(e);
                        break;
                    case Event.GroupTour e:
                        GroupTours.Add(e);
                        break;
                    case Event.MultiSummoningPoint e:
                        MultiSummoningPoints.Add(e);
                        break;
                    case Event.Other e:
                        Others.Add(e);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Event entry)
            {
                entry.Write(bw, id);
            }
        }

        internal enum EventType : uint
        {
            Light = 0x0,
            Sound = 0x1,
            SFX = 0x2,
            WindSFX = 0x3,
            Treasure = 0x4,
            Generator = 0x5,
            Message = 0x6,
            ObjAct = 0x7,
            SpawnPoint = 0x8,
            MapOffset = 0x9,
            Navimesh = 0xA,
            Environment = 0xB,
            PseudoMultiplayer = 0xC,
            Wind = 0xD,
            WalkRoute = 0xE,
            Unknown = 0xF, // Called "Dark Range" or something in the MSB
            GroupTour = 0x10,
            MultiSummoningPoint = 0x11, // Used only in chalices
            Other = 0xFFFFFFFF,
        }

        /// <summary>
        /// An interactive or dynamic feature of the map.
        /// </summary>
        public abstract class Event : Entry, IMsbEvent
        {
            internal abstract EventType Type { get; }

            /// <summary>
            /// The name of this event.
            /// </summary>
            public override string Name { get; set; }

            public int EventID { get; set; }

            private int partIndex { get; set; }
            /// <summary>
            /// The name of a part the event is attached to.
            /// </summary>
            public string PartName { get; set; }

            private int pointIndex;
            /// <summary>
            /// The name of a region the event is attached to.
            /// </summary>
            public string PointName { get; set; }

            /// <summary>
            /// Used to identify the event in event scripts.
            /// </summary>
            public int EventEntityID { get; set; }

            public int Unk01 { get; set; }

            internal Event() { }

            internal Event(string name)
            {
                Name = name;
                EventID = -1;
                PartName = null;
                PointName = null;
                EventEntityID = -1;
                Unk01 = 0;
            }

            internal Event(Event clone)
            {
                Name = clone.Name;
                EventID = clone.EventID;
                PartName = clone.PartName;
                PointName = clone.PointName;
                EventEntityID = clone.EventEntityID;
                Unk01 = clone.Unk01;
            }

            internal Event(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                EventID = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                br.AssertInt32(0);
                long baseDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);

                br.StepIn(start + baseDataOffset);
                partIndex = br.ReadInt32();
                pointIndex = br.ReadInt32();
                EventEntityID = br.ReadInt32();
                Unk01 = br.ReadInt32();
                br.StepOut();

                br.StepIn(start + typeDataOffset);
                Read(br);
                br.StepOut();
            }

            internal abstract void Read(BinaryReaderEx br);

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(EventID);
                bw.WriteUInt32((uint)Type);
                // Yet another disgusting meme in the name of byte perfect writes
                if (Type == EventType.Other)
                    bw.WriteInt32(id + 1);
                else
                    bw.WriteInt32(id);
                bw.WriteInt32(0);
                bw.ReserveInt64("BaseDataOffset");
                bw.ReserveInt64("TypeDataOffset");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(ReambiguateName(Name), true);
                bw.Pad(8);

                bw.FillInt64("BaseDataOffset", bw.Position - start);
                bw.WriteInt32(partIndex);
                bw.WriteInt32(pointIndex);
                bw.WriteInt32(EventEntityID);
                bw.WriteInt32(Unk01);

                if (Type == EventType.Other)
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
                else
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    WriteSpecific(bw);
                }
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw);

            internal virtual void GetNames(MSBB msb, Entries entries)
            {
                PartName = GetName(entries.Parts, partIndex);
                PointName = GetName(entries.Regions, pointIndex);
            }

            internal virtual void GetIndices(MSBB msb, Entries entries)
            {
                partIndex = GetIndex(entries.Parts, PartName);
                pointIndex = GetIndex(entries.Regions, PointName);
            }

            /// <summary>
            /// Returns the type, ID, and name of this event.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }

            /// <summary>
            /// </summary>
            public class Sound : Event
            {
                /// <summary>
                /// Types of sound that may be in a Sound region.
                /// </summary>
                public enum SndType : uint
                {
                    /// <summary>
                    /// Ambient sounds like wind, creaking, etc.
                    /// </summary>
                    Environment = 0,

                    /// <summary>
                    /// Unknown
                    /// </summary>
                    Unk01 = 1,

                    /// <summary>
                    /// Boss fight music.
                    /// </summary>
                    BGM = 6,

                    /// <summary>
                    /// Character voices.
                    /// </summary>
                    Voice = 7,
                }

                internal override EventType Type => EventType.Sound;

                /// <summary>
                /// Type of sound in this region; determines mixing behavior like muffling.
                /// </summary>
                public SndType SoundType { get; set; }

                /// <summary>
                /// ID of the sound to play in this region, or 0 for child regions.
                /// </summary>
                public int SoundID { get; set; }

                public Sound() { }

                /// <summary>
                /// Creates a new MapOffset with the given ID and name.
                /// </summary>
                public Sound(string name) : base(name)
                {
                    SoundType = SndType.Environment;
                    SoundID = 0;
                }

                /// <summary>
                /// Creates a new Sound with values copied from another.
                /// </summary>
                public Sound(Sound clone) : base(clone)
                {
                    SoundType = clone.SoundType;
                    SoundID = clone.SoundID;
                }

                internal Sound(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    SoundType = br.ReadEnum32<SndType>();
                    SoundID = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteUInt32((uint)SoundType);
                    bw.WriteInt32(SoundID);
                }
            }

            /// <summary>
            /// </summary>
            public class SFX : Event
            {
                internal override EventType Type => EventType.SFX;

                /// <summary>
                /// The ID of the .fxr file to play in this region.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// If true, the effect is off by default until enabled by event scripts.
                /// </summary>
                public bool StartDisabled { get; set; }

                public SFX() { }

                /// <summary>
                /// Creates a new MapOffset with the given ID and name.
                /// </summary>
                public SFX(string name) : base(name)
                {
                    FFXID = -1;
                    StartDisabled = false;
                }

                /// <summary>
                /// Creates a new MapOffset with values copied from another.
                /// </summary>
                public SFX(SFX clone) : base(clone)
                {
                    FFXID = clone.FFXID;
                    StartDisabled = clone.StartDisabled;
                }

                internal SFX(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    FFXID = br.ReadInt32();
                    StartDisabled = br.AssertInt32(0, 1) == 1;
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(FFXID);
                    bw.WriteInt32(StartDisabled ? 1 : 0);
                }
            }

            /// <summary>
            /// A pickuppable item.
            /// </summary>
            public class Treasure : Event
            {
                internal override EventType Type => EventType.Treasure;

                private int partIndex2;
                /// <summary>
                /// The part the treasure is attached to.
                /// </summary>
                public string PartName2 { get; set; }

                /// <summary>
                /// IDs in the item lot param given by this treasure.
                /// </summary>
                public int ItemLot1 { get; set; }
                public int ItemLot2 { get; set; }
                public int ItemLot3 { get; set; }

                // Mostly chalice related
                public int UnkT1C { get; set; }
                public int UnkT20 { get; set; }
                public int UnkT24 { get; set; }
                public int UnkT28 { get; set; }
                public int UnkT2C { get; set; }
                public int UnkT30 { get; set; }
                public int UnkT34 { get; set; }
                public int UnkT38 { get; set; }

                /// <summary>
                /// Animation to play when taking this treasure.
                /// </summary>
                public int PickupAnimID { get; set; }

                /// <summary>
                /// Used for treasures inside chests, exact significance unknown.
                /// </summary>
                public bool InChest { get; set; }

                /// <summary>
                /// Used only for Yoel's ashes treasure; in DS1, used for corpses in barrels.
                /// </summary>
                public bool StartDisabled { get; set; }

                public short UnkT42 { get; set; }
                public int UnkT44 { get; set; }
                public int UnkT48 { get; set; }
                public int UnkT4C { get; set; }

                public Treasure() { }

                /// <summary>
                /// Creates a new Treasure with the given ID and name.
                /// </summary>
                public Treasure(string name) : base(name)
                {
                    PartName2 = null;
                    ItemLot1 = -1;
                    ItemLot2 = -1;
                    ItemLot3 = -1;
                    UnkT1C = -1;
                    UnkT20 = -1;
                    UnkT24 = -1;
                    UnkT28 = -1;
                    UnkT2C = -1;
                    UnkT30 = -1;
                    UnkT34 = -1;
                    UnkT38 = -1;
                    PickupAnimID = -1;
                    InChest = false;
                    StartDisabled = false;
                    UnkT42 = 0;
                    UnkT44 = -1;
                    UnkT48 = -1;
                    UnkT4C = -1;
                }

                /// <summary>
                /// Creates a new Treasure with values copied from another.
                /// </summary>
                public Treasure(Treasure clone) : base(clone)
                {
                    PartName2 = clone.PartName2;
                    ItemLot1 = clone.ItemLot1;
                    ItemLot2 = clone.ItemLot2;
                    ItemLot3 = clone.ItemLot3;
                    UnkT1C = clone.UnkT1C;
                    UnkT20 = clone.UnkT20;
                    UnkT24 = clone.UnkT24;
                    UnkT28 = clone.UnkT28;
                    UnkT2C = clone.UnkT2C;
                    UnkT30 = clone.UnkT30;
                    UnkT34 = clone.UnkT34;
                    UnkT38 = clone.UnkT38;
                    PickupAnimID = clone.PickupAnimID;
                    InChest = clone.InChest;
                    StartDisabled = clone.StartDisabled;
                    UnkT42 = clone.UnkT42;
                    UnkT44 = clone.UnkT44;
                    UnkT48 = clone.UnkT48;
                    UnkT4C = clone.UnkT4C;
                }

                internal Treasure(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    partIndex2 = br.ReadInt32();
                    br.AssertInt32(0);
                    ItemLot1 = br.ReadInt32();
                    ItemLot2 = br.ReadInt32();
                    ItemLot3 = br.ReadInt32();
                    UnkT1C = br.ReadInt32();
                    UnkT20 = br.ReadInt32();
                    UnkT24 = br.ReadInt32();
                    UnkT28 = br.ReadInt32();
                    UnkT2C = br.ReadInt32();
                    UnkT30 = br.ReadInt32();
                    UnkT34 = br.ReadInt32();
                    UnkT38 = br.ReadInt32();
                    PickupAnimID = br.ReadInt32();

                    InChest = br.ReadBoolean();
                    StartDisabled = br.ReadBoolean();
                    UnkT42 = br.ReadInt16();

                    UnkT44 = br.ReadInt32();
                    UnkT48 = br.ReadInt32();
                    UnkT4C = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(partIndex2);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ItemLot1);
                    bw.WriteInt32(ItemLot2);
                    bw.WriteInt32(ItemLot3);
                    bw.WriteInt32(UnkT1C);
                    bw.WriteInt32(UnkT20);
                    bw.WriteInt32(UnkT24);
                    bw.WriteInt32(UnkT28);
                    bw.WriteInt32(UnkT2C);
                    bw.WriteInt32(UnkT30);
                    bw.WriteInt32(UnkT34);
                    bw.WriteInt32(UnkT38);
                    bw.WriteInt32(PickupAnimID);

                    bw.WriteBoolean(InChest);
                    bw.WriteBoolean(StartDisabled);
                    bw.WriteInt16(UnkT42);

                    bw.WriteInt32(UnkT44);
                    bw.WriteInt32(UnkT48);
                    bw.WriteInt32(UnkT4C);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    PartName2 = GetName(entries.Parts, partIndex2);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    partIndex2 = GetIndex(entries.Parts, PartName2);
                }
            }

            /// <summary>
            /// A continuous enemy spawner.
            /// </summary>
            public class Generator : Event
            {
                internal override EventType Type => EventType.Generator;

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

                private int[] spawnPointIndices;
                /// <summary>
                /// Regions that enemies can be spawned at.
                /// </summary>
                public string[] SpawnPointNames { get; private set; }

                private int[] spawnPartIndices;
                /// <summary>
                /// Enemies spawned by this generator.
                /// </summary>
                public string[] SpawnPartNames { get; private set; }


                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }
                public float UnkT18 { get; set; }

                public Generator() { }

                /// <summary>
                /// Creates a new Generator with the given ID and name.
                /// </summary>
                public Generator(string name) : base(name)
                {
                    MaxNum = 0;
                    LimitNum = 0;
                    MinGenNum = 0;
                    MaxGenNum = 0;
                    MinInterval = 0;
                    MaxInterval = 0;
                    UnkT10 = 0;
                    UnkT14 = 0;
                    UnkT18 = 0;
                    SpawnPartNames = new string[32];
                    SpawnPointNames = new string[8];
                }

                /// <summary>
                /// Creates a new Generator with values copied from another.
                /// </summary>
                public Generator(Generator clone) : base(clone)
                {
                    MaxNum = clone.MaxNum;
                    LimitNum = clone.LimitNum;
                    MinGenNum = clone.MinGenNum;
                    MaxGenNum = clone.MaxGenNum;
                    MinInterval = clone.MinInterval;
                    MaxInterval = clone.MaxInterval;
                    UnkT10 = clone.UnkT10;
                    UnkT14 = clone.UnkT14;
                    UnkT18 = clone.UnkT18;
                    SpawnPointNames = (string[])clone.SpawnPointNames.Clone();
                    SpawnPartNames = (string[])clone.SpawnPartNames.Clone();
                }

                internal Generator(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    MaxNum = br.ReadInt16();
                    LimitNum = br.ReadInt16();
                    MinGenNum = br.ReadInt16();
                    MaxGenNum = br.ReadInt16();
                    MinInterval = br.ReadSingle();
                    MaxInterval = br.ReadSingle();
                    UnkT10 = br.ReadInt32();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    spawnPointIndices = br.ReadInt32s(8);
                    spawnPartIndices = br.ReadInt32s(32);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt16(MaxNum);
                    bw.WriteInt16(LimitNum);
                    bw.WriteInt16(MinGenNum);
                    bw.WriteInt16(MaxGenNum);
                    bw.WriteSingle(MinInterval);
                    bw.WriteSingle(MaxInterval);
                    bw.WriteInt32(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteSingle(UnkT18);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(spawnPointIndices);
                    bw.WriteInt32s(spawnPartIndices);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    SpawnPointNames = new string[spawnPointIndices.Length];
                    for (int i = 0; i < spawnPointIndices.Length; i++)
                        SpawnPointNames[i] = GetName(entries.Regions, spawnPointIndices[i]);

                    SpawnPartNames = new string[spawnPartIndices.Length];
                    for (int i = 0; i < spawnPartIndices.Length; i++)
                        SpawnPartNames[i] = GetName(entries.Parts, spawnPartIndices[i]);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    spawnPointIndices = new int[SpawnPointNames.Length];
                    for (int i = 0; i < SpawnPointNames.Length; i++)
                        spawnPointIndices[i] = GetIndex(entries.Regions, SpawnPointNames[i]);

                    spawnPartIndices = new int[SpawnPartNames.Length];
                    for (int i = 0; i < SpawnPartNames.Length; i++)
                        spawnPartIndices[i] = GetIndex(entries.Parts, SpawnPartNames[i]);
                }
            }

            /// <summary>
            /// </summary>
            public class Message : Event
            {
                internal override EventType Type => EventType.Message;

                /// <summary>
                /// ID of the message's text in the FMGs.
                /// </summary>
                public short MessageID { get; set; }

                /// <summary>
                /// Unknown. Always 0 or 2.
                /// </summary>
                public short UnkT02 { get; set; }

                /// <summary>
                /// Whether the message requires Seek Guidance to appear.
                /// </summary>
                public bool Hidden { get; set; }

                public Message() { }

                /// <summary>
                /// Creates a new Message with the given ID and name.
                /// </summary>
                public Message(string name) : base(name)
                {
                    MessageID = -1;
                    UnkT02 = 0;
                    Hidden = false;
                }

                /// <summary>
                /// Creates a new MapOffset with values copied from another.
                /// </summary>
                public Message(Message clone) : base(clone)
                {
                    MessageID = clone.MessageID;
                    UnkT02 = clone.UnkT02;
                    Hidden = clone.Hidden;
                }

                internal Message(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    MessageID = br.ReadInt16();
                    UnkT02 = br.ReadInt16();
                    Hidden = br.AssertInt32(0, 1) == 1;
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt16(MessageID);
                    bw.WriteInt16(UnkT02);
                    bw.WriteInt32(Hidden ? 1 : 0);
                }
            }

            /// <summary>
            /// Controls usable objects like levers.
            /// </summary>
            public class ObjAct : Event
            {
                internal override EventType Type => EventType.ObjAct;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int ObjActEntityID { get; set; }

                private int partIndex2;
                /// <summary>
                /// Unknown.
                /// </summary>
                public string PartName2 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int ParameterID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int EventFlagID { get; set; }

                public ObjAct() { }

                /// <summary>
                /// Creates a new ObjAct with the given ID and name.
                /// </summary>
                public ObjAct(string name) : base(name)
                {
                    ObjActEntityID = -1;
                    PartName2 = null;
                    ParameterID = 0;
                    UnkT10 = 0;
                    EventFlagID = 0;
                }

                /// <summary>
                /// Creates a new ObjAct with values copied from another.
                /// </summary>
                public ObjAct(ObjAct clone) : base(clone)
                {
                    ObjActEntityID = clone.ObjActEntityID;
                    PartName2 = clone.PartName2;
                    ParameterID = clone.ParameterID;
                    UnkT10 = clone.UnkT10;
                    EventFlagID = clone.EventFlagID;
                }

                internal ObjAct(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    ObjActEntityID = br.ReadInt32();
                    partIndex2 = br.ReadInt32();
                    ParameterID = br.ReadInt32();
                    UnkT10 = br.ReadInt32();
                    EventFlagID = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(ObjActEntityID);
                    bw.WriteInt32(partIndex2);
                    bw.WriteInt32(ParameterID);
                    bw.WriteInt32(UnkT10);
                    bw.WriteInt32(EventFlagID);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    if (partIndex2 >= entries.Parts.Count)
                    {
                        // Nightmare of mensis has a case where a part that doesn't exist is referenced
                        PartName2 = null;
                    }
                    else
                    {
                        PartName2 = GetName(entries.Parts, partIndex2);
                    }
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    partIndex2 = GetIndex(entries.Parts, PartName2);
                }
            }

            /// <summary>
            /// </summary>
            public class SpawnPoint : Event
            {
                internal override EventType Type => EventType.SpawnPoint;

                /// <summary>
                /// Spawn region
                /// </summary>
                private int SpawnRegionIndex;
                public string SpawnRegionName { get; set; }

                public SpawnPoint() { }

                /// <summary>
                /// Creates a new Spawn point with the given ID and name.
                /// </summary>
                public SpawnPoint(string name) : base(name)
                {
                    SpawnRegionName = null;
                }

                /// <summary>
                /// Creates a new Spawn point with values copied from another.
                /// </summary>
                public SpawnPoint(SpawnPoint clone) : base(clone)
                {
                    SpawnRegionName = clone.SpawnRegionName;
                }

                internal SpawnPoint(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    SpawnRegionIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(SpawnRegionIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    if (SpawnRegionIndex >= entries.Regions.Count)
                    {
                        SpawnRegionName = null;
                    }
                    else
                    {
                        SpawnRegionName = GetName(entries.Regions, SpawnRegionIndex);
                    }
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    SpawnRegionIndex = GetIndex(entries.Regions, SpawnRegionName);
                }
            }

            /// <summary>
            /// Moves all of the map pieces when cutscenes are played.
            /// </summary>
            public class MapOffset : Event
            {
                internal override EventType Type => EventType.MapOffset;

                /// <summary>
                /// Position of the map offset.
                /// </summary>
                public Vector3 Position { get; set; }

                /// <summary>
                /// Rotation of the map offset.
                /// </summary>
                public float Degree { get; set; }

                public MapOffset() { }

                /// <summary>
                /// Creates a new MapOffset with the given ID and name.
                /// </summary>
                public MapOffset(string name) : base(name)
                {
                    Position = Vector3.Zero;
                    Degree = 0;
                }

                /// <summary>
                /// Creates a new MapOffset with values copied from another.
                /// </summary>
                public MapOffset(MapOffset clone) : base(clone)
                {
                    Position = clone.Position;
                    Degree = clone.Degree;
                }

                internal MapOffset(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    Position = br.ReadVector3();
                    Degree = br.ReadSingle();
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteVector3(Position);
                    bw.WriteSingle(Degree);
                }
            }

            /// <summary>
            /// </summary>
            public class Navimesh : Event
            {
                internal override EventType Type => EventType.Navimesh;

                private int regionIndex;
                /// <summary>
                /// Region for navimesh
                /// </summary>
                public string RegionName { get; set; }

                public Navimesh() { }

                /// <summary>
                /// Creates a new Navimesh with the given ID and name.
                /// </summary>
                public Navimesh(string name) : base(name)
                {
                    regionIndex = -1;
                    RegionName = null;
                }

                /// <summary>
                /// Creates a new Navimesh with values copied from another.
                /// </summary>
                public Navimesh(Navimesh clone) : base(clone)
                {
                    regionIndex = clone.regionIndex;
                    RegionName = clone.RegionName;
                }

                internal Navimesh(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    regionIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(regionIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    RegionName = GetName(entries.Regions, regionIndex);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    regionIndex = (short)GetIndex(entries.Regions, RegionName);
                }
            }

            /// <summary>
            /// </summary>
            public class Environment : Event
            {
                internal override EventType Type => EventType.Environment;

                public int UnkT00 { get; set; }
                public float UnkT04 { get; set; }
                public float UnkT08 { get; set; }
                public float UnkT0C { get; set; }
                public float UnkT10 { get; set; }
                public float UnkT14 { get; set; }

                public Environment() { }

                /// <summary>
                /// Creates a new MapOffset with the given ID and name.
                /// </summary>
                public Environment(string name) : base(name)
                {
                    UnkT00 = 0;
                    UnkT04 = 0.0f;
                    UnkT08 = 0.0f;
                    UnkT0C = 0.0f;
                    UnkT10 = 0.0f;
                    UnkT14 = 0.0f;
                }

                /// <summary>
                /// Creates a new MapOffset with values copied from another.
                /// </summary>
                public Environment(Environment clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                    UnkT04 = clone.UnkT04;
                    UnkT08 = clone.UnkT08;
                    UnkT0C = clone.UnkT0C;
                    UnkT10 = clone.UnkT10;
                    UnkT14 = clone.UnkT14;
                }

                internal Environment(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
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

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
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
            /// Wind
            /// </summary>
            public class Wind : Event
            {
                internal override EventType Type => EventType.Wind;

                /// <summary>
                /// ID of an .fxr file.
                /// </summary>
                public int FFXID { get; set; }

                private int WindAreaIndex;
                /// <summary>
                /// Name of a corresponding WindArea region.
                /// </summary>
                public string WindAreaName { get; set; }

                public float UnkF0C { get; set; }

                public Wind() { }

                /// <summary>
                /// Creates a new Wind with the given ID and name.
                /// </summary>
                public Wind(string name) : base(name)
                {
                    FFXID = -1;
                    WindAreaName = null;
                    UnkF0C = -1.0f;
                }

                /// <summary>
                /// Creates a new Wind with values copied from another.
                /// </summary>
                public Wind(Wind clone) : base(clone)
                {
                    FFXID = clone.FFXID;
                    WindAreaName = clone.WindAreaName;
                    UnkF0C = clone.UnkF0C;
                }

                internal Wind(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    FFXID = br.ReadInt32();
                    WindAreaIndex = br.ReadInt32();
                    UnkF0C = br.ReadSingle();
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(FFXID);
                    bw.WriteInt32(WindAreaIndex);
                    bw.WriteSingle(UnkF0C);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    WindAreaName = GetName(entries.Regions, WindAreaIndex);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    WindAreaIndex = GetIndex(entries.Regions, WindAreaName);
                }
            }

            /// <summary>
            /// A fake multiplayer interaction where the player goes to an NPC's world.
            /// </summary>
            public class Invasion : Event
            {
                internal override EventType Type => EventType.PseudoMultiplayer;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int HostEventEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int InvasionEventEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int InvasionRegionIndex { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int SoundIDMaybe { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int MapEventIDMaybe { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int FlagsMaybe { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT18 { get; set; }

                public Invasion() { }

                /// <summary>
                /// Creates a new Invasion with the given ID and name.
                /// </summary>
                public Invasion(string name) : base(name)
                {
                    HostEventEntityID = -1;
                    InvasionEventEntityID = -1;
                    InvasionRegionIndex = -1;
                    SoundIDMaybe = -1;
                    MapEventIDMaybe = -1;
                    FlagsMaybe = 0;
                    UnkT18 = 0;
                }

                /// <summary>
                /// Creates a new Invasion with values copied from another.
                /// </summary>
                public Invasion(Invasion clone) : base(clone)
                {
                    HostEventEntityID = clone.HostEventEntityID;
                    InvasionEventEntityID = clone.InvasionEventEntityID;
                    InvasionRegionIndex = clone.InvasionRegionIndex;
                    SoundIDMaybe = clone.SoundIDMaybe;
                    MapEventIDMaybe = clone.MapEventIDMaybe;
                    FlagsMaybe = clone.FlagsMaybe;
                    UnkT18 = clone.UnkT18;
                }

                internal Invasion(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    HostEventEntityID = br.ReadInt32();
                    InvasionEventEntityID = br.ReadInt32();
                    InvasionRegionIndex = br.ReadInt32();
                    SoundIDMaybe = br.ReadInt32();
                    MapEventIDMaybe = br.ReadInt32();
                    FlagsMaybe = br.ReadInt32();
                    UnkT18 = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(HostEventEntityID);
                    bw.WriteInt32(InvasionEventEntityID);
                    bw.WriteInt32(InvasionRegionIndex);
                    bw.WriteInt32(SoundIDMaybe);
                    bw.WriteInt32(MapEventIDMaybe);
                    bw.WriteInt32(FlagsMaybe);
                    bw.WriteInt32(UnkT18);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A simple list of points defining a path for enemies to take.
            /// </summary>
            public class WalkRoute : Event
            {
                internal override EventType Type => EventType.WalkRoute;

                /// <summary>
                /// Unknown; probably some kind of route type.
                /// </summary>
                public int UnkT00 { get; set; }

                private short[] walkPointIndices;
                /// <summary>
                /// List of points in the route.
                /// </summary>
                public string[] WalkPointNames { get; private set; }

                public WalkRoute() { }

                /// <summary>
                /// Creates a new WalkRoute with the given ID and name.
                /// </summary>
                public WalkRoute(string name) : base(name)
                {
                    UnkT00 = 0;
                    WalkPointNames = new string[32];
                }

                /// <summary>
                /// Creates a new WalkRoute with values copied from another.
                /// </summary>
                public WalkRoute(WalkRoute clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                    WalkPointNames = (string[])clone.WalkPointNames.Clone();
                }

                internal WalkRoute(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    UnkT00 = br.AssertInt32(0, 1, 2, 5, 6);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    walkPointIndices = br.ReadInt16s(32);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt16s(walkPointIndices);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    WalkPointNames = new string[walkPointIndices.Length];
                    for (int i = 0; i < walkPointIndices.Length; i++)
                        WalkPointNames[i] = GetName(entries.Regions, walkPointIndices[i]);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    walkPointIndices = new short[WalkPointNames.Length];
                    for (int i = 0; i < WalkPointNames.Length; i++)
                        walkPointIndices[i] = (short)GetIndex(entries.Regions, WalkPointNames[i]);
                }
            }

            /// <summary>
            /// Unknown event
            /// </summary>
            public class Unknown : Event
            {
                internal override EventType Type => EventType.Unknown;

                public Unknown() { }

                /// <summary>
                /// Creates a new WalkRoute with the given ID and name.
                /// </summary>
                public Unknown(string name) : base(name)
                {
                }

                /// <summary>
                /// Creates a new WalkRoute with values copied from another.
                /// </summary>
                public Unknown(Unknown clone) : base(clone)
                {
                }

                internal Unknown(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class GroupTour : Event
            {
                internal override EventType Type => EventType.GroupTour;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }
                public int UnkT04 { get; set; }

                private int[] groupPartsIndices;
                /// <summary>
                /// Unknown.
                /// </summary>
                public string[] GroupPartsNames { get; private set; }

                public GroupTour() { }

                /// <summary>
                /// Creates a new GroupTour with the given ID and name.
                /// </summary>
                public GroupTour(string name) : base(name)
                {
                    UnkT00 = 0;
                    UnkT04 = 0;
                    GroupPartsNames = new string[32];
                }

                /// <summary>
                /// Creates a new GroupTour with values copied from another.
                /// </summary>
                public GroupTour(GroupTour clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                    UnkT04 = clone.UnkT04;
                    GroupPartsNames = (string[])clone.GroupPartsNames.Clone();
                }

                internal GroupTour(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    groupPartsIndices = br.ReadInt32s(32);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(groupPartsIndices);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    GroupPartsNames = new string[groupPartsIndices.Length];
                    for (int i = 0; i < groupPartsIndices.Length; i++)
                    {
                        // fatcat
                        if (groupPartsIndices[i] > entries.Parts.Count)
                            GroupPartsNames[i] = null;
                        else
                            GroupPartsNames[i] = GetName(entries.Parts, groupPartsIndices[i]);
                    }
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    groupPartsIndices = new int[GroupPartsNames.Length];
                    for (int i = 0; i < GroupPartsNames.Length; i++)
                        groupPartsIndices[i] = GetIndex(entries.Parts, GroupPartsNames[i]);
                }
            }

            /// <summary>
            /// Unknown event
            /// </summary>
            public class MultiSummoningPoint : Event
            {
                internal override EventType Type => EventType.MultiSummoningPoint;

                public int UnkT00 { get; set; }
                public short UnkT04 { get; set; }
                public short UnkT06 { get; set; }
                public short UnkT08 { get; set; }
                public short UnkT0A { get; set; }

                public MultiSummoningPoint() { }

                /// <summary>
                /// Creates a new WalkRoute with the given ID and name.
                /// </summary>
                public MultiSummoningPoint(string name) : base(name)
                {
                    UnkT00 = 0;
                    UnkT04 = -1;
                    UnkT06 = -1;
                    UnkT08 = -1;
                    UnkT0A = -1;
                }

                /// <summary>
                /// Creates a new WalkRoute with values copied from another.
                /// </summary>
                public MultiSummoningPoint(MultiSummoningPoint clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                    UnkT04 = clone.UnkT04;
                    UnkT06 = clone.UnkT06;
                    UnkT08 = clone.UnkT08;
                    UnkT0A = clone.UnkT0A;
                }

                internal MultiSummoningPoint(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt16();
                    UnkT06 = br.ReadInt16();
                    UnkT08 = br.ReadInt16();
                    UnkT0A = br.ReadInt16();
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt16(UnkT04);
                    bw.WriteInt16(UnkT06);
                    bw.WriteInt16(UnkT08);
                    bw.WriteInt16(UnkT0A);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBB msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                }

                internal override void GetIndices(MSBB msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                }
            }

            /// <summary>
            /// Unknown. Only appears once in one unused MSB so it's hard to draw too many conclusions from it.
            /// </summary>
            public class Other : Event
            {
                internal override EventType Type => EventType.Other;

                public Other() { }

                /// <summary>
                /// Creates a new Other with the given ID and name.
                /// </summary>
                public Other(string name) : base(name)
                {
                }

                /// <summary>
                /// Creates a new Other with values copied from another.
                /// </summary>
                public Other(Other clone) : base(clone)
                {
                }

                internal Other(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                }
            }
        }
    }
}
