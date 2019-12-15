using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// A general-purpose configuration file used throughout the series.
    /// </summary>
    public partial class PARAM : SoulsFile<PARAM>
    {
        /// <summary>
        /// Whether the file is big-endian; true for PS3/360 files, false otherwise.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// An unknown format or version indicator.
        /// </summary>
        public byte Format2D { get; set; }

        /// <summary>
        /// An unknown format or version indicator.
        /// </summary>
        public byte Format2E { get; set; }

        /// <summary>
        /// An unknown format or version indicator; usually 0x00, 0xFF in DS2 NT.
        /// </summary>
        public byte Format2F { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public short Unk06 { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public short Unk08 { get; set; }

        /// <summary>
        /// The param format ID of rows in this param.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Automatically determined based on spacing of row offsets; could be wrong in theory, but never seems to be.
        /// </summary>
        public long DetectedSize { get; private set; }

        /// <summary>
        /// The rows of this param; must be loaded with PARAM.ReadRows() before cells can be used.
        /// </summary>
        public List<Row> Rows { get; set; }

        /// <summary>
        /// Gets a BinaryReaderEx ready to read at the beginning of the 
        /// specified row's data. Only call this if you know what 
        /// you are doing.
        /// </summary>
        public BinaryReaderEx GetRowReader(Row row)
        {
            if (Rows.Contains(row))
            {
                brRows.Position = row.Offset;
                return brRows;
            }
            else
            {
                throw new ArgumentException("Row does not exist within this PARAM. Cannot read it.");
            }
        }

        private BinaryReaderEx brRows;
        private Layout layout;

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.Position = 0x2C;
            BigEndian = br.AssertByte(0, 0xFF) == 0xFF;
            Format2D = br.ReadByte();
            Format2E = br.ReadByte();
            Format2F = br.AssertByte(0, 0xFF);
            br.Position = 0;
            br.BigEndian = BigEndian;

            // Make a private copy of the file to read row data from later
            byte[] copy = br.GetBytes(0, (int)br.Stream.Length);
            brRows = new BinaryReaderEx(BigEndian, copy);

            ushort rowCount;
            long stringsOffset;

            // DeS, DS1
            if ((Format2D & 0x7F) < 3)
            {
                stringsOffset = br.ReadUInt32();
                br.ReadUInt16(); // Data start
                Unk06 = br.ReadInt16();
                Unk08 = br.ReadInt16();
                rowCount = br.ReadUInt16();
                ID = br.ReadFixStr(0x20);
                br.Skip(4); // Format
            }
            // DS2
            else if ((Format2D & 0x7F) == 3)
            {
                stringsOffset = br.ReadUInt32();
                br.AssertInt16(0);
                Unk06 = br.ReadInt16();
                Unk08 = br.ReadInt16();
                rowCount = br.ReadUInt16();
                ID = br.ReadFixStr(0x20);
                br.Skip(4); // Format
                br.ReadUInt32(); // Data start
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);
            }
            // SotFS, BB
            else if ((Format2D & 0x7F) == 4)
            {
                stringsOffset = br.ReadUInt32();
                br.AssertInt16(0);
                Unk06 = br.ReadInt16();
                Unk08 = br.ReadInt16();
                rowCount = br.ReadUInt16();
                ID = br.ReadFixStr(0x20);
                br.Skip(4); // Format
                br.ReadInt64(); // Data start
                br.AssertInt64(0);
            }
            // DS3, SDT
            else
            {
                stringsOffset = br.ReadUInt32();
                br.AssertInt16(0);
                Unk06 = br.ReadInt16();
                Unk08 = br.ReadInt16();
                rowCount = br.ReadUInt16();
                br.AssertInt32(0);
                long idOffset = br.ReadInt64();
                br.AssertPattern(0x14, 0x00);
                br.Skip(4); // Format
                br.ReadInt64(); // Data start
                br.AssertInt64(0);
                ID = br.GetASCII(idOffset);

                // This is stupid, but the strings offset is always aligned to 0x10,
                // which can put it right in the middle of the ID string
                stringsOffset = idOffset;
            }

            Rows = new List<Row>(rowCount);
            for (int i = 0; i < rowCount; i++)
                Rows.Add(new Row(br, Format2D));

            if (Rows.Count > 1)
                DetectedSize = Rows[1].Offset - Rows[0].Offset;
            else
                DetectedSize = stringsOffset - Rows[0].Offset;
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            if (layout == null)
                throw new InvalidOperationException("Params cannot be written without a layout.");

            Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

            bw.BigEndian = BigEndian;
            void WriteFormat()
            {
                bw.WriteByte((byte)(BigEndian ? 0xFF : 0x00));
                bw.WriteByte(Format2D);
                bw.WriteByte(Format2E);
                bw.WriteByte(Format2F);
            }

            // DeS, DS1
            if ((Format2D & 0x7F) < 3)
            {
                bw.ReserveUInt32("StringsOffset");
                bw.ReserveUInt16("DataStart");
                bw.WriteInt16(Unk06);
                bw.WriteInt16(Unk08);
                bw.WriteUInt16((ushort)Rows.Count);
                bw.WriteFixStr(ID, 0x20, 0x20);
                WriteFormat();
            }
            // DS2
            else if ((Format2D & 0x7F) == 3)
            {
                bw.ReserveUInt32("StringsOffset");
                bw.WriteInt16(0);
                bw.WriteInt16(Unk06);
                bw.WriteInt16(Unk08);
                bw.WriteUInt16((ushort)Rows.Count);
                bw.WriteFixStr(ID, 0x20, 0x20);
                WriteFormat();
                bw.ReserveUInt32("DataStart");
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
            // SotFS, BB
            else if ((Format2D & 0x7F) == 4)
            {
                bw.ReserveUInt32("StringsOffset");
                bw.WriteInt16(0);
                bw.WriteInt16(Unk06);
                bw.WriteInt16(Unk08);
                bw.WriteUInt16((ushort)Rows.Count);
                bw.WriteFixStr(ID, 0x20, 0x00);
                WriteFormat();
                bw.ReserveInt64("DataStart");
                bw.WriteInt64(0);
            }
            // DS3, SDT
            else
            {
                bw.ReserveUInt32("StringsOffset");
                bw.WriteInt16(0);
                bw.WriteInt16(Unk06);
                bw.WriteInt16(Unk08);
                bw.WriteUInt16((ushort)Rows.Count);
                bw.WriteInt32(0);
                bw.ReserveInt64("IDOffset");
                bw.WritePattern(0x14, 0x00);
                WriteFormat();
                bw.ReserveInt64("DataStart");
                bw.WriteInt64(0);
            }

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].WriteHeader(bw, Format2D, i);

            if ((Format2D & 0x7F) < 3)
                bw.FillUInt16("DataStart", (ushort)bw.Position);
            else if ((Format2D & 0x7F) == 3)
                bw.FillUInt32("DataStart", (uint)bw.Position);
            else
                bw.FillInt64("DataStart", bw.Position);

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].WriteCells(bw, Format2D, i, layout);

            bw.FillUInt32("StringsOffset", (uint)bw.Position);

            if ((Format2D & 0x7F) > 4)
            {
                bw.FillInt64("IDOffset", bw.Position);
                bw.WriteASCII(ID, true);
            }

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].WriteName(bw, Format2D, i);
        }

        /// <summary>
        /// Sets the layout to use when writing
        /// </summary>
        public void SetLayout(Layout layout)
        {
            this.layout = layout;
            foreach (Row row in Rows)
                row.ReadRow(brRows, layout);
        }

        /// <summary>
        /// Returns the first row with the given ID, or null if not found.
        /// </summary>
        public Row this[int id] => Rows.Find(row => row.ID == id);

        /// <summary>
        /// One row in a param file.
        /// </summary>
        public class Row
        {
            /// <summary>
            /// The ID number of this row.
            /// </summary>
            public long ID { get; set; }

            /// <summary>
            /// A name given to this row; no functional significance, may be null.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Cells contained in this row. Must be loaded with PARAM.ReadRows() before use.
            /// </summary>
            public List<Cell> Cells { get; set; }

            internal long Offset;

            /// <summary>
            /// Creates a new row based on the given layout with default values.
            /// </summary>
            public Row(long id, string name, Layout layout)
            {
                ID = id;
                Name = name;

                Cells = new List<Cell>(layout.Count);
                foreach (Layout.Entry entry in layout)
                    Cells.Add(new Cell(entry, entry.Default));
            }

            internal Row(BinaryReaderEx br, byte format2D)
            {
                long nameOffset;
                if ((format2D & 0x7F) < 4)
                {
                    ID = br.ReadUInt32();
                    Offset = br.ReadUInt32();
                    nameOffset = br.ReadUInt32();
                }
                else
                {
                    ID = br.ReadInt64();
                    Offset = br.ReadInt64();
                    nameOffset = br.ReadInt64();
                }

                if (nameOffset != 0)
                {
                    if ((format2D & 0x7F) < 4)
                        Name = br.GetShiftJIS(nameOffset);
                    else
                        Name = br.GetUTF16(nameOffset);
                }
            }

            internal void ReadRow(BinaryReaderEx br, Layout layout)
            {
                br.StepIn(Offset);
                Cells = new List<Cell>(layout.Count);

                for (int i = 0; i < layout.Count; i++)
                {
                    Layout.Entry entry = layout[i];
                    CellType type = entry.Type;

                    object value = null;

                    void ReadBools(CellType boolType, int fieldSize)
                    {
                        byte[] b = br.ReadBytes(fieldSize);
                        int j;
                        for (j = 0; j < fieldSize * 8; j++)
                        {
                            if (i + j >= layout.Count || layout[i + j].Type != boolType)
                                break;

                            byte mask = (byte)(1 << (j % 8));
                            Cells.Add(new Cell(layout[i + j], (b[j / 8] & mask) != 0));
                        }
                        i += j - 1;
                    }

                    if (type == CellType.s8)
                        value = br.ReadSByte();
                    else if (type == CellType.u8 || type == CellType.x8)
                        value = br.ReadByte();
                    else if (type == CellType.s16)
                        value = br.ReadInt16();
                    else if (type == CellType.u16 || type == CellType.x16)
                        value = br.ReadUInt16();
                    else if (type == CellType.s32)
                        value = br.ReadInt32();
                    else if (type == CellType.u32 || type == CellType.x32)
                        value = br.ReadUInt32();
                    else if (type == CellType.f32)
                        value = br.ReadSingle();
                    else if (type == CellType.dummy8)
                        value = br.ReadBytes(entry.Size);
                    else if (type == CellType.fixstr)
                        value = br.ReadFixStr(entry.Size);
                    else if (type == CellType.fixstrW)
                        value = br.ReadFixStrW(entry.Size);
                    else if (type == CellType.b8)
                        ReadBools(type, 1);
                    else if (type == CellType.b16)
                        ReadBools(type, 2);
                    else if (type == CellType.b32)
                        ReadBools(type, 4);
                    else
                        throw new NotImplementedException($"Unsupported param layout type: {type}");

                    if (value != null)
                        Cells.Add(new Cell(entry, value));
                }

                br.StepOut();
            }

            internal void WriteHeader(BinaryWriterEx bw, byte format2D, int i)
            {
                if ((format2D & 0x7F) < 4)
                {
                    bw.WriteUInt32((uint)ID);
                    bw.ReserveUInt32($"RowOffset{i}");
                    bw.ReserveUInt32($"NameOffset{i}");
                }
                else
                {
                    bw.WriteInt64(ID);
                    bw.ReserveInt64($"RowOffset{i}");
                    bw.ReserveInt64($"NameOffset{i}");
                }
            }

            internal void WriteCells(BinaryWriterEx bw, byte format2D, int i, Layout layout)
            {
                if ((format2D & 0x7F) < 4)
                    bw.FillUInt32($"RowOffset{i}", (uint)bw.Position);
                else
                    bw.FillInt64($"RowOffset{i}", bw.Position);

                for (int j = 0; j < layout.Count; j++)
                {
                    Cell cell = Cells[j];
                    Layout.Entry entry = layout[j];
                    CellType type = entry.Type;
                    object value = cell.Value;

                    if (entry.Name != cell.Name || type != cell.Type)
                        throw new FormatException("Layout does not match cells.");

                    void WriteBools(CellType boolType, int fieldSize)
                    {
                        byte[] b = new byte[fieldSize];
                        int k;
                        for (k = 0; k < fieldSize * 8; k++)
                        {
                            if (j + k >= layout.Count || layout[j + k].Type != boolType)
                                break;

                            if ((bool)Cells[j + k].Value)
                                b[k / 8] |= (byte)(1 << (k % 8));
                        }
                        j += k - 1;
                        bw.WriteBytes(b);
                    }

                    if (type == CellType.s8)
                        bw.WriteSByte((sbyte)value);
                    else if (type == CellType.u8 || type == CellType.x8)
                        bw.WriteByte((byte)value);
                    else if (type == CellType.s16)
                        bw.WriteInt16((short)value);
                    else if (type == CellType.u16 || type == CellType.x16)
                        bw.WriteUInt16((ushort)value);
                    else if (type == CellType.s32)
                        bw.WriteInt32((int)value);
                    else if (type == CellType.u32 || type == CellType.x32)
                        bw.WriteUInt32((uint)value);
                    else if (type == CellType.f32)
                        bw.WriteSingle((float)value);
                    else if (type == CellType.dummy8)
                        bw.WriteBytes((byte[])value);
                    else if (type == CellType.fixstr)
                        bw.WriteFixStr((string)value, entry.Size);
                    else if (type == CellType.fixstrW)
                        bw.WriteFixStrW((string)value, entry.Size);
                    else if (type == CellType.b8)
                        WriteBools(type, 1);
                    else if (type == CellType.b16)
                        WriteBools(type, 2);
                    else if (type == CellType.b32)
                        WriteBools(type, 4);
                }
            }

            internal void WriteName(BinaryWriterEx bw, byte format2D, int i)
            {
                if (Name == null || Name == "")
                {
                    if ((format2D & 0x7F) < 4)
                        bw.FillUInt32($"NameOffset{i}", 0);
                    else
                        bw.FillInt64($"NameOffset{i}", 0);
                }
                else
                {
                    if ((format2D & 0x7F) < 4)
                    {
                        bw.FillUInt32($"NameOffset{i}", (uint)bw.Position);
                        bw.WriteShiftJIS(Name, true);
                    }
                    else
                    {
                        bw.FillInt64($"NameOffset{i}", bw.Position);
                        bw.WriteUTF16(Name, true);
                    }
                }
            }

            /// <summary>
            /// Returns the first cell in the row with the given name.
            /// </summary>
            public Cell this[string name] => Cells.Find(cell => cell.Name == name);
        }

        /// <summary>
        /// One cell in one row in a param.
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// Layout containing name and type of this cell.
            /// </summary>
            public Layout.Entry Layout;

            /// <summary>
            /// The type of value stored in this cell.
            /// </summary>
            public CellType Type => Layout.Type;

            /// <summary>
            /// A name given to this cell based on the param layout; no functional significance.
            /// </summary>
            public string Name => Layout.Name;

            /// <summary>
            /// A description of this field's purpose; may be null.
            /// </summary>
            public string Description => Layout.Description;

            /// <summary>
            /// If not null, the enum containing possible values for this cell.
            /// </summary>
            public string Enum => Layout.Enum;

            /// <summary>
            /// The value of this cell.
            /// </summary>
            public object Value { get; set; }

            internal Cell(Layout.Entry layout, object value)
            {
                Layout = layout;
                Value = value;
            }
        }
    }
}
