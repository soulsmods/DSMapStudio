using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB2
    {
        /// <summary>
        /// Types of region used in DS2.
        /// </summary>
        public enum RegionType : byte
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            Region0 = 0,
            Light = 3,
            StartPoint = 5,
            Sound = 7,
            SFX = 9,
            Wind = 13,
            EnvLight = 14,
            Fog = 15,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Points or volumes that trigger some behavior.
        /// </summary>
        public class PointParam : Param<Region>, IMsbParam<IMsbRegion>
        {
            internal override string Name => "POINT_PARAM_ST";
            internal override int Version => 5;

            /// <summary>
            /// Unknown, possibly walk points for enemies.
            /// </summary>
            public List<Region.Region0> Region0s { get; set; }

            /// <summary>
            /// Unknown if these do anything.
            /// </summary>
            public List<Region.Light> Lights { get; set; }

            /// <summary>
            /// Unknown, presumably the default position for spawning into the map.
            /// </summary>
            public List<Region.StartPoint> StartPoints { get; set; }

            /// <summary>
            /// Sound effects that play in certain areas.
            /// </summary>
            public List<Region.Sound> Sounds { get; set; }

            /// <summary>
            /// Special effects that play at certain areas.
            /// </summary>
            public List<Region.SFX> SFXs { get; set; }

            /// <summary>
            /// Unknown, presumably set wind speed/direction.
            /// </summary>
            public List<Region.Wind> Winds { get; set; }

            /// <summary>
            /// Unknown, names mention lightmaps and GI.
            /// </summary>
            public List<Region.EnvLight> EnvLights { get; set; }

            /// <summary>
            /// Unknown if these do anything.
            /// </summary>
            public List<Region.Fog> Fogs { get; set; }

            /// <summary>
            /// Creates an empty PointParam.
            /// </summary>
            public PointParam()
            {
                Region0s = new List<Region.Region0>();
                Lights = new List<Region.Light>();
                StartPoints = new List<Region.StartPoint>();
                Sounds = new List<Region.Sound>();
                SFXs = new List<Region.SFX>();
                Winds = new List<Region.Wind>();
                EnvLights = new List<Region.EnvLight>();
                Fogs = new List<Region.Fog>();
            }

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                RegionType type = br.GetEnum8<RegionType>(br.Position + 0xA);
                switch (type)
                {
                    case RegionType.Region0:
                        var region0 = new Region.Region0(br);
                        Region0s.Add(region0);
                        return region0;

                    case RegionType.Light:
                        var light = new Region.Light(br);
                        Lights.Add(light);
                        return light;

                    case RegionType.StartPoint:
                        var startPoint = new Region.StartPoint(br);
                        StartPoints.Add(startPoint);
                        return startPoint;

                    case RegionType.Sound:
                        var sound = new Region.Sound(br);
                        Sounds.Add(sound);
                        return sound;

                    case RegionType.SFX:
                        var sfx = new Region.SFX(br);
                        SFXs.Add(sfx);
                        return sfx;

                    case RegionType.Wind:
                        var wind = new Region.Wind(br);
                        Winds.Add(wind);
                        return wind;

                    case RegionType.EnvLight:
                        var envLight = new Region.EnvLight(br);
                        EnvLights.Add(envLight);
                        return envLight;

                    case RegionType.Fog:
                        var fog = new Region.Fog(br);
                        Fogs.Add(fog);
                        return fog;

                    default:
                        throw new NotImplementedException($"Unimplemented region type: {type}");
                }
            }

            /// <summary>
            /// Returns every Region in the order they'll be written.
            /// </summary>
            public override List<Region> GetEntries()
            {
                return SFUtil.ConcatAll<Region>(
                    Region0s, Lights, StartPoints, Sounds, SFXs,
                    Winds, EnvLights, Fogs);
            }
            IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() => GetEntries();

            public void Add(IMsbRegion item)
            {
                switch (item)
                {
                    case Region.Region0 r:
                        Region0s.Add(r);
                        break;
                    case Region.Light r:
                        Lights.Add(r);
                        break;
                    case Region.StartPoint r:
                        StartPoints.Add(r);
                        break;
                    case Region.Sound r:
                        Sounds.Add(r);
                        break;
                    case Region.SFX r:
                        SFXs.Add(r);
                        break;
                    case Region.Wind r:
                        Winds.Add(r);
                        break;
                    case Region.EnvLight r:
                        EnvLights.Add(r);
                        break;
                    case Region.Fog r:
                        Fogs.Add(r);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        /// <summary>
        /// A point or volume that triggers some behavior.
        /// </summary>
        public abstract class Region : NamedEntry, IMsbRegion
        {
            /// <summary>
            /// The specific type of this region.
            /// </summary>
            public abstract RegionType Type { get; }

            internal abstract bool HasTypeData { get; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short Unk08 { get; set; }

            /// <summary>
            /// Describes the space encompassed by the region.
            /// </summary>
            public MSB.Shape Shape { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short Unk0E { get; set; }

            /// <summary>
            /// Location of the region.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            internal Region(string name = "")
            {
                Name = name;
                Shape = new MSB.Shape.Point();
            }

            internal Region(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                Unk08 = br.ReadInt16();
                br.AssertByte((byte)Type);
                ShapeType shapeType = br.ReadEnum8<ShapeType>();
                br.ReadInt16(); // Index
                Unk0E = br.ReadInt16();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                long unkOffsetA = br.ReadInt64();
                long unkOffsetB = br.ReadInt64();
                br.AssertInt32(-1);
                br.AssertPattern(0x24, 0x00);
                long shapeDataOffset = br.ReadInt64();
                long typeDataOffset = br.ReadInt64();
                br.AssertInt64(0);
                br.AssertInt64(0);

                Name = br.GetUTF16(start + nameOffset);

                br.Position = start + unkOffsetA;
                br.AssertInt32(0);

                br.Position = start + unkOffsetB;
                br.AssertInt32(0);

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

                    default:
                        throw new NotImplementedException($"Unimplemented shape type: {shapeType}");
                }

                if (HasTypeData)
                {
                    br.Position = start + typeDataOffset;
                    ReadTypeData(br);
                }
            }

            internal virtual void ReadTypeData(BinaryReaderEx br)
            {
                throw new InvalidOperationException("Type data should not be read for regions with no type data.");
            }

            internal override void Write(BinaryWriterEx bw, int index)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteInt16(Unk08);
                bw.WriteByte((byte)Type);
                bw.WriteByte((byte)Shape.Type);
                bw.WriteInt16((short)index);
                bw.WriteInt16(Unk0E);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.ReserveInt64("UnkOffsetA");
                bw.ReserveInt64("UnkOffsetB");
                bw.WriteInt32(-1);
                bw.WritePattern(0x24, 0x00);
                bw.ReserveInt64("ShapeDataOffset");
                bw.ReserveInt64("TypeDataOffset");
                bw.WriteInt64(0);
                bw.WriteInt64(0);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.Pad(4);

                bw.FillInt64("UnkOffsetA", bw.Position - start);
                bw.WriteInt32(0);

                bw.FillInt64("UnkOffsetB", bw.Position - start);
                bw.WriteInt32(0);
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
                throw new InvalidOperationException("Type data should not be written for regions with no type data.");
            }

            /// <summary>
            /// Returns a string representation of the region.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {Shape.Type} \"{Name}\"";
            }

            /// <summary>
            /// Unknown, names always seem to mention enemies; possibly walk points.
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
                public Region0(string name = "") : base(name) { }

                internal Region0(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown if this does anything.
            /// </summary>
            public class Light : Region
            {
                /// <summary>
                /// RegionType.Light
                /// </summary>
                public override RegionType Type => RegionType.Light;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public Color ColorT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public Color ColorT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT0C { get; set; }

                /// <summary>
                /// Creates a Light with default values.
                /// </summary>
                public Light(string name = "") : base(name) { }

                internal Light(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    ColorT04 = br.ReadRGBA();
                    ColorT08 = br.ReadRGBA();
                    UnkT0C = br.ReadSingle();
                    br.AssertPattern(0x14, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteRGBA(ColorT04);
                    bw.WriteRGBA(ColorT08);
                    bw.WriteSingle(UnkT0C);
                    bw.WritePattern(0x14, 0x00);
                }
            }

            /// <summary>
            /// Unknown, presumably the default spawn location for a map.
            /// </summary>
            public class StartPoint : Region
            {
                /// <summary>
                /// RegionType.StartPoint
                /// </summary>
                public override RegionType Type => RegionType.StartPoint;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a StartPoint with default values.
                /// </summary>
                public StartPoint(string name = "") : base(name) { }

                internal StartPoint(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A sound effect that plays in a certain area.
            /// </summary>
            public class Sound : Region
            {
                /// <summary>
                /// RegionType.Sound
                /// </summary>
                public override RegionType Type => RegionType.Sound;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown; possibly sound type.
                /// </summary>
                public int UnkT00 { get; set; }

                /// <summary>
                /// ID of the sound to play.
                /// </summary>
                public int SoundID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT08 { get; set; }

                /// <summary>
                /// Creates a Sound with default values.
                /// </summary>
                public Sound(string name = "") : base(name) { }

                internal Sound(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    SoundID = br.ReadInt32();
                    UnkT08 = br.ReadInt32();
                    br.AssertPattern(0x14, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(SoundID);
                    bw.WriteInt32(UnkT08);
                    bw.WritePattern(0x14, 0x00);
                }
            }

            /// <summary>
            /// A special effect that plays at a certain region.
            /// </summary>
            public class SFX : Region
            {
                /// <summary>
                /// RegionType.SFX
                /// </summary>
                public override RegionType Type => RegionType.SFX;

                internal override bool HasTypeData => true;

                /// <summary>
                /// The effect to play at this region.
                /// </summary>
                public int EffectID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int UnkT04 { get; set; }

                /// <summary>
                /// Creates an SFX with default values.
                /// </summary>
                public SFX(string name = "") : base(name) { }

                internal SFX(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    EffectID = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertPattern(0x18, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(EffectID);
                    bw.WriteInt32(UnkT04);
                    bw.WritePattern(0x18, 0x00);
                }
            }

            /// <summary>
            /// Unknown, presumably sets wind speed/direction.
            /// </summary>
            public class Wind : Region
            {
                /// <summary>
                /// RegionType.Wind
                /// </summary>
                public override RegionType Type => RegionType.Wind;

                internal override bool HasTypeData => true;

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
                /// Unknown.
                /// </summary>
                public float UnkT18 { get; set; }

                /// <summary>
                /// Creates a Wind with default values.
                /// </summary>
                public Wind(string name = "") : base(name) { }

                internal Wind(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadSingle();
                    UnkT08 = br.ReadSingle();
                    UnkT0C = br.ReadSingle();
                    UnkT10 = br.ReadSingle();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteSingle(UnkT04);
                    bw.WriteSingle(UnkT08);
                    bw.WriteSingle(UnkT0C);
                    bw.WriteSingle(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteSingle(UnkT18);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Unknown, names mention lightmaps and GI.
            /// </summary>
            public class EnvLight : Region
            {
                /// <summary>
                /// RegionType.EnvLight
                /// </summary>
                public override RegionType Type => RegionType.EnvLight;

                internal override bool HasTypeData => true;

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
                /// Creates an EnvLight with default values.
                /// </summary>
                public EnvLight(string name = "") : base(name) { }

                internal EnvLight(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadSingle();
                    UnkT08 = br.ReadSingle();
                    br.AssertPattern(0x14, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteSingle(UnkT04);
                    bw.WriteSingle(UnkT08);
                    bw.WritePattern(0x14, 0x00);
                }
            }

            /// <summary>
            /// Unknown if this does anything.
            /// </summary>
            public class Fog : Region
            {
                /// <summary>
                /// RegionType.Fog
                /// </summary>
                public override RegionType Type => RegionType.Fog;

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
                /// Creates a Fog with default values.
                /// </summary>
                public Fog(string name = "") : base(name) { }

                internal Fog(BinaryReaderEx br) : base(br) { }

                internal override void ReadTypeData(BinaryReaderEx br)
                {
                    UnkT00 = br.ReadInt32();
                    UnkT04 = br.ReadInt32();
                    br.AssertPattern(0x1C, 0x00);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(UnkT00);
                    bw.WriteInt32(UnkT04);
                    bw.WritePattern(0x1C, 0x00);
                }
            }
        }
    }
}
