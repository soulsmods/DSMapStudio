using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum RegionType : uint
        {
            Region0 = 0,
            InvasionPoint = 1,
            EnvironmentMapPoint = 2,
            Sound = 4,
            SFX = 5,
            WindSFX = 6,
            SpawnPoint = 8,
            WalkRoute = 11,
            WarpPoint = 13,
            ActivationArea = 14,
            Event = 15,
            EnvironmentMapEffectBox = 17,
            WindArea = 18,
            MufflingBox = 20,
            MufflingPortal = 21,
            Region23 = 23,
            Region24 = 24,
            PartsGroup = 25,
            AutoDrawGroup = 26,
            Other = 0xFFFFFFFF,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Points and volumes used to trigger various effects.
        /// </summary>
        public class PointParam : Param<Region>, IMsbParam<IMsbRegion>
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.Region0> Region0s { get; set; }

            /// <summary>
            /// Previously points where players will appear when invading; not sure if they do anything in Sekiro.
            /// </summary>
            public List<Region.InvasionPoint> InvasionPoints { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.EnvironmentMapPoint> EnvironmentMapPoints { get; set; }

            /// <summary>
            /// Areas where a sound will play.
            /// </summary>
            public List<Region.Sound> Sounds { get; set; }

            /// <summary>
            /// Points for particle effects to play at.
            /// </summary>
            public List<Region.SFX> SFXs { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.WindSFX> WindSFXs { get; set; }

            /// <summary>
            /// Points where the player can spawn into a map.
            /// </summary>
            public List<Region.SpawnPoint> SpawnPoints { get; set; }

            /// <summary>
            /// Points that describe an NPC patrol path.
            /// </summary>
            public List<Region.WalkRoute> WalkRoutes { get; set; }

            /// <summary>
            /// Regions for warping the player.
            /// </summary>
            public List<Region.WarpPoint> WarpPoints { get; set; }

            /// <summary>
            /// Regions that trigger enemies when entered.
            /// </summary>
            public List<Region.ActivationArea> ActivationAreas { get; set; }

            /// <summary>
            /// Generic regions for use with event scripts.
            /// </summary>
            public List<Region.Event> Events { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.EnvironmentMapEffectBox> EnvironmentMapEffectBoxes { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.WindArea> WindAreas { get; set; }

            /// <summary>
            /// Areas where sound is muffled.
            /// </summary>
            public List<Region.MufflingBox> MufflingBoxes { get; set; }

            /// <summary>
            /// Entrances to muffling boxes.
            /// </summary>
            public List<Region.MufflingPortal> MufflingPortals { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.Region23> Region23s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.Region24> Region24s { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.PartsGroup> PartsGroups { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.AutoDrawGroup> AutoDrawGroups { get; set; }

            /// <summary>
            /// Some sort of generic region that is almost never used.
            /// </summary>
            public List<Region.Other> Others { get; set; }

            /// <summary>
            /// Creates an empty PointParam with the given version.
            /// </summary>
            public PointParam(int unk00 = 0x23) : base(unk00, "POINT_PARAM_ST")
            {
                Region0s = new List<Region.Region0>();
                InvasionPoints = new List<Region.InvasionPoint>();
                EnvironmentMapPoints = new List<Region.EnvironmentMapPoint>();
                Sounds = new List<Region.Sound>();
                SFXs = new List<Region.SFX>();
                WindSFXs = new List<Region.WindSFX>();
                SpawnPoints = new List<Region.SpawnPoint>();
                WalkRoutes = new List<Region.WalkRoute>();
                WarpPoints = new List<Region.WarpPoint>();
                ActivationAreas = new List<Region.ActivationArea>();
                Events = new List<Region.Event>();
                EnvironmentMapEffectBoxes = new List<Region.EnvironmentMapEffectBox>();
                WindAreas = new List<Region.WindArea>();
                MufflingBoxes = new List<Region.MufflingBox>();
                MufflingPortals = new List<Region.MufflingPortal>();
                Region23s = new List<Region.Region23>();
                Region24s = new List<Region.Region24>();
                PartsGroups = new List<Region.PartsGroup>();
                AutoDrawGroups = new List<Region.AutoDrawGroup>();
                Others = new List<Region.Other>();
            }

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                RegionType type = br.GetEnum32<RegionType>(br.Position + 8);
                switch (type)
                {
                    case RegionType.Region0:
                        var region0 = new Region.Region0(br);
                        Region0s.Add(region0);
                        return region0;

                    case RegionType.InvasionPoint:
                        var invasionPoint = new Region.InvasionPoint(br);
                        InvasionPoints.Add(invasionPoint);
                        return invasionPoint;

                    case RegionType.EnvironmentMapPoint:
                        var environmentMapPoint = new Region.EnvironmentMapPoint(br);
                        EnvironmentMapPoints.Add(environmentMapPoint);
                        return environmentMapPoint;

                    case RegionType.Sound:
                        var sound = new Region.Sound(br);
                        Sounds.Add(sound);
                        return sound;

                    case RegionType.SFX:
                        var sfx = new Region.SFX(br);
                        SFXs.Add(sfx);
                        return sfx;

                    case RegionType.WindSFX:
                        var windSFX = new Region.WindSFX(br);
                        WindSFXs.Add(windSFX);
                        return windSFX;

                    case RegionType.SpawnPoint:
                        var spawnPoint = new Region.SpawnPoint(br);
                        SpawnPoints.Add(spawnPoint);
                        return spawnPoint;

                    case RegionType.WalkRoute:
                        var walkRoute = new Region.WalkRoute(br);
                        WalkRoutes.Add(walkRoute);
                        return walkRoute;

                    case RegionType.WarpPoint:
                        var warpPoint = new Region.WarpPoint(br);
                        WarpPoints.Add(warpPoint);
                        return warpPoint;

                    case RegionType.ActivationArea:
                        var activationArea = new Region.ActivationArea(br);
                        ActivationAreas.Add(activationArea);
                        return activationArea;

                    case RegionType.Event:
                        var evt = new Region.Event(br);
                        Events.Add(evt);
                        return evt;

                    case RegionType.EnvironmentMapEffectBox:
                        var environmentMapEffectBox = new Region.EnvironmentMapEffectBox(br);
                        EnvironmentMapEffectBoxes.Add(environmentMapEffectBox);
                        return environmentMapEffectBox;

                    case RegionType.WindArea:
                        var windArea = new Region.WindArea(br);
                        WindAreas.Add(windArea);
                        return windArea;

                    case RegionType.MufflingBox:
                        var mufflingBox = new Region.MufflingBox(br);
                        MufflingBoxes.Add(mufflingBox);
                        return mufflingBox;

                    case RegionType.MufflingPortal:
                        var mufflingPortal = new Region.MufflingPortal(br);
                        MufflingPortals.Add(mufflingPortal);
                        return mufflingPortal;

                    case RegionType.Region23:
                        var region23 = new Region.Region23(br);
                        Region23s.Add(region23);
                        return region23;

                    case RegionType.Region24:
                        var region24 = new Region.Region24(br);
                        Region24s.Add(region24);
                        return region24;

                    case RegionType.PartsGroup:
                        var partsGroup = new Region.PartsGroup(br);
                        PartsGroups.Add(partsGroup);
                        return partsGroup;

                    case RegionType.AutoDrawGroup:
                        var autoDrawGroup = new Region.AutoDrawGroup(br);
                        AutoDrawGroups.Add(autoDrawGroup);
                        return autoDrawGroup;

                    case RegionType.Other:
                        var other = new Region.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unimplemented region type: {type}");
                }
            }

            /// <summary>
            /// Returns every region in the order they'll be written.
            /// </summary>
            public override List<Region> GetEntries()
            {
                return SFUtil.ConcatAll<Region>(
                    InvasionPoints, EnvironmentMapPoints, Sounds, SFXs, WindSFXs,
                    SpawnPoints, WalkRoutes, WarpPoints, ActivationAreas, Events,
                    Region0s, EnvironmentMapEffectBoxes, WindAreas, MufflingBoxes, MufflingPortals,
                    Region23s, Region24s, PartsGroups, AutoDrawGroups, Others);
            }
            IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() => GetEntries();

            public void Add(IMsbRegion item)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A point or volume that triggers some sort of interaction.
        /// </summary>
        public abstract class Region : Entry, IMsbRegion
        {
            /// <summary>
            /// The specific type of the region.
            /// </summary>
            public abstract RegionType Type { get; }

            internal abstract bool HasTypeData { get; }

            /// <summary>
            /// The shape of the region.
            /// </summary>
            public MSB.Shape Shape { get; set; }

            /// <summary>
            /// The location of the region.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// The rotiation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk2C { get; set; }

            /// <summary>
            /// Controls whether the region is active in different ceremonies.
            /// </summary>
            public uint MapStudioLayer { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<short> UnkA { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<short> UnkB { get; set; }

            /// <summary>
            /// If specified, the region is only active when the part is loaded.
            /// </summary>
            public string ActivationPartName { get; set; }
            private int ActivationPartIndex;

            /// <summary>
            /// Identifies the region in event scripts.
            /// </summary>
            public int EntityID { get; set; }

            internal Region()
            {
                Name = "";
                Shape = new MSB.Shape.Point();
                MapStudioLayer = 0xFFFFFFFF;
                UnkA = new List<short>();
                UnkB = new List<short>();
                EntityID = -1;
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
                Unk2C = br.ReadInt32();
                long baseDataOffset1 = br.ReadInt64();
                long baseDataOffset2 = br.ReadInt64();
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

                    case ShapeType.Rect:
                        Shape = new MSB.Shape.Rect(br);
                        break;

                    case ShapeType.Box:
                        Shape = new MSB.Shape.Box(br);
                        break;

                    case ShapeType.Composite:
                        Shape = new MSB.Shape.Composite(br);
                        break;

                    default:
                        throw new NotImplementedException($"Unimplemented shape type: {shapeType}");
                }

                br.Position = start + baseDataOffset3;
                ActivationPartIndex = br.ReadInt32();
                EntityID = br.ReadInt32();
                br.Position = start + typeDataOffset;
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WriteUInt32((uint)Shape.Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteInt32(Unk2C);
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
                {
                    if (Type == RegionType.Region23 || Type == RegionType.PartsGroup || Type == RegionType.AutoDrawGroup)
                        bw.Pad(8);

                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    WriteTypeData(bw);
                }
                else
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
                bw.Pad(8);
            }

            internal virtual void WriteTypeData(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Type data should not be written for regions with no type data.");
            }

            internal virtual void GetNames(Entries entries)
            {
                ActivationPartName = MSB.FindName(entries.Parts, ActivationPartIndex);
                if (Shape is MSB.Shape.Composite composite)
                {
                    foreach (MSB.Shape.Composite.Child child in composite.Children)
                        child.GetNames(entries);
                }
            }

            internal virtual void GetIndices(Entries entries)
            {
                ActivationPartIndex = MSB.FindIndex(entries.Parts, ActivationPartName);
                if (Shape is MSB.Shape.Composite composite)
                {
                    foreach (MSB.Shape.Composite.Child child in composite.Children)
                        child.GetIndices(entries);
                }
            }

            /// <summary>
            /// Returns the type, shape type, and name of the region as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Type} {Shape.Type} {Name}";
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Region0 : Region
            {
                /// <summary>
                /// RegionType.Region0
                /// </summary>
                public override RegionType Type => RegionType.Region0;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a Region0 with default values.
                /// </summary>
                public Region0() : base() { }

                internal Region0(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A point where a player can invade your world.
            /// </summary>
            public class InvasionPoint : Region
            {
                /// <summary>
                /// RegionType.InvasionPoint
                /// </summary>
                public override RegionType Type => RegionType.InvasionPoint;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates an InvasionPoint with default values.
                /// </summary>
                public InvasionPoint() : base() { }

                internal InvasionPoint(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class EnvironmentMapPoint : Region
            {
                /// <summary>
                /// RegionType.EnvironmentMapPoint
                /// </summary>
                public override RegionType Type => RegionType.EnvironmentMapPoint;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT0C { get; set; }

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
                public int UnkT18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT1C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT20 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT28 { get; set; }

                /// <summary>
                /// Creates an EnvironmentMapPoint with default values.
                /// </summary>
                public EnvironmentMapPoint() : base() { }

                internal EnvironmentMapPoint(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadSingle();
                    UnkT04 = br.ReadInt32();
                    br.AssertInt32(-1);
                    UnkT0C = br.ReadInt32();
                    UnkT10 = br.ReadSingle();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadInt32();
                    UnkT1C = br.ReadInt32();
                    UnkT20 = br.ReadInt32();
                    UnkT24 = br.ReadInt32();
                    UnkT28 = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertPattern(0x10, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(UnkT00);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(UnkT0C);
                    bw.WriteSingle(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteInt32(UnkT18);
                    bw.WriteInt32(UnkT1C);
                    bw.WriteInt32(UnkT20);
                    bw.WriteInt32(UnkT24);
                    bw.WriteInt32(UnkT28);
                    bw.WriteInt32(-1);
                    bw.WritePattern(0x10, 0x00);
                }
            }

            /// <summary>
            /// An area where a sound plays.
            /// </summary>
            public class Sound : Region
            {
                /// <summary>
                /// RegionType.Sound
                /// </summary>
                public override RegionType Type => RegionType.Sound;

                internal override bool HasTypeData => true;

                /// <summary>
                /// The category of the sound.
                /// </summary>
                public int SoundType { get; set; }

                /// <summary>
                /// The ID of the sound.
                /// </summary>
                public int SoundID { get; set; }

                /// <summary>
                /// References to other regions used to build a composite shape.
                /// </summary>
                public string[] ChildRegionNames { get; private set; }
                private int[] ChildRegionIndices;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT48 { get; set; }

                /// <summary>
                /// Creates a Sound with default values.
                /// </summary>
                public Sound() : base()
                {
                    ChildRegionNames = new string[16];
                }

                internal Sound(BinaryReaderEx br) : base(br)
                {
                    SoundType = br.ReadInt32();
                    SoundID = br.ReadInt32();
                    ChildRegionIndices = br.ReadInt32s(16);
                    UnkT48 = br.ReadInt32();
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(SoundType);
                    bw.WriteInt32(SoundID);
                    bw.WriteInt32s(ChildRegionIndices);
                    bw.WriteInt32(UnkT48);
                }

                internal override void GetNames(Entries entries)
                {
                    base.GetNames(entries);
                    ChildRegionNames = MSB.FindNames(entries.Regions, ChildRegionIndices);
                }

                internal override void GetIndices(Entries entries)
                {
                    base.GetIndices(entries);
                    ChildRegionIndices = MSB.FindIndices(entries.Regions, ChildRegionNames);
                }
            }

            /// <summary>
            /// A point where a particle effect can play.
            /// </summary>
            public class SFX : Region
            {
                /// <summary>
                /// RegionType.SFX
                /// </summary>
                public override RegionType Type => RegionType.SFX;

                internal override bool HasTypeData => true;

                /// <summary>
                /// The ID of the particle effect FFX.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT04 { get; set; }

                /// <summary>
                /// Whether the effect is off until activated.
                /// </summary>
                public int StartDisabled { get; set; }

                /// <summary>
                /// Creates an SFX with default values.
                /// </summary>
                public SFX() : base() { }

                internal SFX(BinaryReaderEx br) : base(br)
                {
                    FFXID = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    br.AssertInt32(-1);
                    StartDisabled = br.ReadInt32();
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(FFXID);
                    bw.WriteInt32(UnkT04);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(-1);
                    bw.WriteInt32(StartDisabled);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class WindSFX : Region
            {
                /// <summary>
                /// RegionType.WindSFX
                /// </summary>
                public override RegionType Type => RegionType.WindSFX;

                internal override bool HasTypeData => true;

                /// <summary>
                /// ID of the effect FFX.
                /// </summary>
                public int FFXID { get; set; }

                /// <summary>
                /// Reference to a WindArea region.
                /// </summary>
                public string WindAreaName;
                private int WindAreaIndex;

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT18 { get; set; }

                /// <summary>
                /// Creates a WindSFX with default values.
                /// </summary>
                public WindSFX() : base() { }

                internal WindSFX(BinaryReaderEx br) : base(br)
                {
                    FFXID = br.ReadInt32();
                    br.AssertPattern(0x10, 0xFF);
                    WindAreaIndex = br.ReadInt32();
                    UnkT18 = br.ReadSingle();
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(FFXID);
                    bw.WritePattern(0x10, 0xFF);
                    bw.WriteInt32(WindAreaIndex);
                    bw.WriteSingle(UnkT18);
                    bw.WriteInt32(0);
                }

                internal override void GetNames(Entries entries)
                {
                    base.GetNames(entries);
                    WindAreaName = MSB.FindName(entries.Regions, WindAreaIndex);
                }

                internal override void GetIndices(Entries entries)
                {
                    base.GetIndices(entries);
                    WindAreaIndex = MSB.FindIndex(entries.Regions, WindAreaName);
                }
            }

            /// <summary>
            /// A point where the player can spawn into the map.
            /// </summary>
            public class SpawnPoint : Region
            {
                /// <summary>
                /// RegionType.SpawnPoint
                /// </summary>
                public override RegionType Type => RegionType.SpawnPoint;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Creates a SpawnPoint with default values.
                /// </summary>
                public SpawnPoint() : base() { }

                internal SpawnPoint(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt32(-1);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(-1);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A point along an NPC patrol path.
            /// </summary>
            public class WalkRoute : Region
            {
                /// <summary>
                /// RegionType.WalkRoute
                /// </summary>
                public override RegionType Type => RegionType.WalkRoute;

                internal override bool HasTypeData => false;
                
                /// <summary>
                /// Creates a WalkRoute with default values.
                /// </summary>
                public WalkRoute() : base() { }

                internal WalkRoute(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A point the player can be warped to.
            /// </summary>
            public class WarpPoint : Region
            {
                /// <summary>
                /// RegionType.WarpPoint
                /// </summary>
                public override RegionType Type => RegionType.WarpPoint;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a WarpPoint with default values.
                /// </summary>
                public WarpPoint() : base() { }

                internal WarpPoint(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// An area that triggers enemies when entered.
            /// </summary>
            public class ActivationArea : Region
            {
                /// <summary>
                /// RegionType.ActivationArea
                /// </summary>
                public override RegionType Type => RegionType.ActivationArea;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates an ActivationArea with default values.
                /// </summary>
                public ActivationArea() : base() { }

                internal ActivationArea(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A generic area used by event scripts.
            /// </summary>
            public class Event : Region
            {
                /// <summary>
                /// RegionType.Event
                /// </summary>
                public override RegionType Type => RegionType.Event;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates an Event with default values.
                /// </summary>
                public Event() : base() { }

                internal Event(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class EnvironmentMapEffectBox : Region
            {
                /// <summary>
                /// RegionType.EnvironmentMapEffectBox
                /// </summary>
                public override RegionType Type => RegionType.EnvironmentMapEffectBox;

                internal override bool HasTypeData => true;

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
                public byte UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte UnkT09 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short UnkT0A { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT24 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT28 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT2C { get; set; }

                /// <summary>
                /// Creates an EnvironmentMapEffectBox with default values.
                /// </summary>
                public EnvironmentMapEffectBox() : base() { }

                internal EnvironmentMapEffectBox(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadSingle();
                    Compare = br.ReadSingle();
                    UnkT08 = br.ReadByte();
                    UnkT09 = br.ReadByte();
                    UnkT0A = br.ReadInt16();
                    br.AssertPattern(0x18, 0x00);
                    UnkT24 = br.ReadInt32();
                    UnkT28 = br.ReadSingle();
                    UnkT2C = br.ReadSingle();
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(UnkT00);
                    bw.WriteSingle(Compare);
                    bw.WriteByte(UnkT08);
                    bw.WriteByte(UnkT09);
                    bw.WriteInt16(UnkT0A);
                    bw.WritePattern(0x18, 0x00);
                    bw.WriteInt32(UnkT24);
                    bw.WriteSingle(UnkT28);
                    bw.WriteSingle(UnkT2C);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class WindArea : Region
            {
                /// <summary>
                /// RegionType.WindArea
                /// </summary>
                public override RegionType Type => RegionType.WindArea;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a WindArea with default values.
                /// </summary>
                public WindArea() : base() { }

                internal WindArea(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// An area where sound is muffled.
            /// </summary>
            public class MufflingBox : Region
            {
                /// <summary>
                /// RegionType.MufflingBox
                /// </summary>
                public override RegionType Type => RegionType.MufflingBox;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a MufflingBox with default values.
                /// </summary>
                public MufflingBox() : base() { }

                internal MufflingBox(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                }
            }

            /// <summary>
            /// An entrance to a muffling box.
            /// </summary>
            public class MufflingPortal : Region
            {
                /// <summary>
                /// RegionType.MufflingPortal
                /// </summary>
                public override RegionType Type => RegionType.MufflingPortal;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Creates a MufflingPortal with default values.
                /// </summary>
                public MufflingPortal() : base() { }

                internal MufflingPortal(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt32();
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Region23 : Region
            {
                /// <summary>
                /// RegionType.Region23
                /// </summary>
                public override RegionType Type => RegionType.Region23;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public long UnkT00 { get; set; }

                /// <summary>
                /// Creates a Region23 with default values.
                /// </summary>
                public Region23() : base() { }

                internal Region23(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt64();
                    br.AssertPattern(0x18, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt64(UnkT00);
                    bw.WritePattern(0x18, 0x00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Region24 : Region
            {
                /// <summary>
                /// RegionType.Region24
                /// </summary>
                public override RegionType Type => RegionType.Region24;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a Region24 with default values.
                /// </summary>
                public Region24() : base() { }

                internal Region24(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class PartsGroup : Region
            {
                /// <summary>
                /// RegionType.PartsGroup
                /// </summary>
                public override RegionType Type => RegionType.PartsGroup;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public long UnkT00 { get; set; }

                /// <summary>
                /// Creates a PartsGroup with default values.
                /// </summary>
                public PartsGroup() : base() { }

                internal PartsGroup(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt64();
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt64(UnkT00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class AutoDrawGroup : Region
            {
                /// <summary>
                /// RegionType.AutoDrawGroup
                /// </summary>
                public override RegionType Type => RegionType.AutoDrawGroup;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public long UnkT00 { get; set; }

                /// <summary>
                /// Creates an AutoDrawGroup with default values.
                /// </summary>
                public AutoDrawGroup() : base() { }

                internal AutoDrawGroup(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadInt64();
                    br.AssertPattern(0x18, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt64(UnkT00);
                    bw.WritePattern(0x18, 0x00);
                }
            }

            /// <summary>
            /// A rarely used generic type of region.
            /// </summary>
            public class Other : Region
            {
                /// <summary>
                /// RegionType.Other
                /// </summary>
                public override RegionType Type => RegionType.Other;

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
