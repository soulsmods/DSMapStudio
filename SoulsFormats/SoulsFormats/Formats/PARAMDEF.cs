using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A companion format to params that describes each field present in the rows. Extension: .def, .paramdef
    /// </summary>
    public class PARAMDEF : SoulsFile<PARAMDEF>
    {
        /// <summary>
        /// Unknown; observed values 0, 1, and 3.
        /// </summary>
        public short Unk06 { get; set; }

        /// <summary>
        /// Identifies corresponding params and paramdefs.
        /// </summary>
        public string ParamID { get; set; }

        /// <summary>
        /// True for PS3 and X360 games, otherwise false.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// If true, strings are written as UTF-16; if false, as Shift-JIS.
        /// </summary>
        public bool Unicode { get; set; }

        /// <summary>
        /// Determines format of the file.
        /// </summary>
        public short Version { get; set; }

        /// <summary>
        /// Fields in each param row, in order of appearance.
        /// </summary>
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Creates a new PARAMDEF formatted for DS1.
        /// </summary>
        public PARAMDEF()
        {
            ParamID = "AI_STANDARD_INFO_BANK";
            Version = 104;
            Fields = new List<Field>();
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            BigEndian = br.GetSByte(0x2C) == -1;
            br.BigEndian = BigEndian;

            br.ReadInt32(); // File size
            short unk04 = br.AssertInt16(0x30, 0xFF);
            Unk06 = br.ReadInt16();
            short fieldCount = br.ReadInt16();
            short fieldSize = br.AssertInt16(0x6C, 0x8C, 0xAC, 0xB0, 0xD0);
            ParamID = br.ReadFixStr(0x20);
            br.ReadByte(); // Big-endian
            Unicode = br.ReadBoolean();
            Version = br.AssertInt16(101, 102, 103, 104, 201);
            if (Version >= 201)
                br.AssertInt64(0x38);

            if (!(Version < 200 && unk04 == 0x30 || Version >= 200 && unk04 == 0xFF))
                throw new InvalidDataException($"Unexpected unk04 0x{unk04:X} for version {Version}.");

            // Please note that for version 103 (Ninja Blade), this value is straight up wrong.
            if (!(Version == 101 && fieldSize == 0x8C || Version == 102 && fieldSize == 0xAC || Version == 103 && fieldSize == 0x6C
                || Version == 104 && fieldSize == 0xB0 || Version == 201 && fieldSize == 0xD0))
                throw new InvalidDataException($"Unexpected field size 0x{fieldSize:X} for version {Version}.");

            Fields = new List<Field>(fieldCount);
            for (int i = 0; i < fieldCount; i++)
                Fields.Add(new Field(br, this));
        }

        /// <summary>
        /// Verifies that the file can be written safely.
        /// </summary>
        public override bool Validate(out Exception ex)
        {
            if (!(Version == 101 || Version == 102 || Version == 103 || Version == 104 || Version == 201))
            {
                ex = new InvalidDataException($"Unknown version: {Version}");
                return false;
            }

            if (!ValidateNull(ParamID, $"{nameof(ParamID)} may not be null.", out ex)
                || !ValidateNull(Fields, $"{nameof(Fields)} may not be null.", out ex))
                return false;

            for (int i = 0; i < Fields.Count; i++)
            {
                Field field = Fields[i];
                if (!ValidateNull(field, $"{nameof(Fields)}[{i}]: {nameof(Field)} may not be null.", out ex)
                    || !ValidateNull(field.DisplayName, $"{nameof(Fields)}[{i}]: {nameof(Field.DisplayName)} may not be null.", out ex)
                    || !ValidateNull(field.DisplayType, $"{nameof(Fields)}[{i}]: {nameof(Field.DisplayType)} may not be null.", out ex)
                    || !ValidateNull(field.DisplayFormat, $"{nameof(Fields)}[{i}]: {nameof(Field.DisplayFormat)} may not be null.", out ex)
                    || !ValidateNull(field.InternalType, $"{nameof(Fields)}[{i}]: {nameof(Field.InternalType)} may not be null.", out ex)
                    || Version >= 102 && !ValidateNull(field.InternalName, $"{nameof(Fields)}[{i}]: {nameof(Field.InternalName)} may not be null on version {Version}.", out ex))
                    return false;
            }

            ex = null;
            return true;
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = BigEndian;

            bw.ReserveInt32("FileSize");
            bw.WriteInt16((short)(Version >= 201 ? 0xFF : 0x30));
            bw.WriteInt16(Unk06);
            bw.WriteInt16((short)Fields.Count);

            if (Version == 101)
                bw.WriteInt16(0x8C);
            else if (Version == 102)
                bw.WriteInt16(0xAC);
            else if (Version == 103)
                bw.WriteInt16(0x6C);
            else if (Version == 104)
                bw.WriteInt16(0xB0);
            else if (Version == 201)
                bw.WriteInt16(0xD0);

            bw.WriteFixStr(ParamID, 0x20, (byte)(Version >= 201 ? 0x00 : 0x20));
            bw.WriteSByte((sbyte)(BigEndian ? -1 : 0));
            bw.WriteBoolean(Unicode);
            bw.WriteInt16(Version);
            if (Version >= 201)
                bw.WriteInt64(0x38);

            for (int i = 0; i < Fields.Count; i++)
                Fields[i].Write(bw, this, i);

            long descriptionsStart = bw.Position;
            for (int i = 0; i < Fields.Count; i++)
                Fields[i].WriteDescription(bw, this, i);

            if (Version >= 104)
            {
                long descriptionsLength = bw.Position - descriptionsStart;
                if (descriptionsLength % 0x10 != 0)
                    bw.WritePattern((int)(0x10 - descriptionsLength % 0x10), 0x00);
            }
            else
            {
                bw.Pad(0x10);
            }
            bw.FillInt32("FileSize", (int)bw.Position);
        }

        /// <summary>
        /// Flags that control editor behavior for a field.
        /// </summary>
        [Flags]
        public enum EditFlags
        {
            /// <summary>
            /// Value is editable and does not wrap.
            /// </summary>
            None = 0,

            /// <summary>
            /// Value wraps around when scrolled past the minimum or maximum.
            /// </summary>
            Wrap = 1,

            /// <summary>
            /// Value may not be edited.
            /// </summary>
            Lock = 4,
        }

        /// <summary>
        /// Information about a field present in each row in a param.
        /// </summary>
        public class Field
        {
            /// <summary>
            /// Name to display in the editor.
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            /// Type of value to display in the editor.
            /// </summary>
            public string DisplayType { get; set; }

            /// <summary>
            /// Printf-style format string to apply to the value in the editor.
            /// </summary>
            public string DisplayFormat { get; set; }

            /// <summary>
            /// Default value for new rows.
            /// </summary>
            public float Default { get; set; }

            /// <summary>
            /// Minimum valid value.
            /// </summary>
            public float Minimum { get; set; }

            /// <summary>
            /// Maximum valid value.
            /// </summary>
            public float Maximum { get; set; }

            /// <summary>
            /// Amount of increase or decrease per step when scrolling in the editor.
            /// </summary>
            public float Increment { get; set; }

            /// <summary>
            /// Flags determining behavior of the field in the editor.
            /// </summary>
            public EditFlags EditFlags { get; set; }

            /// <summary>
            /// Number of bytes taken up by the value; may be variable for some types.
            /// </summary>
            public int ByteCount { get; set; }

            /// <summary>
            /// Optional description of the field; may be null.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Type of the value in the engine; may be an enum type.
            /// </summary>
            public string InternalType { get; set; }

            /// <summary>
            /// Name of the value in the engine; not present before version 102.
            /// </summary>
            public string InternalName { get; set; }

            /// <summary>
            /// Fields are ordered by this value in the editor; not present before version 104.
            /// </summary>
            public int SortID { get; set; }

            /// <summary>
            /// Creates a Field with placeholder values.
            /// </summary>
            public Field()
            {
                DisplayName = "Placeholder";
                DisplayType = "f32";
                DisplayFormat = "%f";
                Minimum = float.MinValue;
                Maximum = float.MaxValue;
                Increment = 1;
                EditFlags = EditFlags.Wrap;
                ByteCount = 4;
                InternalType = "f32";
                InternalName = "placeholder";
            }

            internal Field(BinaryReaderEx br, PARAMDEF def)
            {
                if (def.Unicode)
                    DisplayName = br.ReadFixStrW(0x40);
                else
                    DisplayName = br.ReadFixStr(0x40);

                DisplayType = br.ReadFixStr(8);
                DisplayFormat = br.ReadFixStr(8);
                Default = br.ReadSingle();
                Minimum = br.ReadSingle();
                Maximum = br.ReadSingle();
                Increment = br.ReadSingle();
                EditFlags = (EditFlags)br.ReadInt32();
                ByteCount = br.ReadInt32();

                long descriptionOffset;
                if (def.Version >= 201)
                    descriptionOffset = br.ReadInt64();
                else
                    descriptionOffset = br.ReadInt32();

                InternalType = br.ReadFixStr(0x20);

                if (def.Version >= 102)
                    InternalName = br.ReadFixStr(0x20);

                if (def.Version >= 104)
                    SortID = br.ReadInt32();

                if (def.Version >= 201)
                    br.AssertPattern(0x1C, 0x00);

                if (descriptionOffset != 0)
                {
                    if (def.Unicode)
                        Description = br.GetUTF16(descriptionOffset);
                    else
                        Description = br.GetShiftJIS(descriptionOffset);
                }
            }

            internal void Write(BinaryWriterEx bw, PARAMDEF def, int index)
            {
                if (def.Unicode)
                    bw.WriteFixStrW(DisplayName, 0x40, (byte)(def.Version >= 104 ? 0x00 : 0x20));
                else
                    bw.WriteFixStr(DisplayName, 0x40, (byte)(def.Version >= 104 ? 0x00 : 0x20));

                byte padding = (byte)(def.Version >= 201 ? 0x00 : 0x20);
                bw.WriteFixStr(DisplayType, 8, padding);
                bw.WriteFixStr(DisplayFormat, 8, padding);
                bw.WriteSingle(Default);
                bw.WriteSingle(Minimum);
                bw.WriteSingle(Maximum);
                bw.WriteSingle(Increment);
                bw.WriteInt32((int)EditFlags);
                bw.WriteInt32(ByteCount);

                if (def.Version >= 201)
                    bw.ReserveInt64($"DescriptionOffset{index}");
                else
                    bw.ReserveInt32($"DescriptionOffset{index}");

                bw.WriteFixStr(InternalType, 0x20, padding);

                if (def.Version >= 102)
                    bw.WriteFixStr(InternalName, 0x20, padding);

                if (def.Version >= 104)
                    bw.WriteInt32(SortID);

                if (def.Version >= 201)
                    bw.WritePattern(0x1C, 0x00);
            }

            internal void WriteDescription(BinaryWriterEx bw, PARAMDEF def, int index)
            {
                long descriptionOffset = 0;
                if (Description != null)
                {
                    descriptionOffset = bw.Position;
                    if (def.Unicode)
                        bw.WriteUTF16(Description, true);
                    else
                        bw.WriteShiftJIS(Description, true);
                }

                if (def.Version >= 201)
                    bw.FillInt64($"DescriptionOffset{index}", descriptionOffset);
                else
                    bw.FillInt32($"DescriptionOffset{index}", (int)descriptionOffset);
            }
        }
    }
}
