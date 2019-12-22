using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum EventType : uint
        {
            Treasure = 4,
            Generator = 5,
            ObjAct = 7,
            MapOffset = 9,
            WalkRoute = 14,
            GroupTour = 15,
            Event17 = 17,
            Event18 = 18,
            Event20 = 20,
            Event21 = 21,
            PartsGroup = 22,
            Talk = 23,
            AutoDrawGroup = 24,
            Other = 0xFFFFFFFF,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Dynamic or interactive systems such as item pickups, levers, enemy spawners, etc.
        /// </summary>
        public class EventParam : Param<Event>, IMsbParam<IMsbEvent>
        {
            /// <summary>
            /// Item pickups out in the open or inside containers.
            /// </summary>
            public List<Event.Treasure> Treasures { get; set; }

            /// <summary>
            /// Enemy spawners.
            /// </summary>
            public List<Event.Generator> Generators { get; set; }

            /// <summary>
            /// Interactive objects like levers and doors.
            /// </summary>
            public List<Event.ObjAct> ObjActs { get; set; }

            /// <summary>
            /// Shift the entire map by a certain amount.
            /// </summary>
            public List<Event.MapOffset> MapOffsets { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.WalkRoute> WalkRoutes { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.GroupTour> GroupTours { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Event17> Event17s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Event18> Event18s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Event20> Event20s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Event21> Event21s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.PartsGroup> PartsGroups { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Talk> Talks { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.AutoDrawGroup> AutoDrawGroups { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Event.Other> Others { get; set; }

            /// <summary>
            /// Creates an empty EventParam with the given version.
            /// </summary>
            public EventParam(int unk00 = 0x23) : base(unk00, "EVENT_PARAM_ST")
            {
                Treasures = new List<Event.Treasure>();
                Generators = new List<Event.Generator>();
                ObjActs = new List<Event.ObjAct>();
                MapOffsets = new List<Event.MapOffset>();
                WalkRoutes = new List<Event.WalkRoute>();
                GroupTours = new List<Event.GroupTour>();
                Event17s = new List<Event.Event17>();
                Event18s = new List<Event.Event18>();
                Event20s = new List<Event.Event20>();
                Event21s = new List<Event.Event21>();
                PartsGroups = new List<Event.PartsGroup>();
                Talks = new List<Event.Talk>();
                AutoDrawGroups = new List<Event.AutoDrawGroup>();
                Others = new List<Event.Other>();
            }

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

                    case EventType.WalkRoute:
                        var walkRoute = new Event.WalkRoute(br);
                        WalkRoutes.Add(walkRoute);
                        return walkRoute;

                    case EventType.GroupTour:
                        var groupTour = new Event.GroupTour(br);
                        GroupTours.Add(groupTour);
                        return groupTour;

                    case EventType.Event17:
                        var event17 = new Event.Event17(br);
                        Event17s.Add(event17);
                        return event17;

                    case EventType.Event18:
                        var event18 = new Event.Event18(br);
                        Event18s.Add(event18);
                        return event18;

                    case EventType.Event20:
                        var event20 = new Event.Event20(br);
                        Event20s.Add(event20);
                        return event20;

                    case EventType.Event21:
                        var event21 = new Event.Event21(br);
                        Event21s.Add(event21);
                        return event21;

                    case EventType.PartsGroup:
                        var partsGroup = new Event.PartsGroup(br);
                        PartsGroups.Add(partsGroup);
                        return partsGroup;

                    case EventType.Talk:
                        var talk = new Event.Talk(br);
                        Talks.Add(talk);
                        return talk;

                    case EventType.AutoDrawGroup:
                        var autoDrawGroup = new Event.AutoDrawGroup(br);
                        AutoDrawGroups.Add(autoDrawGroup);
                        return autoDrawGroup;

                    case EventType.Other:
                        var other = new Event.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unimplemented model type: {type}");
                }
            }

            /// <summary>
            /// Returns every Event in the order they'll be written.
            /// </summary>
            public override List<Event> GetEntries()
            {
                return SFUtil.ConcatAll<Event>(
                    Treasures, Generators, ObjActs, MapOffsets, WalkRoutes,
                    GroupTours, Event17s, Event18s, Event20s, Event21s,
                    PartsGroups, Talks, AutoDrawGroups, Others);
            }
            IReadOnlyList<IMsbEvent> IMsbParam<IMsbEvent>.GetEntries() => GetEntries();

            public void Add(IMsbEvent item)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A dynamic or interactive system.
        /// </summary>
        public abstract class Event : Entry, IMsbEvent
        {
            /// <summary>
            /// The specific type of this Event.
            /// </summary>
            public abstract EventType Type { get; }

            internal abstract bool HasTypeData { get; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int EventIndex { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public string PartName { get; set; }
            private int PartIndex;

            /// <summary>
            /// Unknown.
            /// </summary>
            public string RegionName { get; set; }
            private int RegionIndex;

            /// <summary>
            /// Identifies the Event in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            internal Event()
            {
                Name = "";
                EventIndex = -1;
                EntityID = -1;
            }

            internal Event(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                EventIndex = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                br.AssertInt32(0);
                long baseDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);
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
                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(EventIndex);
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
                bw.WriteInt32(RegionIndex);
                bw.WriteInt32(EntityID);
                bw.WriteInt32(0);

                if (HasTypeData)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    WriteTypeData(bw);
                }
                else
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
            }

            internal virtual void WriteTypeData(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Type data should not be written for events with no type data.");
            }

            internal virtual void GetNames(MSBS msb, Entries entries)
            {
                PartName = MSB.FindName(entries.Parts, PartIndex);
                RegionName = MSB.FindName(entries.Regions, RegionIndex);
            }

            internal virtual void GetIndices(MSBS msb, Entries entries)
            {
                PartIndex = MSB.FindIndex(entries.Parts, PartName);
                RegionIndex = MSB.FindIndex(entries.Regions, RegionName);
            }

            /// <summary>
            /// Returns the type and name of the event as a string.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {Name}";
            }

            /// <summary>
            /// An item pickup in the open or inside a container.
            /// </summary>
            public class Treasure : Event
            {
                /// <summary>
                /// EventType.Treasure
                /// </summary>
                public override EventType Type => EventType.Treasure;

                internal override bool HasTypeData => true;

                /// <summary>
                /// The part that the treasure is attached to.
                /// </summary>
                public string TreasurePartName { get; set; }
                private int TreasurePartIndex;

                /// <summary>
                /// The item lot to be given.
                /// </summary>
                public int ItemLotID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int ActionButtonID { get; set; }

                /// <summary>
                /// The animation to play on pickup.
                /// </summary>
                public int PickupAnimID { get; set; }

                /// <summary>
                /// Whether the treasure is inside a container.
                /// </summary>
                public bool InChest { get; set; }

                /// <summary>
                /// Whether the treasure should be disabled by default.
                /// </summary>
                public bool StartDisabled { get; set; }

                /// <summary>
                /// Creates a Treasure with default values.
                /// </summary>
                public Treasure() : base()
                {
                    ItemLotID = -1;
                    ActionButtonID = -1;
                    PickupAnimID = -1;
                }

                internal Treasure(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    TreasurePartIndex = br.ReadInt32();
                    br.AssertInt32(0);
                    ItemLotID = br.ReadInt32();
                    br.AssertPattern(0x24, 0xFF);
                    ActionButtonID = br.ReadInt32();
                    PickupAnimID = br.ReadInt32();
                    InChest = br.ReadBoolean();
                    StartDisabled = br.ReadBoolean();
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(TreasurePartIndex);
                    bw.WriteInt32(0);
                    bw.WriteInt32(ItemLotID);
                    bw.WritePattern(0x24, 0xFF);
                    bw.WriteInt32(ActionButtonID);
                    bw.WriteInt32(PickupAnimID);
                    bw.WriteBoolean(InChest);
                    bw.WriteBoolean(StartDisabled);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    TreasurePartName = MSB.FindName(entries.Parts, TreasurePartIndex);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    TreasurePartIndex = MSB.FindIndex(entries.Parts, TreasurePartName);
                }
            }

            /// <summary>
            /// An enemy spawner.
            /// </summary>
            public class Generator : Event
            {
                /// <summary>
                /// EventType.Generator
                /// </summary>
                public override EventType Type => EventType.Generator;

                internal override bool HasTypeData => true;

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
                public int SessionCondition { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT18 { get; set; }

                /// <summary>
                /// Regions where parts will spawn from.
                /// </summary>
                public string[] SpawnRegionNames { get; private set; }
                private int[] SpawnRegionIndices;

                /// <summary>
                /// Parts that will be respawned.
                /// </summary>
                public string[] SpawnPartNames { get; private set; }
                private int[] SpawnPartIndices;

                /// <summary>
                /// Creates a Generator with default values.
                /// </summary>
                public Generator() : base()
                {
                    SpawnRegionNames = new string[8];
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
                    SessionCondition = br.ReadInt32();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    br.AssertPattern(0x14, 0x00);
                    SpawnRegionIndices = br.ReadInt32s(8);
                    br.AssertPattern(0x10, 0x00);
                    SpawnPartIndices = br.ReadInt32s(32);
                    br.AssertPattern(0x20, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
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
                    bw.WritePattern(0x14, 0x00);
                    bw.WriteInt32s(SpawnRegionIndices);
                    bw.WritePattern(0x10, 0x00);
                    bw.WriteInt32s(SpawnPartIndices);
                    bw.WritePattern(0x20, 0x00);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    SpawnRegionNames = MSB.FindNames(entries.Regions, SpawnRegionIndices);
                    SpawnPartNames = MSB.FindNames(entries.Parts, SpawnPartIndices);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    SpawnRegionIndices = MSB.FindIndices(entries.Regions, SpawnRegionNames);
                    SpawnPartIndices = MSB.FindIndices(entries.Parts, SpawnPartNames);
                }
            }

            /// <summary>
            /// An interactive object.
            /// </summary>
            public class ObjAct : Event
            {
                /// <summary>
                /// EventType.ObjAct
                /// </summary>
                public override EventType Type => EventType.ObjAct;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown why objacts need an extra entity ID.
                /// </summary>
                public int ObjActEntityID { get; set; }

                /// <summary>
                /// The part to be interacted with.
                /// </summary>
                public string ObjActPartName { get; set; }
                private int ObjActPartIndex;

                /// <summary>
                /// A row in ObjActParam.
                /// </summary>
                public int ObjActID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte StateType { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int EventFlagID { get; set; }

                /// <summary>
                /// Creates an ObjAct with default values.
                /// </summary>
                public ObjAct() : base()
                {
                    ObjActEntityID = -1;
                    ObjActID = -1;
                    EventFlagID = -1;
                }

                internal ObjAct(BinaryReaderEx br) : base(br)
                {
                    ObjActEntityID = br.ReadInt32();
                    ObjActPartIndex = br.ReadInt32();
                    ObjActID = br.ReadInt32();
                    StateType = br.ReadByte();
                    br.AssertByte(0);
                    br.AssertInt16(0);
                    EventFlagID = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(ObjActEntityID);
                    bw.WriteInt32(ObjActPartIndex);
                    bw.WriteInt32(ObjActID);
                    bw.WriteByte(StateType);
                    bw.WriteByte(0);
                    bw.WriteInt16(0);
                    bw.WriteInt32(EventFlagID);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    ObjActPartName = MSB.FindName(entries.Parts, ObjActPartIndex);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    ObjActPartIndex = MSB.FindIndex(entries.Parts, ObjActPartName);
                }
            }

            /// <summary>
            /// Shifts the entire map; already accounted for in MSB coordinates.
            /// </summary>
            public class MapOffset : Event
            {
                /// <summary>
                /// EventType.MapOffset
                /// </summary>
                public override EventType Type => EventType.MapOffset;

                internal override bool HasTypeData => true;

                /// <summary>
                /// How much to shift by.
                /// </summary>
                public Vector3 Position { get; set; }

                /// <summary>
                /// Unknown, but looks like rotation.
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

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteVector3(Position);
                    bw.WriteSingle(Degree);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class WalkRoute : Event
            {
                /// <summary>
                /// EventType.WalkRoute
                /// </summary>
                public override EventType Type => EventType.WalkRoute;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public string[] WalkRegionNames { get; private set; }
                private short[] WalkRegionIndices;

                /// <summary>
                /// Unknown.
                /// </summary>
                public WREntry[] WREntries { get; set; }

                /// <summary>
                /// Creates a WalkRoute with default values.
                /// </summary>
                public WalkRoute() : base()
                {
                    WalkRegionNames = new string[32];
                    WREntries = new WREntry[5];
                    for (int i = 0; i < 5; i++)
                        WREntries[i] = new WREntry();
                }

                internal WalkRoute(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    WalkRegionIndices = br.ReadInt16s(32);
                    WREntries = new WREntry[5];
                    for (int i = 0; i < 5; i++)
                        WREntries[i] = new WREntry(br);
                    br.AssertPattern(0x14, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt16s(WalkRegionIndices);
                    for (int i = 0; i < 5; i++)
                        WREntries[i].Write(bw);
                    bw.WritePattern(0x14, 0x00);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    WalkRegionNames = new string[WalkRegionIndices.Length];
                    for (int i = 0; i < WalkRegionIndices.Length; i++)
                        WalkRegionNames[i] = MSB.FindName(entries.Regions, WalkRegionIndices[i]);

                    foreach (WREntry wrEntry in WREntries)
                        wrEntry.GetNames(entries);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    WalkRegionIndices = new short[WalkRegionNames.Length];
                    for (int i = 0; i < WalkRegionNames.Length; i++)
                        WalkRegionIndices[i] = (short)MSB.FindIndex(entries.Regions, WalkRegionNames[i]);

                    foreach (WREntry wrEntry in WREntries)
                        wrEntry.GetIndices(entries);
                }

                /// <summary>
                /// Unknown.
                /// </summary>
                public class WREntry
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public string RegionName { get; set; }
                    private short RegionIndex;

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk04 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk08 { get; set; }

                    /// <summary>
                    /// Creates a WREntry with default values.
                    /// </summary>
                    public WREntry() { }

                    internal WREntry(BinaryReaderEx br)
                    {
                        RegionIndex = br.ReadInt16();
                        br.AssertInt16(0);
                        Unk04 = br.ReadInt32();
                        Unk08 = br.ReadInt32();
                    }

                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteInt16(RegionIndex);
                        bw.WriteInt16(0);
                        bw.WriteInt32(Unk04);
                        bw.WriteInt32(Unk08);
                    }

                    internal void GetNames(Entries entries)
                    {
                        RegionName = MSB.FindName(entries.Regions, RegionIndex);
                    }

                    internal void GetIndices(Entries entries)
                    {
                        RegionIndex = (short)MSB.FindIndex(entries.Regions, RegionName);
                    }
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class GroupTour : Event
            {
                /// <summary>
                /// EventType.GroupTour
                /// </summary>
                public override EventType Type => EventType.GroupTour;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int PlatoonIDScriptActive { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int State { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public string[] GroupPartNames { get; private set; }
                private int[] GroupPartIndices;

                /// <summary>
                /// Creates a GroupTour with default values.
                /// </summary>
                public GroupTour() : base()
                {
                    GroupPartNames = new string[32];
                }

                internal GroupTour(BinaryReaderEx br) : base(br)
                {
                    PlatoonIDScriptActive = br.ReadInt32();
                    State = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    GroupPartIndices = br.ReadInt32s(32);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(PlatoonIDScriptActive);
                    bw.WriteInt32(State);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32s(GroupPartIndices);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    GroupPartNames = MSB.FindNames(entries.Parts, GroupPartIndices);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    GroupPartIndices = MSB.FindIndices(entries.Parts, GroupPartNames);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Event17 : Event
            {
                /// <summary>
                /// EventType.Event17
                /// </summary>
                public override EventType Type => EventType.Event17;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates an Event17 with default values.
                /// </summary>
                public Event17() : base() { }

                internal Event17(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertPattern(0x1C, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WritePattern(0x1C, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Event18 : Event
            {
                /// <summary>
                /// EventType.Event18
                /// </summary>
                public override EventType Type => EventType.Event18;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates an Event18 with default values.
                /// </summary>
                public Event18() : base() { }

                internal Event18(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertPattern(0x1C, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WritePattern(0x1C, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Event20 : Event
            {
                /// <summary>
                /// EventType.Event20
                /// </summary>
                public override EventType Type => EventType.Event20;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT06 { get; set; }

                /// <summary>
                /// Creates an Event20 with default values.
                /// </summary>
                public Event20() : base() { }

                internal Event20(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt16();
                    UnkT06 = br.ReadInt16();
                    br.AssertPattern(0x18, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt16(UnkT04);
                    bw.WriteInt16(UnkT06);
                    bw.WritePattern(0x18, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Event21 : Event
            {
                /// <summary>
                /// EventType.Event21
                /// </summary>
                public override EventType Type => EventType.Event21;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public string[] Event21PartNames { get; private set; }
                private int[] Event21PartIndices;

                /// <summary>
                /// Creates an Event21 with default values.
                /// </summary>
                public Event21() : base()
                {
                    Event21PartNames = new string[32];
                }

                internal Event21(BinaryReaderEx br) : base(br)
                {
                    Event21PartIndices = br.ReadInt32s(32);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32s(Event21PartIndices);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    Event21PartNames = MSB.FindNames(entries.Parts, Event21PartIndices);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    Event21PartIndices = MSB.FindIndices(entries.Parts, Event21PartNames);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class PartsGroup : Event
            {
                /// <summary>
                /// EventType.PartsGroup
                /// </summary>
                public override EventType Type => EventType.PartsGroup;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a PartsGroup with default values.
                /// </summary>
                public PartsGroup() : base() { }

                internal PartsGroup(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Talk : Event
            {
                /// <summary>
                /// EventType.Talk
                /// </summary>
                public override EventType Type => EventType.Talk;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public string[] EnemyNames { get; private set; }
                private int[] EnemyIndices;

                /// <summary>
                /// IDs of talk ESDs.
                /// </summary>
                public int[] TalkIDs { get; private set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT44 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT46 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT48 { get; set; }

                /// <summary>
                /// Creates a Talk with default values.
                /// </summary>
                public Talk() : base()
                {
                    EnemyNames = new string[8];
                    TalkIDs = new int[8];
                }

                internal Talk(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    EnemyIndices = br.ReadInt32s(8);
                    TalkIDs = br.ReadInt32s(8);
                    UnkT44 = br.ReadInt16();
                    UnkT46 = br.ReadInt16();
                    UnkT48 = br.ReadInt32();
                    br.AssertPattern(0x34, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32s(EnemyIndices);
                    bw.WriteInt32s(TalkIDs);
                    bw.WriteInt16(UnkT44);
                    bw.WriteInt16(UnkT46);
                    bw.WriteInt32(UnkT48);
                    bw.WritePattern(0x34, 0x00);
                }

                internal override void GetNames(MSBS msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    EnemyNames = MSB.FindNames(msb.Parts.Enemies, EnemyIndices);
                }

                internal override void GetIndices(MSBS msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    EnemyIndices = MSB.FindIndices(msb.Parts.Enemies, EnemyNames);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class AutoDrawGroup : Event
            {
                /// <summary>
                /// EventType.AutoDrawGroup
                /// </summary>
                public override EventType Type => EventType.AutoDrawGroup;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT04 { get; set; }

                /// <summary>
                /// Creates an AutoDrawGroup with default values.
                /// </summary>
                public AutoDrawGroup() : base() { }

                internal AutoDrawGroup(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertPattern(0x18, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(UnkT04);
                    bw.WritePattern(0x18, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Other : Event
            {
                /// <summary>
                /// EventType.Other
                /// </summary>
                public override EventType Type => EventType.Other;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates an Other with default values.
                /// </summary>
                public Other() : base() { }

                internal Other(BinaryReaderEx br) : base(br) { }
            }
        }
    }
}
