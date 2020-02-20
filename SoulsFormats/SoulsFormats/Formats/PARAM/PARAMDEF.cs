using SoulsFormats.XmlExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

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
        public string ParamType { get; set; }

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
        // 101 - Enchanted Arms, Chromehounds, Armored Core 4/For Answer/V/Verdict Day, Shadow Assault: Tenchu
        // 102 - Demon's Souls
        // 103 - Ninja Blade, Another Century's Episode: R
        // 104 - Dark Souls, Steel Battalion: Heavy Armor
        // 201 - Bloodborne
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
            ParamType = "AI_STANDARD_INFO_BANK";
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
            ParamType = br.ReadFixStr(0x20);
            br.ReadByte(); // Big-endian
            Unicode = br.ReadBoolean();
            Version = br.AssertInt16(101, 102, 103, 104, 201);
            if (Version >= 201)
                br.AssertInt64(0x38);

            if (!(Version < 200 && unk04 == 0x30 || Version >= 200 && unk04 == 0xFF))
                throw new InvalidDataException($"Unexpected unk04 0x{unk04:X} for version {Version}.");

            // Please note that for version 103 this value is wrong.
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

            if (!ValidateNull(ParamType, $"{nameof(ParamType)} may not be null.", out ex)
                || !ValidateNull(Fields, $"{nameof(Fields)} may not be null.", out ex))
                return false;

            for (int i = 0; i < Fields.Count; i++)
            {
                Field field = Fields[i];
                string which = $"{nameof(Fields)}[{i}]";
                if (!ValidateNull(field, $"{which}: {nameof(Field)} may not be null.", out ex)
                    || !ValidateNull(field.DisplayName, $"{which}: {nameof(Field.DisplayName)} may not be null.", out ex)
                    || !ValidateNull(field.DisplayFormat, $"{which}: {nameof(Field.DisplayFormat)} may not be null.", out ex)
                    || !ValidateNull(field.InternalType, $"{which}: {nameof(Field.InternalType)} may not be null.", out ex)
                    || Version >= 102 && !ValidateNull(field.InternalName, $"{which}: {nameof(Field.InternalName)} may not be null on version {Version}.", out ex))
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

            bw.WriteFixStr(ParamType, 0x20, (byte)(Version >= 201 ? 0x00 : 0x20));
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
        /// Calculates the size of cell data for each row.
        /// </summary>
        public int GetRowSize()
        {
            int size = 0;
            for (int i = 0; i < Fields.Count; i++)
            {
                Field field = Fields[i];
                DefType type = field.DisplayType;
                if (ParamUtil.IsArrayType(type))
                    size += ParamUtil.GetValueSize(type) * field.ArrayLength;
                else
                    size += ParamUtil.GetValueSize(type);

                if (ParamUtil.IsBitType(type) && field.BitSize != -1)
                {
                    int bitOffset = field.BitSize;
                    DefType bitType = type == DefType.dummy8 ? DefType.u8 : type;
                    int bitLimit = ParamUtil.GetBitLimit(bitType);

                    for (; i < Fields.Count - 1; i++)
                    {
                        Field nextField = Fields[i + 1];
                        DefType nextType = nextField.DisplayType;
                        if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                            || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
                            break;
                        bitOffset += nextField.BitSize;
                    }
                }
            }
            return size;
        }

        #region XML Serialization
        private const int XML_VERSION = 0;

        /// <summary>
        /// Reads an XML-formatted PARAMDEF from a file.
        /// </summary>
        public static PARAMDEF XmlDeserialize(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);
            return new PARAMDEF(xml);
        }

        private PARAMDEF(XmlDocument xml)
        {
            XmlNode root = xml.SelectSingleNode("PARAMDEF");
            int xmlVersion = int.Parse(root.Attributes["XmlVersion"].InnerText);
            if (xmlVersion != XML_VERSION)
                throw new InvalidDataException($"Mismatched XML version; current version: {XML_VERSION}, file version: {xmlVersion}");

            ParamType = root.SelectSingleNode(nameof(ParamType)).InnerText;
            Unk06 = root.ReadInt16(nameof(Unk06));
            BigEndian = root.ReadBoolean(nameof(BigEndian));
            Unicode = root.ReadBoolean(nameof(Unicode));
            Version = root.ReadInt16(nameof(Version));

            Fields = new List<Field>();
            foreach (XmlNode node in root.SelectNodes($"{nameof(Fields)}/{nameof(Field)}"))
            {
                Fields.Add(new Field(node));
            }
        }

        /// <summary>
        /// Writes an XML-formatted PARAMDEF to a file.
        /// </summary>
        public void XmlSerialize(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var xws = new XmlWriterSettings { Indent = true };
            using (var xw = XmlWriter.Create(path, xws))
                XmlSerialize(xw);
        }

        private void XmlSerialize(XmlWriter xw)
        {
            xw.WriteStartDocument();
            xw.WriteStartElement(nameof(PARAMDEF));
            xw.WriteAttributeString("XmlVersion", XML_VERSION.ToString());
            xw.WriteElementString(nameof(ParamType), ParamType);
            xw.WriteElementString(nameof(Unk06), Unk06.ToString());
            xw.WriteElementString(nameof(BigEndian), BigEndian.ToString());
            xw.WriteElementString(nameof(Unicode), Unicode.ToString());
            xw.WriteElementString(nameof(Version), Version.ToString());

            xw.WriteStartElement(nameof(Fields));
            foreach (Field field in Fields)
            {
                xw.WriteStartElement(nameof(Field));
                field.XmlSerialize(xw);
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
        }
        #endregion

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
        /// Supported primitive field types.
        /// </summary>
        public enum DefType
        {
            /// <summary>
            /// Signed 1-byte integer.
            /// </summary>
            s8,

            /// <summary>
            /// Unsigned 1-byte integer.
            /// </summary>
            u8,

            /// <summary>
            /// Signed 2-byte integer.
            /// </summary>
            s16,

            /// <summary>
            /// Unsigned 2-byte integer.
            /// </summary>
            u16,

            /// <summary>
            /// Signed 4-byte integer.
            /// </summary>
            s32,

            /// <summary>
            /// Unsigned 4-byte integer.
            /// </summary>
            u32,

            /// <summary>
            /// Single-precision floating point value.
            /// </summary>
            f32,

            /// <summary>
            /// Byte or array of bytes used for padding or placeholding.
            /// </summary>
            dummy8,

            /// <summary>
            /// Fixed-width Shift-JIS string.
            /// </summary>
            fixstr,

            /// <summary>
            /// Fixed-width UTF-16 string.
            /// </summary>
            fixstrW,
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
            public DefType DisplayType { get; set; }

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
            /// Number of elements for array types; only supported for dummy8, fixstr, and fixstrW.
            /// </summary>
            public int ArrayLength { get; set; }

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
            /// Number of bits used by a bitfield; only supported for unsigned types, -1 when not used.
            /// </summary>
            public int BitSize { get; set; }

            /// <summary>
            /// Fields are ordered by this value in the editor; not present before version 104.
            /// </summary>
            public int SortID { get; set; }

            private static readonly Regex arrayLengthRx = new Regex(@"^(?<name>.+?)\s*\[\s*(?<length>\d+)\s*\]\s*$");
            private static readonly Regex bitSizeRx = new Regex(@"^(?<name>.+?)\s*\:\s*(?<size>\d+)\s*$");

            /// <summary>
            /// Creates a Field with placeholder values.
            /// </summary>
            public Field() : this(DefType.f32, "placeholder") { }

            /// <summary>
            /// Creates a Field with the given type, name, and appropriate default values.
            /// </summary>
            public Field(DefType displayType, string internalName)
            {
                DisplayName = internalName;
                DisplayType = displayType;
                DisplayFormat = ParamUtil.GetDefaultFormat(DisplayType);
                Minimum = ParamUtil.GetDefaultMinimum(DisplayType);
                Maximum = ParamUtil.GetDefaultMaximum(DisplayType);
                Increment = ParamUtil.GetDefaultIncrement(DisplayType);
                EditFlags = ParamUtil.GetDefaultEditFlags(DisplayType);
                ArrayLength = 1;
                InternalType = DisplayType.ToString();
                InternalName = internalName;
                BitSize = -1;
            }

            internal Field(BinaryReaderEx br, PARAMDEF def)
            {
                if (def.Unicode)
                    DisplayName = br.ReadFixStrW(0x40);
                else
                    DisplayName = br.ReadFixStr(0x40);

                DisplayType = (DefType)Enum.Parse(typeof(DefType), br.ReadFixStr(8));
                DisplayFormat = br.ReadFixStr(8);
                Default = br.ReadSingle();
                Minimum = br.ReadSingle();
                Maximum = br.ReadSingle();
                Increment = br.ReadSingle();
                EditFlags = (EditFlags)br.ReadInt32();

                int byteCount = br.ReadInt32();
                if (!ParamUtil.IsArrayType(DisplayType) && byteCount != ParamUtil.GetValueSize(DisplayType)
                    || ParamUtil.IsArrayType(DisplayType) && byteCount % ParamUtil.GetValueSize(DisplayType) != 0)
                    throw new InvalidDataException($"Unexpected byte count {byteCount} for type {DisplayType}.");
                ArrayLength = byteCount / ParamUtil.GetValueSize(DisplayType);

                long descriptionOffset;
                if (def.Version >= 201)
                    descriptionOffset = br.ReadInt64();
                else
                    descriptionOffset = br.ReadInt32();

                InternalType = br.ReadFixStr(0x20);

                BitSize = -1;
                if (def.Version >= 102)
                {
                    // A few fields in DS1 FaceGenParam have a trailing space in the name
                    InternalName = br.ReadFixStr(0x20).Trim();

                    Match match = bitSizeRx.Match(InternalName);
                    if (match.Success)
                    {
                        InternalName = match.Groups["name"].Value;
                        BitSize = int.Parse(match.Groups["size"].Value);
                    }

                    if (ParamUtil.IsArrayType(DisplayType))
                    {
                        match = arrayLengthRx.Match(InternalName);
                        int length = match.Success ? int.Parse(match.Groups["length"].Value) : 1;
                        if (length != ArrayLength)
                            throw new InvalidDataException($"Mismatched array length in {InternalName} with byte count {byteCount}.");
                        if (match.Success)
                            InternalName = match.Groups["name"].Value;
                    }
                }

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
                bw.WriteFixStr(DisplayType.ToString(), 8, padding);
                bw.WriteFixStr(DisplayFormat, 8, padding);
                bw.WriteSingle(Default);
                bw.WriteSingle(Minimum);
                bw.WriteSingle(Maximum);
                bw.WriteSingle(Increment);
                bw.WriteInt32((int)EditFlags);
                bw.WriteInt32(ParamUtil.GetValueSize(DisplayType) * (ParamUtil.IsArrayType(DisplayType) ? ArrayLength : 1));

                if (def.Version >= 201)
                    bw.ReserveInt64($"DescriptionOffset{index}");
                else
                    bw.ReserveInt32($"DescriptionOffset{index}");

                bw.WriteFixStr(InternalType, 0x20, padding);

                if (def.Version >= 102)
                {
                    string internalName = InternalName;
                    // This is accurate except for "hasTarget : 1" in SpEffect
                    if (BitSize != -1)
                        internalName = $"{internalName}:{BitSize}";
                    // BB is not consistent about including [1] or not, but PTDE always does
                    else if (ParamUtil.IsArrayType(DisplayType))
                        internalName = $"{internalName}[{ArrayLength}]";
                    bw.WriteFixStr(internalName, 0x20, padding);
                }

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

            /// <summary>
            /// Returns a string representation of the field.
            /// </summary>
            public override string ToString()
            {
                if (ParamUtil.IsBitType(DisplayType) && BitSize != -1)
                    return $"{DisplayType} {InternalName}:{BitSize}";
                else if (ParamUtil.IsArrayType(DisplayType))
                    return $"{DisplayType} {InternalName}[{ArrayLength}]";
                else
                    return $"{DisplayType} {InternalName}";
            }

            #region XML Serialization
            private static readonly Regex defOuterRx = new Regex($@"^(?<type>\S+)\s+(?<name>.+?)(?:\s*=\s*(?<default>\S+))?$");
            private static readonly Regex defBitRx = new Regex($@"^(?<name>.+?)\s*:\s*(?<size>\d+)$");
            private static readonly Regex defArrayRx = new Regex($@"^(?<name>.+?)\s*\[\s*(?<length>\d+)\]$");

            internal Field(XmlNode node)
            {
                string def = node.Attributes["Def"].InnerText;
                Match outerMatch = defOuterRx.Match(def);
                DisplayType = (DefType)Enum.Parse(typeof(DefType), outerMatch.Groups["type"].Value.Trim());
                if (outerMatch.Groups["default"].Success)
                    Default = float.Parse(outerMatch.Groups["default"].Value, CultureInfo.InvariantCulture);

                string internalName = outerMatch.Groups["name"].Value.Trim();
                Match bitMatch = defBitRx.Match(internalName);
                Match arrayMatch = defArrayRx.Match(internalName);
                BitSize = -1;
                ArrayLength = 1;
                if (ParamUtil.IsBitType(DisplayType) && bitMatch.Success)
                {
                    BitSize = int.Parse(bitMatch.Groups["size"].Value);
                    internalName = bitMatch.Groups["name"].Value;
                }
                else if (ParamUtil.IsArrayType(DisplayType))
                {
                    ArrayLength = int.Parse(arrayMatch.Groups["length"].Value);
                    internalName = arrayMatch.Groups["name"].Value;
                }
                InternalName = internalName;

                DisplayName = node.ReadStringOrDefault(nameof(DisplayName), InternalName);
                InternalType = node.ReadStringOrDefault("Enum", DisplayType.ToString());
                Description = node.ReadStringOrDefault(nameof(Description), null);
                DisplayFormat = node.ReadStringOrDefault(nameof(DisplayFormat), ParamUtil.GetDefaultFormat(DisplayType));
                EditFlags = (EditFlags)Enum.Parse(typeof(EditFlags),
                    node.ReadStringOrDefault(nameof(EditFlags), ParamUtil.GetDefaultEditFlags(DisplayType).ToString()));
                Minimum = node.ReadSingleOrDefault(nameof(Minimum), ParamUtil.GetDefaultMinimum(DisplayType), CultureInfo.InvariantCulture);
                Maximum = node.ReadSingleOrDefault(nameof(Maximum), ParamUtil.GetDefaultMaximum(DisplayType), CultureInfo.InvariantCulture);
                Increment = node.ReadSingleOrDefault(nameof(Increment), ParamUtil.GetDefaultIncrement(DisplayType), CultureInfo.InvariantCulture);
                SortID = node.ReadInt32OrDefault(nameof(SortID), 0);
            }

            internal void XmlSerialize(XmlWriter xw)
            {
                string def = $"{DisplayType} {InternalName}";
                if (ParamUtil.IsBitType(DisplayType) && BitSize != -1)
                    def += $":{BitSize}";
                else if (ParamUtil.IsArrayType(DisplayType))
                    def += $"[{ArrayLength}]";

                if (Default != 0)
                    def += $" = {Default.ToString("R", CultureInfo.InvariantCulture)}";

                xw.WriteAttributeString("Def", def);
                xw.WriteDefaultElement(nameof(DisplayName), DisplayName, InternalName);
                xw.WriteDefaultElement("Enum", InternalType, DisplayType.ToString());
                xw.WriteDefaultElement(nameof(Description), Description, null);
                xw.WriteDefaultElement(nameof(DisplayFormat), DisplayFormat, ParamUtil.GetDefaultFormat(DisplayType));
                xw.WriteDefaultElement(nameof(EditFlags), EditFlags.ToString(), ParamUtil.GetDefaultEditFlags(DisplayType).ToString());
                xw.WriteDefaultElement(nameof(Minimum), Minimum, ParamUtil.GetDefaultMinimum(DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement(nameof(Maximum), Maximum, ParamUtil.GetDefaultMaximum(DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement(nameof(Increment), Increment, ParamUtil.GetDefaultIncrement(DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement(nameof(SortID), SortID, 0);
            }
            #endregion
        }
    }
}
