using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// Events controlling various interactive or dynamic features in the map.
        /// </summary>
        public class EventParam : Param<Event>, IMsbParam<IMsbEvent>
        {
            internal override string Type => "EVENT_PARAM_ST";

            /// <summary>
            /// Treasures in the MSB.
            /// </summary>
            public List<Event.Treasure> Treasures;

            /// <summary>
            /// Generators in the MSB.
            /// </summary>
            public List<Event.Generator> Generators;

            /// <summary>
            /// Object actions in the MSB.
            /// </summary>
            public List<Event.ObjAct> ObjActs;

            /// <summary>
            /// Map offsets in the MSB.
            /// </summary>
            public List<Event.MapOffset> MapOffsets;

            /// <summary>
            /// Pseudo multiplayer events in the MSB.
            /// </summary>
            public List<Event.PseudoMultiplayer> PseudoMultiplayers;

            /// <summary>
            /// Walk routes in the MSB.
            /// </summary>
            public List<Event.WalkRoute> WalkRoutes;

            /// <summary>
            /// Group tours in the MSB.
            /// </summary>
            public List<Event.GroupTour> GroupTours;

            /// <summary>
            /// Other events in the MSB.
            /// </summary>
            public List<Event.Other> Others;

            /// <summary>
            /// Creates a new EventParam with no events.
            /// </summary>
            public EventParam(int unk1 = 3) : base(unk1)
            {
                Treasures = new List<Event.Treasure>();
                Generators = new List<Event.Generator>();
                ObjActs = new List<Event.ObjAct>();
                MapOffsets = new List<Event.MapOffset>();
                PseudoMultiplayers = new List<Event.PseudoMultiplayer>();
                WalkRoutes = new List<Event.WalkRoute>();
                GroupTours = new List<Event.GroupTour>();
                Others = new List<Event.Other>();
            }

            /// <summary>
            /// Returns every Event in the order they'll be written.
            /// </summary>
            public override List<Event> GetEntries()
            {
                return SFUtil.ConcatAll<Event>(
                    Treasures, Generators, ObjActs, MapOffsets, PseudoMultiplayers, WalkRoutes, GroupTours, Others);
            }
            IReadOnlyList<IMsbEvent> IMsbParam<IMsbEvent>.GetEntries() => GetEntries();

            internal override Event ReadEntry(BinaryReaderEx br)
            {
                EventType type = br.GetEnum32<EventType>(br.Position + 0xC);

                switch (type)
                {
                    case EventType.Treasure:
                        var treasure = new Event.Treasure(br);
                        Treasures.Add(treasure);
                        return treasure;

                    case EventType.Generator:
                        var generator = new Event.Generator(br);
                        Generators.Add(generator);
                        return generator;

                    case EventType.ObjAct:
                        var objAct = new Event.ObjAct(br);
                        ObjActs.Add(objAct);
                        return objAct;

                    case EventType.MapOffset:
                        var mapOffset = new Event.MapOffset(br);
                        MapOffsets.Add(mapOffset);
                        return mapOffset;

                    case EventType.PseudoMultiplayer:
                        var invasion = new Event.PseudoMultiplayer(br);
                        PseudoMultiplayers.Add(invasion);
                        return invasion;

                    case EventType.WalkRoute:
                        var walkRoute = new Event.WalkRoute(br);
                        WalkRoutes.Add(walkRoute);
                        return walkRoute;

                    case EventType.GroupTour:
                        var groupTour = new Event.GroupTour(br);
                        GroupTours.Add(groupTour);
                        return groupTour;

                    case EventType.Other:
                        var other = new Event.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unsupported event type: {type}");
                }
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Event entry)
            {
                entry.Write(bw, id);
            }

            public void Add(IMsbEvent item)
            {
                switch (item)
                {
                    case Event.Treasure e:
                        Treasures.Add(e);
                        break;
                    case Event.Generator e:
                        Generators.Add(e);
                        break;
                    case Event.ObjAct e:
                        ObjActs.Add(e);
                        break;
                    case Event.MapOffset e:
                        MapOffsets.Add(e);
                        break;
                    case Event.PseudoMultiplayer e:
                        PseudoMultiplayers.Add(e);
                        break;
                    case Event.WalkRoute e:
                        WalkRoutes.Add(e);
                        break;
                    case Event.GroupTour e:
                        GroupTours.Add(e);
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
            Unk0D = 0xD,
            WalkRoute = 0xE,
            GroupTour = 0xF,
            Unk10 = 0x10,
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

            /// <summary>
            /// Unknown.
            /// </summary>
            public int EventID { get; set; }

            /// <summary>
            /// The name of a part the event is attached to.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Part))]
            public string PartName { get; set; }
            private int PartIndex;

            /// <summary>
            /// The name of a region the event is attached to.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Region))]
            public string PointName { get; set; }
            private int PointIndex;

            /// <summary>
            /// Used to identify the event in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            internal Event(string name)
            {
                Name = name;
                EventID = -1;
                EntityID = -1;
            }

            internal Event(Event clone)
            {
                Name = clone.Name;
                EventID = clone.EventID;
                PartName = clone.PartName;
                PointName = clone.PointName;
                EntityID = clone.EntityID;
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

                br.Position = start + baseDataOffset;
                PartIndex = br.ReadInt32();
                PointIndex = br.ReadInt32();
                EntityID = br.ReadInt32();
                br.AssertInt32(0);

                br.Position = start + typeDataOffset;
                Read(br);
            }

            internal abstract void Read(BinaryReaderEx br);

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(EventID);
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteInt32(0);
                bw.ReserveInt64("BaseDataOffset");
                bw.ReserveInt64("TypeDataOffset");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.Pad(8);

                bw.FillInt64("BaseDataOffset", bw.Position - start);
                bw.WriteInt32(PartIndex);
                bw.WriteInt32(PointIndex);
                bw.WriteInt32(EntityID);
                bw.WriteInt32(0);

                bw.FillInt64("TypeDataOffset", bw.Position - start);
                WriteSpecific(bw);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw);

            internal virtual void GetNames(MSB3 msb, Entries entries)
            {
                PartName = MSB.FindName(entries.Parts, PartIndex);
                PointName = MSB.FindName(entries.Regions, PointIndex);
            }

            internal virtual void GetIndices(MSB3 msb, Entries entries)
            {
                PartIndex = MSB.FindIndex(entries.Parts, PartName);
                PointIndex = MSB.FindIndex(entries.Regions, PointName);
            }

            /// <summary>
            /// Returns the type and name of this event.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }

            /// <summary>
            /// A pickuppable item.
            /// </summary>
            public class Treasure : Event
            {
                internal override EventType Type => EventType.Treasure;

                /// <summary>
                /// The part the treasure is attached to.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string PartName2 { get; set; }
                private int PartIndex2;

                /// <summary>
                /// IDs in the item lot param given by this treasure.
                /// </summary>
                [MSBParamReference(ParamName = "ItemLotParam")]
                public int ItemLot1 { get; set; }
                [MSBParamReference(ParamName = "ItemLotParam")]
                public int ItemLot2 { get; set; }

                /// <summary>
                /// Unknown; always -1 in vanilla.
                /// </summary>
                [MSBParamReference(ParamName = "ActionButtonParam")]
                public int ActionButtonParamID { get; set; }

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

                /// <summary>
                /// Creates a new Treasure with the given name.
                /// </summary>
                public Treasure(string name) : base(name)
                {
                    ItemLot1 = -1;
                    ItemLot2 = -1;
                    ActionButtonParamID = -1;
                    PickupAnimID = -1;
                }

                /// <summary>
                /// Creates a new Treasure with values copied from another.
                /// </summary>
                public Treasure(Treasure clone) : base(clone)
                {
                    PartName2 = clone.PartName2;
                    ItemLot1 = clone.ItemLot1;
                    ItemLot2 = clone.ItemLot2;
                    ActionButtonParamID = clone.ActionButtonParamID;
                    PickupAnimID = clone.PickupAnimID;
                    InChest = clone.InChest;
                    StartDisabled = clone.StartDisabled;
                }

                internal Treasure(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    PartIndex2 = br.ReadInt32();
                    br.AssertInt32(0);
                    ItemLot1 = br.ReadInt32();
                    ItemLot2 = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    ActionButtonParamID = br.ReadInt32();
                    PickupAnimID = br.ReadInt32();

                    InChest = br.ReadBoolean();
                    StartDisabled = br.ReadBoolean();
                    br.AssertByte(0);
                    br.AssertByte(0);

                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(PartIndex2);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ItemLot1);
                    bw.WriteInt32(ItemLot2);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(ActionButtonParamID);
                    bw.WriteInt32(PickupAnimID);

                    bw.WriteBoolean(InChest);
                    bw.WriteBoolean(StartDisabled);
                    bw.WriteByte(0);
                    bw.WriteByte(0);

                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    PartName2 = MSB.FindName(entries.Parts, PartIndex2);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    PartIndex2 = MSB.FindIndex(entries.Parts, PartName2);
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

                /// <summary>
                /// Regions that enemies can be spawned at.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string[] SpawnPointNames { get; private set; }
                private int[] SpawnPointIndices;

                /// <summary>
                /// Enemies spawned by this generator.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string[] SpawnPartNames { get; private set; }
                private int[] SpawnPartIndices;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int SessionCondition { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }
                public float UnkT18 { get; set; }

                /// <summary>
                /// Creates a new Generator with the given name.
                /// </summary>
                public Generator(string name) : base(name)
                {
                    SpawnPointNames = new string[8];
                    SpawnPartNames = new string[32];
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
                    SessionCondition = clone.SessionCondition;
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
                    SessionCondition = br.ReadInt32();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    SpawnPointIndices = br.ReadInt32s(8);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    SpawnPartIndices = br.ReadInt32s(32);
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
                    bw.WriteInt32(SessionCondition);
                    bw.WriteSingle(UnkT14);
                    bw.WriteSingle(UnkT18);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(SpawnPointIndices);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(SpawnPartIndices);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    SpawnPointNames = MSB.FindNames(entries.Regions, SpawnPointIndices);
                    SpawnPartNames = MSB.FindNames(entries.Parts, SpawnPartIndices);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    SpawnPointIndices = MSB.FindIndices(entries.Regions, SpawnPointNames);
                    SpawnPartIndices = MSB.FindIndices(entries.Parts, SpawnPartNames);
                }
            }

            /// <summary>
            /// Controls usable objects like levers.
            /// </summary>
            public class ObjAct : Event
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public enum ObjActState : byte
                {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
                    OneState = 0,
                    DoorState = 1,
                    OneLoopState = 2,
                    OneLoopState2 = 3,
                    DoorState2 = 4,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
                }

                internal override EventType Type => EventType.ObjAct;

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBEntityReference]
                public int ObjActEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string PartName2 { get; set; }
                private int PartIndex2;

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBParamReference(ParamName = "ObjActParam")]
                public int ObjActParamID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public ObjActState ObjActStateType { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int EventFlagID { get; set; }

                /// <summary>
                /// Creates a new ObjAct with the given name.
                /// </summary>
                public ObjAct(string name) : base(name)
                {
                    ObjActEntityID = -1;
                    ObjActStateType = ObjActState.OneState;
                }

                /// <summary>
                /// Creates a new ObjAct with values copied from another.
                /// </summary>
                public ObjAct(ObjAct clone) : base(clone)
                {
                    ObjActEntityID = clone.ObjActEntityID;
                    PartName2 = clone.PartName2;
                    ObjActParamID = clone.ObjActParamID;
                    ObjActStateType = clone.ObjActStateType;
                    EventFlagID = clone.EventFlagID;
                }

                internal ObjAct(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    ObjActEntityID = br.ReadInt32();
                    PartIndex2 = br.ReadInt32();
                    ObjActParamID = br.ReadInt32();

                    ObjActStateType = br.ReadEnum8<ObjActState>();
                    br.AssertByte(0);
                    br.AssertByte(0);
                    br.AssertByte(0);

                    EventFlagID = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(ObjActEntityID);
                    bw.WriteInt32(PartIndex2);
                    bw.WriteInt32(ObjActParamID);

                    bw.WriteByte((byte)ObjActStateType);
                    bw.WriteByte(0);
                    bw.WriteByte(0);
                    bw.WriteByte(0);

                    bw.WriteInt32(EventFlagID);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    PartName2 = MSB.FindName(entries.Parts, PartIndex2);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    PartIndex2 = MSB.FindIndex(entries.Parts, PartName2);
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

                /// <summary>
                /// Creates a new MapOffset with the given name.
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
            /// A fake multiplayer interaction where the player goes to an NPC's world.
            /// </summary>
            public class PseudoMultiplayer : Event
            {
                internal override EventType Type => EventType.PseudoMultiplayer;

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBEntityReference]
                public int HostEntityID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBEntityReference]
                public int InvasionEntityID { get; set; }

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

                /// <summary>
                /// Creates a new Invasion with the given name.
                /// </summary>
                public PseudoMultiplayer(string name) : base(name)
                {
                    HostEntityID = -1;
                    InvasionEntityID = -1;
                    InvasionRegionIndex = -1;
                    SoundIDMaybe = -1;
                    MapEventIDMaybe = -1;
                }

                /// <summary>
                /// Creates a new Invasion with values copied from another.
                /// </summary>
                public PseudoMultiplayer(PseudoMultiplayer clone) : base(clone)
                {
                    HostEntityID = clone.HostEntityID;
                    InvasionEntityID = clone.InvasionEntityID;
                    InvasionRegionIndex = clone.InvasionRegionIndex;
                    SoundIDMaybe = clone.SoundIDMaybe;
                    MapEventIDMaybe = clone.MapEventIDMaybe;
                    FlagsMaybe = clone.FlagsMaybe;
                    UnkT18 = clone.UnkT18;
                }

                internal PseudoMultiplayer(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    HostEntityID = br.ReadInt32();
                    InvasionEntityID = br.ReadInt32();
                    InvasionRegionIndex = br.ReadInt32();
                    SoundIDMaybe = br.ReadInt32();
                    MapEventIDMaybe = br.ReadInt32();
                    FlagsMaybe = br.ReadInt32();
                    UnkT18 = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(HostEntityID);
                    bw.WriteInt32(InvasionEntityID);
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

                /// <summary>
                /// List of points in the route.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string[] WalkPointNames { get; private set; }
                private short[] WalkPointIndices;

                /// <summary>
                /// Creates a new WalkRoute with the given name.
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
                    UnkT00 = br.AssertInt32(0, 1, 2, 5);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    WalkPointIndices = br.ReadInt16s(32);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt16s(WalkPointIndices);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    WalkPointNames = new string[WalkPointIndices.Length];
                    for (int i = 0; i < WalkPointIndices.Length; i++)
                        WalkPointNames[i] = MSB.FindName(entries.Regions, WalkPointIndices[i]);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    WalkPointIndices = new short[WalkPointNames.Length];
                    for (int i = 0; i < WalkPointNames.Length; i++)
                        WalkPointIndices[i] = (short)MSB.FindIndex(entries.Regions, WalkPointNames[i]);
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
                public int PlatoonIDScriptActivate { get; set; }
                public int State { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Part))]
                public string[] GroupPartsNames { get; private set; }
                private int[] GroupPartsIndices;

                /// <summary>
                /// Creates a new GroupTour with the given name.
                /// </summary>
                public GroupTour(string name) : base(name)
                {
                    GroupPartsNames = new string[32];
                }

                /// <summary>
                /// Creates a new GroupTour with values copied from another.
                /// </summary>
                public GroupTour(GroupTour clone) : base(clone)
                {
                    PlatoonIDScriptActivate = clone.PlatoonIDScriptActivate;
                    State = clone.State;
                    GroupPartsNames = (string[])clone.GroupPartsNames.Clone();
                }

                internal GroupTour(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    PlatoonIDScriptActivate = br.ReadInt32();
                    State = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    GroupPartsIndices = br.ReadInt32s(32);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(PlatoonIDScriptActivate);
                    bw.WriteInt32(State);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(GroupPartsIndices);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    GroupPartsNames = MSB.FindNames(entries.Parts, GroupPartsIndices);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    GroupPartsIndices = MSB.FindIndices(entries.Parts, GroupPartsNames);
                }
            }

            /// <summary>
            /// Unknown. Only appears once in one unused MSB so it's hard to draw too many conclusions from it.
            /// </summary>
            public class Other : Event
            {
                internal override EventType Type => EventType.Other;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int SoundTypeMaybe { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int SoundIDMaybe { get; set; }

                /// <summary>
                /// Creates a new Other with the given name.
                /// </summary>
                public Other(string name) : base(name)
                {
                    SoundTypeMaybe = 0;
                    SoundIDMaybe = 0;
                }

                /// <summary>
                /// Creates a new Other with values copied from another.
                /// </summary>
                public Other(Other clone) : base(clone)
                {
                    SoundTypeMaybe = clone.SoundTypeMaybe;
                    SoundIDMaybe = clone.SoundIDMaybe;
                }

                internal Other(BinaryReaderEx br) : base(br) { }

                internal override void Read(BinaryReaderEx br)
                {
                    SoundTypeMaybe = br.ReadInt32();
                    SoundIDMaybe = br.ReadInt32();

                    for (int i = 0; i < 16; i++)
                        br.AssertInt32(-1);
                }

                internal override void WriteSpecific(BinaryWriterEx bw)
                {
                    bw.WriteInt32(SoundTypeMaybe);
                    bw.WriteInt32(SoundIDMaybe);

                    for (int i = 0; i < 16; i++)
                        bw.WriteInt32(-1);
                }
            }
        }
    }
}
