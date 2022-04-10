using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// Point light sources in a map, used in BB, DS3, and Sekiro.
    /// </summary>
    public class BTL : SoulsFile<BTL>
    {
        /// <summary>
        /// Indicates the version, probably.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Whether offsets are 64-bit; set to false for Dark Souls 2.
        /// </summary>
        public bool LongOffsets { get; set; }

        /// <summary>
        /// Light sources in this BTL.
        /// </summary>
        public List<Light> Lights { get; set; }

        /// <summary>
        /// Creates a BTL with Sekiro's version and no lights.
        /// </summary>
        public BTL()
        {
            Version = 16;
            LongOffsets = true;
            Lights = new List<Light>();
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertInt32(2);
            Version = br.AssertInt32(1, 2, 5, 6, 16, 18);
            int lightCount = br.ReadInt32();
            int namesLength = br.ReadInt32();
            br.AssertInt32(0);
            int lightSize = br.AssertInt32(0xC0, 0xC8, 0xE8);
            br.AssertPattern(0x24, 0x00);
            LongOffsets = br.VarintLong = lightSize != 0xC0;

            long namesStart = br.Position;
            br.Skip(namesLength);
            Lights = new List<Light>(lightCount);
            for (int i = 0; i < lightCount; i++)
                Lights.Add(new Light(br, namesStart, Version, LongOffsets));
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;
            bw.VarintLong = LongOffsets;

            bw.WriteInt32(2);
            bw.WriteInt32(Version);
            bw.WriteInt32(Lights.Count);
            bw.ReserveInt32("NamesLength");
            bw.WriteInt32(0);
            bw.WriteInt32(Version >= 16 ? 0xE8 : (LongOffsets ? 0xC8 : 0xC0));
            bw.WritePattern(0x24, 0x00);

            long namesStart = bw.Position;
            var nameOffsets = new List<long>(Lights.Count);
            foreach (Light entry in Lights)
            {
                long nameOffset = bw.Position - namesStart;
                nameOffsets.Add(nameOffset);
                bw.WriteUTF16(entry.Name, true);
                if (nameOffset % 0x10 != 0)
                    bw.WritePattern((int)(0x10 - (nameOffset % 0x10)), 0x00);
            }

            bw.FillInt32("NamesLength", (int)(bw.Position - namesStart));
            for (int i = 0; i < Lights.Count; i++)
                Lights[i].Write(bw, nameOffsets[i], Version, LongOffsets);
        }

        /// <summary>
        /// Type of a light source.
        /// </summary>
        public enum LightType : uint
        {
            /// <summary>
            /// Omnidirectional light.
            /// </summary>
            Point = 0,

            /// <summary>
            /// Cone of light.
            /// </summary>
            Spot = 1,

            /// <summary>
            /// Light at a constant angle.
            /// </summary>
            Directional = 2,
        }

        /// <summary>
        /// An omnidirectional and/or spot light source.
        /// </summary>
        public class Light
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public byte[] Unk00 { get; private set; }

            /// <summary>
            /// Name of this light.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public LightType Type { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public bool Unk1C { get; set; }

            /// <summary>
            /// Color of the light on diffuse surfaces.
            /// </summary>
            [SupportsAlpha(false)]
            public Color DiffuseColor { get; set; }

            /// <summary>
            /// Intensity of diffuse lighting.
            /// </summary>
            public float DiffusePower { get; set; }

            /// <summary>
            /// Color of the light on reflective surfaces.
            /// </summary>
            [SupportsAlpha(false)]
            public Color SpecularColor { get; set; }

            /// <summary>
            /// Whether the light casts shadows.
            /// </summary>
            public bool CastShadows { get; set; }

            /// <summary>
            /// Intensity of specular lighting.
            /// </summary>
            public float SpecularPower { get; set; }

            /// <summary>
            /// Tightness of the spot light beam.
            /// </summary>
            public float ConeAngle { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk30 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk34 { get; set; }

            /// <summary>
            /// Center of the light.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of a spot light.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk50 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk54 { get; set; }

            /// <summary>
            /// Distance the light shines.
            /// </summary>
            public float Radius { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk5C { get; set; }

            /// <summary>
            /// Unknown; 4 bytes.
            /// </summary>
            public byte[] Unk64 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk68 { get; set; }

            /// <summary>
            /// Color of shadows cast by the light; alpha is relative to 100.
            /// </summary>
            [SupportsAlpha(true)]
            public Color ShadowColor { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk70 { get; set; }

            /// <summary>
            /// Minimum time between flickers.
            /// </summary>
            public float FlickerIntervalMin { get; set; }

            /// <summary>
            /// Maximum time between flickers.
            /// </summary>
            public float FlickerIntervalMax { get; set; }

            /// <summary>
            /// Multiplies the brightness of the light while flickering.
            /// </summary>
            public float FlickerBrightnessMult { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk80 { get; set; }

            /// <summary>
            /// Unknown; 4 bytes.
            /// </summary>
            public byte[] Unk84 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk88 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk90 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk98 { get; set; }

            /// <summary>
            /// Distance at which spot light beam starts.
            /// </summary>
            public float NearClip { get; set; }

            /// <summary>
            /// Unknown; 4 bytes.
            /// </summary>
            public byte[] UnkA0 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Sharpness { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float UnkAC { get; set; }

            /// <summary>
            /// Stretches the spot light beam.
            /// </summary>
            public float Width { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float UnkBC { get; set; }

            /// <summary>
            /// Unknown; 4 bytes.
            /// </summary>
            public byte[] UnkC0 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float UnkC4 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkC8 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkCC { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkD0 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkD4 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkD8 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public int UnkDC { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public float UnkE0 { get; set; }

            /// <summary>
            /// Unknown; not present before Sekiro.
            /// </summary>
            public int UnkE4 { get; set; }

            /// <summary>
            /// Creates a Light with default values.
            /// </summary>
            public Light()
            {
                Unk00 = new byte[16];
                Name = "";
                Unk1C = true;
                DiffuseColor = Color.White;
                DiffusePower = 1;
                SpecularColor = Color.White;
                SpecularPower = 1;
                Unk50 = 4;
                Radius = 10;
                Unk5C = -1;
                Unk64 = new byte[4] { 0, 0, 0, 1 };
                ShadowColor = Color.FromArgb(100, 0, 0, 0);
                FlickerBrightnessMult = 1;
                Unk80 = -1;
                Unk84 = new byte[4];
                Unk98 = 1;
                NearClip = 1;
                UnkA0 = new byte[4] { 1, 0, 2, 1 };
                Sharpness = 1;
                UnkC0 = new byte[4];
            }

            /// <summary>
            /// Creates a clone of an existing Light.
            /// </summary>
            public Light Clone()
            {
                var clone = (Light)MemberwiseClone();
                clone.Unk00 = (byte[])Unk00.Clone();
                clone.Unk64 = (byte[])Unk64.Clone();
                clone.Unk84 = (byte[])Unk84.Clone();
                clone.UnkA0 = (byte[])UnkA0.Clone();
                clone.UnkC0 = (byte[])UnkC0.Clone();
                return clone;
            }

            internal Light(BinaryReaderEx br, long namesStart, int version, bool longOffsets)
            {
                Unk00 = br.ReadBytes(16);
                Name = br.GetUTF16(namesStart + br.ReadVarint());
                Type = br.ReadEnum32<LightType>();
                Unk1C = br.ReadBoolean();
                DiffuseColor = ReadRGB(br);
                DiffusePower = br.ReadSingle();
                SpecularColor = ReadRGB(br);
                CastShadows = br.ReadBoolean();
                SpecularPower = br.ReadSingle();
                ConeAngle = br.ReadSingle();
                Unk30 = br.ReadSingle();
                Unk34 = br.ReadSingle();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Unk50 = br.ReadInt32();
                Unk54 = br.ReadSingle();
                Radius = br.ReadSingle();
                Unk5C = br.ReadInt32();
                br.AssertInt32(0);
                Unk64 = br.ReadBytes(4);
                Unk68 = br.ReadSingle();
                ShadowColor = br.ReadRGBA();
                Unk70 = br.ReadSingle();
                FlickerIntervalMin = br.ReadSingle();
                FlickerIntervalMax = br.ReadSingle();
                FlickerBrightnessMult = br.ReadSingle();
                Unk80 = br.ReadInt32();
                Unk84 = br.ReadBytes(4);
                Unk88 = br.ReadSingle();
                br.AssertInt32(0);
                Unk90 = br.ReadSingle();
                br.AssertInt32(0);
                Unk98 = br.ReadSingle();
                NearClip = br.ReadSingle();
                UnkA0 = br.ReadBytes(4);
                Sharpness = br.ReadSingle();
                br.AssertInt32(0);
                UnkAC = br.ReadSingle();
                br.AssertVarint(0);
                Width = br.ReadSingle();
                UnkBC = br.ReadSingle();
                UnkC0 = br.ReadBytes(4);
                UnkC4 = br.ReadSingle();

                if (version >= 16)
                {
                    UnkC8 = br.ReadSingle();
                    UnkCC = br.ReadSingle();
                    UnkD0 = br.ReadSingle();
                    UnkD4 = br.ReadSingle();
                    UnkD8 = br.ReadSingle();
                    UnkDC = br.ReadInt32();
                    UnkE0 = br.ReadSingle();
                    UnkE4 = br.ReadInt32();
                }
            }

            internal void Write(BinaryWriterEx bw, long nameOffset, int version, bool longOffsets)
            {
                bw.WriteBytes(Unk00);
                bw.WriteVarint(nameOffset);
                bw.WriteUInt32((uint)Type);
                bw.WriteBoolean(Unk1C);
                WriteRGB(bw, DiffuseColor);
                bw.WriteSingle(DiffusePower);
                WriteRGB(bw, SpecularColor);
                bw.WriteBoolean(CastShadows);
                bw.WriteSingle(SpecularPower);
                bw.WriteSingle(ConeAngle);
                bw.WriteSingle(Unk30);
                bw.WriteSingle(Unk34);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteInt32(Unk50);
                bw.WriteSingle(Unk54);
                bw.WriteSingle(Radius);
                bw.WriteInt32(Unk5C);
                bw.WriteInt32(0);
                bw.WriteBytes(Unk64);
                bw.WriteSingle(Unk68);
                bw.WriteRGBA(ShadowColor);
                bw.WriteSingle(Unk70);
                bw.WriteSingle(FlickerIntervalMin);
                bw.WriteSingle(FlickerIntervalMax);
                bw.WriteSingle(FlickerBrightnessMult);
                bw.WriteInt32(Unk80);
                bw.WriteBytes(Unk84);
                bw.WriteSingle(Unk88);
                bw.WriteInt32(0);
                bw.WriteSingle(Unk90);
                bw.WriteInt32(0);
                bw.WriteSingle(Unk98);
                bw.WriteSingle(NearClip);
                bw.WriteBytes(UnkA0);
                bw.WriteSingle(Sharpness);
                bw.WriteInt32(0);
                bw.WriteSingle(UnkAC);
                bw.WriteVarint(0);
                bw.WriteSingle(Width);
                bw.WriteSingle(UnkBC);
                bw.WriteBytes(UnkC0);
                bw.WriteSingle(UnkC4);

                if (version >= 16)
                {
                    bw.WriteSingle(UnkC8);
                    bw.WriteSingle(UnkCC);
                    bw.WriteSingle(UnkD0);
                    bw.WriteSingle(UnkD4);
                    bw.WriteSingle(UnkD8);
                    bw.WriteInt32(UnkDC);
                    bw.WriteSingle(UnkE0);
                    bw.WriteInt32(UnkE4);
                }
            }

            /// <summary>
            /// Returns the name of the light.
            /// </summary>
            public override string ToString()
            {
                return Name;
            }

            private static Color ReadRGB(BinaryReaderEx br)
            {
                byte[] rgb = br.ReadBytes(3);
                return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
            }

            private static void WriteRGB(BinaryWriterEx bw, Color color)
            {
                bw.WriteByte(color.R);
                bw.WriteByte(color.G);
                bw.WriteByte(color.B);
            }
        }
    }
}
