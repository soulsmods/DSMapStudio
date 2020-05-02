using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// A section containing points and volumes for various purposes.
        /// </summary>
        public class PointParam : Param<Region>, IMsbParam<IMsbRegion>
        {
            internal override string Type => "POINT_PARAM_ST";

            /// <summary>
            /// General regions in the MSB.
            /// </summary>
            public List<Region.General> General;

            /// <summary>
            /// Unk00 regions in the MSB.
            /// </summary>
            public List<Region.Unk00> Unk00s;

            /// <summary>
            /// InvasionPoints in the MSB.
            /// </summary>
            public List<Region.InvasionPoint> InvasionPoints;

            /// <summary>
            /// EnvironmentMapPoints in the MSB.
            /// </summary>
            public List<Region.EnvironmentMapPoint> EnvironmentMapPoints;

            /// <summary>
            /// Sound regions in the MSB.
            /// </summary>
            public List<Region.Sound> Sounds;

            /// <summary>
            /// SFX regions in the MSB.
            /// </summary>
            public List<Region.SFX> SFX;

            /// <summary>
            /// WindSFX regions in the MSB.
            /// </summary>
            public List<Region.WindSFX> WindSFX;

            /// <summary>
            /// SpawnPoints in the MSB.
            /// </summary>
            public List<Region.SpawnPoint> SpawnPoints;

            /// <summary>
            /// Messages in the MSB.
            /// </summary>
            public List<Region.Message> Messages;

            /// <summary>
            /// WalkRoute points in the MSB.
            /// </summary>
            public List<Region.WalkRoute> WalkRoutes;

            /// <summary>
            /// Unk12 regions in the MSB.
            /// </summary>
            public List<Region.Unk12> Unk12s;

            /// <summary>
            /// WarpPoints in the MSB.
            /// </summary>
            public List<Region.WarpPoint> WarpPoints;

            /// <summary>
            /// ActivationAreas in the MSB.
            /// </summary>
            public List<Region.ActivationArea> ActivationAreas;

            /// <summary>
            /// Event regions in the MSB.
            /// </summary>
            public List<Region.Event> Events;

            /// <summary>
            /// EnvironmentMapEffectBoxes in the MSB.
            /// </summary>
            public List<Region.EnvironmentMapEffectBox> EnvironmentMapEffectBoxes;

            /// <summary>
            /// WindAreas in the MSB.
            /// </summary>
            public List<Region.WindArea> WindAreas;

            /// <summary>
            /// MufflingBoxes in the MSB.
            /// </summary>
            public List<Region.MufflingBox> MufflingBoxes;

            /// <summary>
            /// MufflingPortals in the MSB.
            /// </summary>
            public List<Region.MufflingPortal> MufflingPortals;

            /// <summary>
            /// Creates a new PointParam with no regions.
            /// </summary>
            public PointParam(int unk1 = 3) : base(unk1)
            {
                General = new List<Region.General>();
                Unk00s = new List<Region.Unk00>();
                InvasionPoints = new List<Region.InvasionPoint>();
                EnvironmentMapPoints = new List<Region.EnvironmentMapPoint>();
                Sounds = new List<Region.Sound>();
                SFX = new List<Region.SFX>();
                WindSFX = new List<Region.WindSFX>();
                SpawnPoints = new List<Region.SpawnPoint>();
                Messages = new List<Region.Message>();
                WalkRoutes = new List<Region.WalkRoute>();
                Unk12s = new List<Region.Unk12>();
                WarpPoints = new List<Region.WarpPoint>();
                ActivationAreas = new List<Region.ActivationArea>();
                Events = new List<Region.Event>();
                EnvironmentMapEffectBoxes = new List<Region.EnvironmentMapEffectBox>();
                WindAreas = new List<Region.WindArea>();
                MufflingBoxes = new List<Region.MufflingBox>();
                MufflingPortals = new List<Region.MufflingPortal>();
            }

            /// <summary>
            /// Returns every region in the order they will be written.
            /// </summary>
            public override List<Region> GetEntries()
            {
                return SFUtil.ConcatAll<Region>(
                    InvasionPoints, EnvironmentMapPoints, Sounds, SFX, WindSFX,
                    SpawnPoints, Messages, WalkRoutes, Unk12s, WarpPoints,
                    ActivationAreas, Events, Unk00s, EnvironmentMapEffectBoxes, WindAreas,
                    MufflingBoxes, MufflingPortals, General);
            }
            IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() => GetEntries();

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                RegionType type = br.GetEnum32<RegionType>(br.Position + 0x8);

                switch (type)
                {
                    case RegionType.General:
                        var general = new Region.General(br);
                        General.Add(general);
                        return general;

                    case RegionType.Unk00:
                        var unk00 = new Region.Unk00(br);
                        Unk00s.Add(unk00);
                        return unk00;

                    case RegionType.InvasionPoint:
                        var invasion = new Region.InvasionPoint(br);
                        InvasionPoints.Add(invasion);
                        return invasion;

                    case RegionType.EnvironmentMapPoint:
                        var envMapPoint = new Region.EnvironmentMapPoint(br);
                        EnvironmentMapPoints.Add(envMapPoint);
                        return envMapPoint;

                    case RegionType.Sound:
                        var sound = new Region.Sound(br);
                        Sounds.Add(sound);
                        return sound;

                    case RegionType.SFX:
                        var sfx = new Region.SFX(br);
                        SFX.Add(sfx);
                        return sfx;

                    case RegionType.WindSFX:
                        var windSFX = new Region.WindSFX(br);
                        WindSFX.Add(windSFX);
                        return windSFX;

                    case RegionType.SpawnPoint:
                        var spawnPoint = new Region.SpawnPoint(br);
                        SpawnPoints.Add(spawnPoint);
                        return spawnPoint;

                    case RegionType.Message:
                        var message = new Region.Message(br);
                        Messages.Add(message);
                        return message;

                    case RegionType.WalkRoute:
                        var walkRoute = new Region.WalkRoute(br);
                        WalkRoutes.Add(walkRoute);
                        return walkRoute;

                    case RegionType.Unk12:
                        var unk12 = new Region.Unk12(br);
                        Unk12s.Add(unk12);
                        return unk12;

                    case RegionType.WarpPoint:
                        var warpPoint = new Region.WarpPoint(br);
                        WarpPoints.Add(warpPoint);
                        return warpPoint;

                    case RegionType.ActivationArea:
                        var activationArea = new Region.ActivationArea(br);
                        ActivationAreas.Add(activationArea);
                        return activationArea;

                    case RegionType.Event:
                        var ev = new Region.Event(br);
                        Events.Add(ev);
                        return ev;

                    case RegionType.EnvironmentMapEffectBox:
                        var envMapEffectBox = new Region.EnvironmentMapEffectBox(br);
                        EnvironmentMapEffectBoxes.Add(envMapEffectBox);
                        return envMapEffectBox;

                    case RegionType.WindArea:
                        var windArea = new Region.WindArea(br);
                        WindAreas.Add(windArea);
                        return windArea;

                    case RegionType.MufflingBox:
                        var muffBox = new Region.MufflingBox(br);
                        MufflingBoxes.Add(muffBox);
                        return muffBox;

                    case RegionType.MufflingPortal:
                        var muffPortal = new Region.MufflingPortal(br);
                        MufflingPortals.Add(muffPortal);
                        return muffPortal;

                    default:
                        throw new NotImplementedException($"Unsupported region type: {type}");
                }
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Region entry)
            {
                entry.Write(bw, id);
            }

            public void Add(IMsbRegion item)
            {
                switch (item)
                {
                    case Region.General r:
                        General.Add(r);
                        break;
                    case Region.Unk00 r:
                        Unk00s.Add(r);
                        break;
                    case Region.InvasionPoint r:
                        InvasionPoints.Add(r);
                        break;
                    case Region.EnvironmentMapPoint r:
                        EnvironmentMapPoints.Add(r);
                        break;
                    case Region.Sound r:
                        Sounds.Add(r);
                        break;
                    case Region.SFX r:
                        SFX.Add(r);
                        break;
                    case Region.WindSFX r:
                        WindSFX.Add(r);
                        break;
                    case Region.SpawnPoint r:
                        SpawnPoints.Add(r);
                        break;
                    case Region.Message r:
                        Messages.Add(r);
                        break;
                    case Region.WalkRoute r:
                        WalkRoutes.Add(r);
                        break;
                    case Region.Unk12 r:
                        Unk12s.Add(r);
                        break;
                    case Region.WarpPoint r:
                        WarpPoints.Add(r);
                        break;
                    case Region.ActivationArea r:
                        ActivationAreas.Add(r);
                        break;
                    case Region.Event r:
                        Events.Add(r);
                        break;
                    case Region.EnvironmentMapEffectBox r:
                        EnvironmentMapEffectBoxes.Add(r);
                        break;
                    case Region.WindArea r:
                        WindAreas.Add(r);
                        break;
                    case Region.MufflingBox r:
                        MufflingBoxes.Add(r);
                        break;
                    case Region.MufflingPortal r:
                        MufflingPortals.Add(r);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        internal enum RegionType : uint
        {
            General = 0xFFFFFFFF,
            Unk00 = 0,
            InvasionPoint = 1,
            EnvironmentMapPoint = 2,
            Sound = 4,
            SFX = 5,
            WindSFX = 6,
            SpawnPoint = 8,
            Message = 9,
            WalkRoute = 11,
            Unk12 = 12,
            WarpPoint = 13,
            ActivationArea = 14,
            Event = 15,
            EnvironmentMapEffectBox = 17,
            WindArea = 18,
            MufflingBox = 20,
            MufflingPortal = 21,
        }

        /// <summary>
        /// A point or volumetric area used for a variety of purposes.
        /// </summary>
        public abstract class Region : Entry, IMsbRegion
        {
            internal abstract RegionType Type { get; }

            /// <summary>
            /// The name of this region.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// Whether this region has additional type data. The only region type where this actually varies is Sound.
            /// </summary>
            public bool HasTypeData { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk2 { get; set; }

            /// <summary>
            /// The shape of this region.
            /// </summary>
            public MSB.Shape Shape { get; set; }

            /// <summary>
            /// Controls whether the event is present in different ceremonies. Maybe only used for Messages?
            /// </summary>
            public uint MapStudioLayer { get; set; }

            /// <summary>
            /// Center of the region.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<short> UnkA { get; set; }
            public List<short> UnkB { get; set; }

            /// <summary>
            /// Region is inactive unless this part is drawn; null for always active.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Part))]
            public string ActivationPartName { get; set; }
            private int ActivationPartIndex;

            /// <summary>
            /// An ID used to identify this region in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            internal Region(string name, bool hasTypeData)
            {
                Name = name;
                Position = Vector3.Zero;
                Rotation = Vector3.Zero;
                Shape = new MSB.Shape.Point();
                ActivationPartName = null;
                EntityID = -1;
                UnkA = new List<short>();
                UnkB = new List<short>();
                HasTypeData = hasTypeData;
            }

            internal Region(Region clone)
            {
                Name = clone.Name;
                Position = clone.Position;
                Rotation = clone.Rotation;
                Shape = clone.Shape.Clone();
                ActivationPartName = clone.ActivationPartName;
                EntityID = clone.EntityID;
                Unk2 = clone.Unk2;
                UnkA = new List<short>(clone.UnkA);
                UnkB = new List<short>(clone.UnkB);
                MapStudioLayer = clone.MapStudioLayer;
                HasTypeData = clone.HasTypeData;
            }

            internal Region(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                ShapeType shapeType = br.ReadEnum32<ShapeType>();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Unk2 = br.ReadInt32();
                long baseDataOffset1 = br.ReadInt64();
                long baseDataOffset2 = br.AssertInt64(baseDataOffset1 + 4);
                br.AssertInt32(-1);
                MapStudioLayer = br.ReadUInt32();
                long shapeDataOffset = br.ReadInt64();
                long baseDataOffset3 = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);

                br.Position = start + baseDataOffset1;
                short countA = br.ReadInt16();
                UnkA = new List<short>(br.ReadInt16s(countA));

                br.Position = start + baseDataOffset2;
                short countB = br.ReadInt16();
                UnkB = new List<short>(br.ReadInt16s(countB));

                br.Position = start + shapeDataOffset;
                switch (shapeType)
                {
                    case ShapeType.Point:
                        Shape = new MSB.Shape.Point();
                        break;

                    case ShapeType.Circle:
                        Shape = new MSB.Shape.Circle(br);
                        break;

                    case ShapeType.Sphere:
                        Shape = new MSB.Shape.Sphere(br);
                        break;

                    case ShapeType.Cylinder:
                        Shape = new MSB.Shape.Cylinder(br);
                        break;

                    case ShapeType.Box:
                        Shape = new MSB.Shape.Box(br);
                        break;

                    default:
                        throw new NotImplementedException($"Unsupported shape type: {shapeType}");
                }

                br.Position = start + baseDataOffset3;
                ActivationPartIndex = br.ReadInt32();
                EntityID = br.ReadInt32();

                HasTypeData = typeDataOffset != 0 || Type == RegionType.MufflingBox || Type == RegionType.MufflingPortal;
                if (HasTypeData)
                    ReadSpecific(br);
            }

            internal abstract void ReadSpecific(BinaryReaderEx br);

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteUInt32((uint)Shape.Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteInt32(Unk2);
                bw.ReserveInt64("BaseDataOffset1");
                bw.ReserveInt64("BaseDataOffset2");
                bw.WriteInt32(-1);
                bw.WriteUInt32(MapStudioLayer);

                bw.ReserveInt64("ShapeDataOffset");
                bw.ReserveInt64("BaseDataOffset3");
                bw.ReserveInt64("TypeDataOffset");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
                bw.Pad(4);

                bw.FillInt64("BaseDataOffset1", bw.Position - start);
                bw.WriteInt16((short)UnkA.Count);
                bw.WriteInt16s(UnkA);
                bw.Pad(4);

                bw.FillInt64("BaseDataOffset2", bw.Position - start);
                bw.WriteInt16((short)UnkB.Count);
                bw.WriteInt16s(UnkB);
                bw.Pad(8);

                //Shape.Write(bw, start);
                if (Shape.HasShapeData)
                {
                    bw.FillInt64("ShapeDataOffset", bw.Position - start);
                    Shape.WriteShapeData(bw);
                }
                else
                {
                    bw.FillInt64("ShapeDataOffset", 0);
                }

                bw.FillInt64("BaseDataOffset3", bw.Position - start);
                bw.WriteInt32(ActivationPartIndex);
                bw.WriteInt32(EntityID);

                if (HasTypeData)
                    WriteSpecific(bw, start);
                else
                    bw.FillInt64("TypeDataOffset", 0);

                bw.Pad(8);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw, long start);

            internal virtual void GetNames(MSB3 msb, Entries entries)
            {
                ActivationPartName = MSB.FindName(entries.Parts, ActivationPartIndex);
            }

            internal virtual void GetIndices(MSB3 msb, Entries entries)
            {
                ActivationPartIndex = MSB.FindIndex(entries.Parts, ActivationPartName);
            }

            /// <summary>
            /// Returns the region type, shape type, and name of this region.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {Shape.Type} : {Name}";
            }

            /// <summary>
            /// A region type with no type data.
            /// </summary>
            public abstract class SimpleRegion : Region
            {
                internal SimpleRegion(string name) : base(name, false) { }

                internal SimpleRegion(SimpleRegion clone) : base(clone) { }

                internal SimpleRegion(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    throw new InvalidOperationException("SimpleRegions should never have type data.");
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    throw new InvalidOperationException("SimpleRegions should never have type data.");
                }
            }

            /// <summary>
            /// Regions for random things.
            /// </summary>
            public class General : SimpleRegion
            {
                internal override RegionType Type => RegionType.General;

                /// <summary>
                /// Creates a new General with the given name.
                /// </summary>
                public General(string name) : base(name) { }

                /// <summary>
                /// Creates a new General region with values copied from another.
                /// </summary>
                public General(General clone) : base(clone) { }

                internal General(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown; only used 3 times in Catacombs.
            /// </summary>
            public class Unk00 : SimpleRegion
            {
                internal override RegionType Type => RegionType.Unk00;

                /// <summary>
                /// Creates a new Unk00 with the given name.
                /// </summary>
                public Unk00(string name) : base(name) { }

                /// <summary>
                /// Creates a new Unk00 with values copied from another.
                /// </summary>
                public Unk00(Unk00 clone) : base(clone) { }

                internal Unk00(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A point where other players invade your world.
            /// </summary>
            public class InvasionPoint : Region
            {
                internal override RegionType Type => RegionType.InvasionPoint;

                /// <summary>
                /// Not sure what this does.
                /// </summary>
                public int Priority { get; set; }

                /// <summary>
                /// Creates a new InvasionPoint with the given name.
                /// </summary>
                public InvasionPoint(string name) : base(name, true)
                {
                    Priority = 0;
                }

                /// <summary>
                /// Creates a new InvasionPoint with values copied from another.
                /// </summary>
                public InvasionPoint(InvasionPoint clone) : base(clone)
                {
                    Priority = clone.Priority;
                }

                internal InvasionPoint(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    Priority = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt32(Priority);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class EnvironmentMapPoint : Region
            {
                internal override RegionType Type => RegionType.EnvironmentMapPoint;

                /// <summary>
                /// Unknown. Only ever 1 bit set, so probably flags.
                /// </summary>
                public int UnkFlags { get; set; }

                /// <summary>
                /// Creates a new EnvironmentMapPoint with the given name.
                /// </summary>
                public EnvironmentMapPoint(string name) : base(name, true)
                {
                    UnkFlags = 0;
                }

                /// <summary>
                /// Creates a new EnvironmentMapPoint with values copied from another.
                /// </summary>
                public EnvironmentMapPoint(EnvironmentMapPoint clone) : base(clone)
                {
                    UnkFlags = clone.UnkFlags;
                }

                internal EnvironmentMapPoint(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    UnkFlags = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt32(UnkFlags);
                }
            }

            /// <summary>
            /// A region that plays a sound while you're in it.
            /// </summary>
            public class Sound : Region
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
                    /// Boss fight music.
                    /// </summary>
                    BGM = 6,

                    /// <summary>
                    /// Character voices.
                    /// </summary>
                    Voice = 7,
                }

                internal override RegionType Type => RegionType.Sound;

                /// <summary>
                /// Type of sound in this region; determines mixing behavior like muffling.
                /// </summary>
                public SndType SoundType { get; set; }

                /// <summary>
                /// ID of the sound to play in this region, or 0 for child regions.
                /// </summary>
                public int SoundID { get; set; }

                /// <summary>
                /// Names of other Sound regions which extend this one.
                /// </summary>
                [MSBReference(ReferenceType = typeof(Region))]
                public string[] ChildRegionNames { get; private set; }
                private int[] ChildRegionIndices;

                /// <summary>
                /// Creates a new Sound with the given name.
                /// </summary>
                public Sound(string name) : base(name, true)
                {
                    SoundType = SndType.Environment;
                    SoundID = 0;
                    ChildRegionNames = new string[16];
                }

                /// <summary>
                /// Creates a new Sound region with values copied from another.
                /// </summary>
                public Sound(Sound clone) : base(clone)
                {
                    SoundType = clone.SoundType;
                    SoundID = clone.SoundID;
                    ChildRegionNames = (string[])clone.ChildRegionNames.Clone();
                }

                internal Sound(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    SoundType = br.ReadEnum32<SndType>();
                    SoundID = br.ReadInt32();
                    ChildRegionIndices = br.ReadInt32s(16);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteUInt32((uint)SoundType);
                    bw.WriteInt32(SoundID);
                    bw.WriteInt32s(ChildRegionIndices);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    ChildRegionNames = MSB.FindNames(entries.Regions, ChildRegionIndices);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    ChildRegionIndices = MSB.FindIndices(entries.Regions, ChildRegionNames);
                }
            }

            /// <summary>
            /// A region that plays a special effect.
            /// </summary>
            public class SFX : Region
            {
                internal override RegionType Type => RegionType.SFX;

                /// <summary>
                /// The ID of the .fxr file to play in this region.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// If true, the effect is off by default until enabled by event scripts.
                /// </summary>
                public bool StartDisabled { get; set; }

                /// <summary>
                /// Creates a new SFX with the given name.
                /// </summary>
                public SFX(string name) : base(name, true)
                {
                    FFXID = -1;
                    StartDisabled = false;
                }

                /// <summary>
                /// Creates a new SFX with values copied from another.
                /// </summary>
                public SFX(SFX clone) : base(clone)
                {
                    FFXID = clone.FFXID;
                    StartDisabled = clone.StartDisabled;
                }

                internal SFX(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    FFXID = br.ReadInt32();
                    // These are not additional FFX IDs, I checked
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    StartDisabled = br.AssertInt32(0, 1) == 1;
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt32(FFXID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(StartDisabled ? 1 : 0);
                }
            }

            /// <summary>
            /// Unknown exactly what this does.
            /// </summary>
            public class WindSFX : Region
            {
                internal override RegionType Type => RegionType.WindSFX;

                /// <summary>
                /// ID of an .fxr file.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// Name of a corresponding WindArea region.
                /// </summary>
                [MSBReference(ReferenceType = typeof(WindArea))]
                public string WindAreaName { get; set; }
                private int WindAreaIndex;

                /// <summary>
                /// Creates a new WindSFX with the given name.
                /// </summary>
                public WindSFX(string name) : base(name, true)
                {
                    FFXID = -1;
                    WindAreaName = null;
                }

                /// <summary>
                /// Creates a new WindSFX with values copied from another.
                /// </summary>
                public WindSFX(WindSFX clone) : base(clone)
                {
                    FFXID = clone.FFXID;
                    WindAreaName = clone.WindAreaName;
                }

                internal WindSFX(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    FFXID = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    WindAreaIndex = br.ReadInt32();
                    br.AssertSingle(-1);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt32(FFXID);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(WindAreaIndex);
                    bw.WriteSingle(-1);
                }

                internal override void GetNames(MSB3 msb, Entries entries)
                {
                    base.GetNames(msb, entries);
                    WindAreaName = MSB.FindName(entries.Regions, WindAreaIndex);
                }

                internal override void GetIndices(MSB3 msb, Entries entries)
                {
                    base.GetIndices(msb, entries);
                    WindAreaIndex = MSB.FindIndex(entries.Regions, WindAreaName);
                }
            }

            /// <summary>
            /// A region where players enter the map.
            /// </summary>
            public class SpawnPoint : Region
            {
                internal override RegionType Type => RegionType.SpawnPoint;

                /// <summary>
                /// Unknown; seems kind of like a region index, but also kind of doesn't.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a new SpawnPoint with the given name.
                /// </summary>
                public SpawnPoint(string name) : base(name, true)
                {
                    UnkT00 = -1;
                }

                /// <summary>
                /// Creates a new SpawnPoint with values copied from another.
                /// </summary>
                public SpawnPoint(SpawnPoint clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                }

                internal SpawnPoint(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// An orange developer message.
            /// </summary>
            public class Message : Region
            {
                internal override RegionType Type => RegionType.Message;

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

                /// <summary>
                /// Creates a new Message with the given name.
                /// </summary>
                public Message(string name) : base(name, true)
                {
                    MessageID = -1;
                    UnkT02 = 0;
                    Hidden = false;
                }

                /// <summary>
                /// Creates a new Message with values copied from another.
                /// </summary>
                public Message(Message clone) : base(clone)
                {
                    MessageID = clone.MessageID;
                    UnkT02 = clone.UnkT02;
                    Hidden = clone.Hidden;
                }

                internal Message(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    MessageID = br.ReadInt16();
                    UnkT02 = br.ReadInt16();
                    Hidden = br.AssertInt32(0, 1) == 1;
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt16(MessageID);
                    bw.WriteInt16(UnkT02);
                    bw.WriteInt32(Hidden ? 1 : 0);
                }
            }

            /// <summary>
            /// A point in a WalkRoute.
            /// </summary>
            public class WalkRoute : SimpleRegion
            {
                internal override RegionType Type => RegionType.WalkRoute;

                /// <summary>
                /// Creates a new WalkRoute with the given name.
                /// </summary>
                public WalkRoute(string name) : base(name) { }

                /// <summary>
                /// Creates a new WalkRoute with values copied from another.
                /// </summary>
                public WalkRoute(WalkRoute clone) : base(clone) { }

                internal WalkRoute(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Unk12 : SimpleRegion
            {
                internal override RegionType Type => RegionType.Unk12;

                /// <summary>
                /// Creates a new Unk12 with the given name.
                /// </summary>
                public Unk12(string name) : base(name) { }

                /// <summary>
                /// Creates a new Unk12 with values copied from another.
                /// </summary>
                public Unk12(Unk12 clone) : base(clone) { }

                internal Unk12(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown; seems to be used for moving enemies around.
            /// </summary>
            public class WarpPoint : SimpleRegion
            {
                internal override RegionType Type => RegionType.WarpPoint;

                /// <summary>
                /// Creates a new WarpPoint with the given name.
                /// </summary>
                public WarpPoint(string name) : base(name) { }

                /// <summary>
                /// Creates a new WarpPoint with values copied from another.
                /// </summary>
                public WarpPoint(WarpPoint clone) : base(clone) { }

                internal WarpPoint(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Triggers an enemy when entered.
            /// </summary>
            public class ActivationArea : SimpleRegion
            {
                internal override RegionType Type => RegionType.ActivationArea;

                /// <summary>
                /// Creates a new ActivationArea with the given name.
                /// </summary>
                public ActivationArea(string name) : base(name) { }

                /// <summary>
                /// Creates a new ActivationArea with values copied from another.
                /// </summary>
                public ActivationArea(ActivationArea clone) : base(clone) { }

                internal ActivationArea(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Any kind of region for use with event scripts.
            /// </summary>
            public class Event : SimpleRegion
            {
                internal override RegionType Type => RegionType.Event;

                /// <summary>
                /// Creates a new Event with the given name.
                /// </summary>
                public Event(string name) : base(name) { }

                /// <summary>
                /// Creates a new Event with values copied from another.
                /// </summary>
                public Event(Event clone) : base(clone) { }

                internal Event(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class EnvironmentMapEffectBox : Region
            {
                internal override RegionType Type => RegionType.EnvironmentMapEffectBox;

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float Compare { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT09 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT0A { get; set; }

                /// <summary>
                /// Creates a new EnvironmentMapEffectBox with the given name.
                /// </summary>
                public EnvironmentMapEffectBox(string name) : base(name, true)
                {
                    UnkT00 = 0;
                    Compare = 0;
                    UnkT08 = false;
                    UnkT09 = 0;
                    UnkT0A = 0;
                }

                /// <summary>
                /// Creates a new EnvironmentMapEffectBox with values copied from another.
                /// </summary>
                public EnvironmentMapEffectBox(EnvironmentMapEffectBox clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                    Compare = clone.Compare;
                    UnkT08 = clone.UnkT08;
                    UnkT09 = clone.UnkT09;
                    UnkT0A = clone.UnkT0A;
                }

                internal EnvironmentMapEffectBox(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadSingle();
                    Compare = br.ReadSingle();
                    UnkT08 = br.ReadBoolean();
                    UnkT09 = br.ReadByte();
                    UnkT0A = br.ReadInt16();
                    br.AssertInt32(0); // float (6)
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteSingle(UnkT00);
                    bw.WriteSingle(Compare);
                    bw.WriteBoolean(UnkT08);
                    bw.WriteByte(UnkT09);
                    bw.WriteInt16(UnkT0A);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Unknown; each WindSFX has a reference to a WindArea.
            /// </summary>
            public class WindArea : SimpleRegion
            {
                internal override RegionType Type => RegionType.WindArea;

                /// <summary>
                /// Creates a new WindArea with the given name.
                /// </summary>
                public WindArea(string name) : base(name) { }

                /// <summary>
                /// Creates a new WindArea with values copied from another.
                /// </summary>
                public WindArea(WindArea clone) : base(clone) { }

                internal WindArea(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Muffles environmental sound while inside it.
            /// </summary>
            public class MufflingBox : Region
            {
                internal override RegionType Type => RegionType.MufflingBox;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a new MufflingBox with the given name.
                /// </summary>
                public MufflingBox(string name) : base(name, true) { }

                /// <summary>
                /// Creates a new MufflingBox with values copied from another.
                /// </summary>
                public MufflingBox(MufflingBox clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                }

                internal MufflingBox(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", 0);
                    bw.WriteInt32(UnkT00);
                }
            }

            /// <summary>
            /// A region leading into a MufflingBox.
            /// </summary>
            public class MufflingPortal : Region
            {
                internal override RegionType Type => RegionType.MufflingPortal;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a new MufflingPortal with the given name.
                /// </summary>
                public MufflingPortal(string name) : base(name, true) { }

                /// <summary>
                /// Creates a new MufflingPortal with values copied from another.
                /// </summary>
                public MufflingPortal(MufflingPortal clone) : base(clone)
                {
                    UnkT00 = clone.UnkT00;
                }

                internal MufflingPortal(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", 0);
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }
        }
    }
}
