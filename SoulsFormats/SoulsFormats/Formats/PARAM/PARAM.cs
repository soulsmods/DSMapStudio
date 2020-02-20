using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// Identifies corresponding params and paramdefs.
        /// </summary>
        public string ParamType { get; set; }

        /// <summary>
        /// Automatically determined based on spacing of row offsets; could be wrong in theory, but never seems to be.
        /// </summary>
        public long DetectedSize { get; private set; }

        /// <summary>
        /// The rows of this param; must be loaded with PARAM.ApplyParamdef() before cells can be used.
        /// </summary>
        public List<Row> Rows { get; set; }

        /// <summary>
        /// The current applied PARAMDEF.
        /// </summary>
        public PARAMDEF AppliedParamdef { get; private set; }

        private BinaryReaderEx RowReader;

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
            RowReader = new BinaryReaderEx(BigEndian, copy);

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
                ParamType = br.ReadFixStr(0x20);
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
                ParamType = br.ReadFixStr(0x20);
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
                ParamType = br.ReadFixStr(0x20);
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
                ParamType = br.GetASCII(idOffset);

                // This is stupid, but the strings offset is always aligned to 0x10,
                // which can put it right in the middle of the ID string
                stringsOffset = idOffset;
            }

            Rows = new List<Row>(rowCount);
            for (int i = 0; i < rowCount; i++)
                Rows.Add(new Row(br, Format2D, Format2E));

            if (Rows.Count > 1)
                DetectedSize = Rows[1].DataOffset - Rows[0].DataOffset;
            else
                DetectedSize = stringsOffset - Rows[0].DataOffset;
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            if (AppliedParamdef == null)
                throw new InvalidOperationException("Params cannot be written without applying a paramdef.");

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
                bw.WriteFixStr(ParamType, 0x20, (byte)((Format2D & 0x7F) < 2 ? 0x20 : 0x00));
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
                bw.WriteFixStr(ParamType, 0x20, 0x20);
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
                bw.WriteFixStr(ParamType, 0x20, 0x00);
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

            if ((Format2D & 0x7F) < 2)
                bw.WritePattern(0x20, 0x00);
            if ((Format2D & 0x7F) < 3)
                bw.FillUInt16("DataStart", (ushort)bw.Position);
            else if ((Format2D & 0x7F) == 3)
                bw.FillUInt32("DataStart", (uint)bw.Position);
            else
                bw.FillInt64("DataStart", bw.Position);

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].WriteCells(bw, Format2D, i);

            bw.FillUInt32("StringsOffset", (uint)bw.Position);

            if ((Format2D & 0x7F) > 4)
            {
                bw.FillInt64("IDOffset", bw.Position);
                bw.WriteASCII(ParamType, true);
            }

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].WriteName(bw, Format2D, Format2E, i);
            // DeS and BB sometimes (but not always) include some useless padding here
        }

        /// <summary>
        /// Interprets row data according to the given paramdef and stores it for later writing.
        /// </summary>
        public void ApplyParamdef(PARAMDEF paramdef)
        {
            AppliedParamdef = paramdef;
            foreach (Row row in Rows)
                row.ReadCells(RowReader, AppliedParamdef);
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
            /// Cells contained in this row. Must be loaded with PARAM.ApplyParamdef() before use.
            /// </summary>
            public IReadOnlyList<Cell> Cells { get; private set; }

            internal long DataOffset;

            /// <summary>
            /// Creates a new row based on the given paramdef with default values.
            /// </summary>
            public Row(long id, string name, PARAMDEF paramdef)
            {
                ID = id;
                Name = name;

                var cells = new Cell[paramdef.Fields.Count];
                for (int i = 0; i < paramdef.Fields.Count; i++)
                {
                    PARAMDEF.Field field = paramdef.Fields[i];
                    object value = ParamUtil.CastDefaultValue(field);
                    cells[i] = new Cell(field, value);
                }
                Cells = cells;
            }

            /// <summary>
            /// Copy constructor for a row. Does not add to the param.
            /// </summary>
            /// <param name="clone">The row that is being copied</param>
            public Row(Row clone)
            {
                ID = clone.ID;
                Name = clone.Name;
                var cells = new List<Cell>(clone.Cells.Count);

                foreach (var cell in clone.Cells)
                {
                    cells.Add(new Cell(cell));
                }
                Cells = cells;
            }

            internal Row(BinaryReaderEx br, byte format2D, byte format2E)
            {
                long nameOffset;
                if ((format2D & 0x7F) < 4)
                {
                    ID = br.ReadUInt32();
                    DataOffset = br.ReadUInt32();
                    nameOffset = br.ReadUInt32();
                }
                else
                {
                    ID = br.ReadInt64();
                    DataOffset = br.ReadInt64();
                    nameOffset = br.ReadInt64();
                }

                if (nameOffset != 0)
                {
                    if (format2E < 7)
                        Name = br.GetShiftJIS(nameOffset);
                    else
                        Name = br.GetUTF16(nameOffset);
                }
            }

            internal void ReadCells(BinaryReaderEx br, PARAMDEF paramdef)
            {
                // In case someone decides to add new rows before applying the paramdef (please don't do that)
                if (DataOffset == 0)
                    return;

                br.Position = DataOffset;
                var cells = new Cell[paramdef.Fields.Count];

                int bitOffset = -1;
                PARAMDEF.DefType bitType = PARAMDEF.DefType.u8;
                uint bitValue = 0;

                for (int i = 0; i < paramdef.Fields.Count; i++)
                {
                    PARAMDEF.Field field = paramdef.Fields[i];
                    object value = null;
                    PARAMDEF.DefType type = field.DisplayType;

                    if (type == PARAMDEF.DefType.s8)
                        value = br.ReadSByte();
                    else if (type == PARAMDEF.DefType.s16)
                        value = br.ReadInt16();
                    else if (type == PARAMDEF.DefType.s32)
                        value = br.ReadInt32();
                    else if (type == PARAMDEF.DefType.f32)
                        value = br.ReadSingle();
                    else if (type == PARAMDEF.DefType.fixstr)
                        value = br.ReadFixStr(field.ArrayLength);
                    else if (type == PARAMDEF.DefType.fixstrW)
                        value = br.ReadFixStrW(field.ArrayLength * 2);
                    else if (ParamUtil.IsBitType(type))
                    {
                        if (field.BitSize == -1)
                        {
                            if (type == PARAMDEF.DefType.u8)
                                value = br.ReadByte();
                            else if (type == PARAMDEF.DefType.u16)
                                value = br.ReadUInt16();
                            else if (type == PARAMDEF.DefType.u32)
                                value = br.ReadUInt32();
                            else if (type == PARAMDEF.DefType.dummy8)
                                value = br.ReadBytes(field.ArrayLength);
                        }
                    }
                    else
                        throw new NotImplementedException($"Unsupported field type: {type}");

                    if (value != null)
                    {
                        bitOffset = -1;
                    }
                    else
                    {
                        PARAMDEF.DefType newBitType = type == PARAMDEF.DefType.dummy8 ? PARAMDEF.DefType.u8 : type;
                        int bitLimit = ParamUtil.GetBitLimit(newBitType);

                        if (field.BitSize == 0)
                            throw new NotImplementedException($"Bit size 0 is not supported.");
                        if (field.BitSize > bitLimit)
                            throw new InvalidDataException($"Bit size {field.BitSize} is too large to fit in type {newBitType}.");

                        if (bitOffset == -1 || newBitType != bitType || bitOffset + field.BitSize > bitLimit)
                        {
                            bitOffset = 0;
                            bitType = newBitType;
                            if (bitType == PARAMDEF.DefType.u8)
                                bitValue = br.ReadByte();
                            else if (bitType == PARAMDEF.DefType.u16)
                                bitValue = br.ReadUInt16();
                            else if (bitType == PARAMDEF.DefType.u32)
                                bitValue = br.ReadUInt32();
                        }

                        uint shifted = bitValue << (32 - field.BitSize - bitOffset) >> (32 - field.BitSize);
                        bitOffset += field.BitSize;
                        if (bitType == PARAMDEF.DefType.u8)
                            value = (byte)shifted;
                        else if (bitType == PARAMDEF.DefType.u16)
                            value = (ushort)shifted;
                        else if (bitType == PARAMDEF.DefType.u32)
                            value = shifted;
                    }

                    cells[i] = new Cell(field, value);
                }
                Cells = cells;
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

            internal void WriteCells(BinaryWriterEx bw, byte format2D, int index)
            {
                if ((format2D & 0x7F) < 4)
                    bw.FillUInt32($"RowOffset{index}", (uint)bw.Position);
                else
                    bw.FillInt64($"RowOffset{index}", bw.Position);

                int bitOffset = -1;
                PARAMDEF.DefType bitType = PARAMDEF.DefType.u8;
                uint bitValue = 0;

                for (int i = 0; i < Cells.Count; i++)
                {
                    Cell cell = Cells[i];
                    object value = cell.Value;
                    PARAMDEF.Field field = cell.Def;
                    PARAMDEF.DefType type = field.DisplayType;

                    if (type == PARAMDEF.DefType.s8)
                        bw.WriteSByte((sbyte)value);
                    else if (type == PARAMDEF.DefType.s16)
                        bw.WriteInt16((short)value);
                    else if (type == PARAMDEF.DefType.s32)
                        bw.WriteInt32((int)value);
                    else if (type == PARAMDEF.DefType.f32)
                        bw.WriteSingle((float)value);
                    else if (type == PARAMDEF.DefType.fixstr)
                        bw.WriteFixStr((string)value, field.ArrayLength);
                    else if (type == PARAMDEF.DefType.fixstrW)
                        bw.WriteFixStrW((string)value, field.ArrayLength * 2);
                    else if (ParamUtil.IsBitType(type))
                    {
                        if (field.BitSize == -1)
                        {
                            if (type == PARAMDEF.DefType.u8)
                                bw.WriteByte((byte)value);
                            else if (type == PARAMDEF.DefType.u16)
                                bw.WriteUInt16((ushort)value);
                            else if (type == PARAMDEF.DefType.u32)
                                bw.WriteUInt32((uint)value);
                            else if (type == PARAMDEF.DefType.dummy8)
                                bw.WriteBytes((byte[])value);
                        }
                        else
                        {
                            if (bitOffset == -1)
                            {
                                bitOffset = 0;
                                bitType = type == PARAMDEF.DefType.dummy8 ? PARAMDEF.DefType.u8 : type;
                                bitValue = 0;
                            }

                            uint shifted = 0;
                            if (bitType == PARAMDEF.DefType.u8)
                                shifted = (byte)value;
                            else if (bitType == PARAMDEF.DefType.u16)
                                shifted = (ushort)value;
                            else if (bitType == PARAMDEF.DefType.u32)
                                shifted = (uint)value;
                            // Shift left first to clear any out-of-range bits
                            shifted = shifted << (32 - field.BitSize) >> (32 - field.BitSize - bitOffset);
                            bitValue |= shifted;
                            bitOffset += field.BitSize;

                            bool write = false;
                            if (i == Cells.Count - 1)
                            {
                                write = true;
                            }
                            else
                            {
                                PARAMDEF.Field nextField = Cells[i + 1].Def;
                                PARAMDEF.DefType nextType = nextField.DisplayType;
                                int bitLimit = ParamUtil.GetBitLimit(bitType);
                                if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                                    || (nextType == PARAMDEF.DefType.dummy8 ? PARAMDEF.DefType.u8 : nextType) != bitType)
                                {
                                    write = true;
                                }
                            }

                            if (write)
                            {
                                bitOffset = -1;
                                if (bitType == PARAMDEF.DefType.u8)
                                    bw.WriteByte((byte)bitValue);
                                else if (bitType == PARAMDEF.DefType.u16)
                                    bw.WriteUInt16((ushort)bitValue);
                                else if (bitType == PARAMDEF.DefType.u32)
                                    bw.WriteUInt32(bitValue);
                            }
                        }
                    }
                    else
                        throw new NotImplementedException($"Unsupported field type: {type}");
                }
            }

            internal void WriteName(BinaryWriterEx bw, byte format2D, byte format2E, int i)
            {
                long nameOffset = 0;
                if (Name != null)
                {
                    nameOffset = bw.Position;
                    if (format2E < 7)
                        bw.WriteShiftJIS(Name, true);
                    else
                        bw.WriteUTF16(Name, true);
                }

                if ((format2D & 0x7F) < 4)
                    bw.FillUInt32($"NameOffset{i}", (uint)nameOffset);
                else
                    bw.FillInt64($"NameOffset{i}", nameOffset);
            }

            /// <summary>
            /// Returns a string representation of the row.
            /// </summary>
            public override string ToString()
            {
                return $"{ID} {Name}";
            }

            /// <summary>
            /// Returns the first cell in the row with the given internal name.
            /// </summary>
            public Cell this[string name] => Cells.First(cell => cell.Def.InternalName == name);
        }

        /// <summary>
        /// One cell in one row in a param.
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// The paramdef field that describes this cell.
            /// </summary>
            public PARAMDEF.Field Def { get; }

            /// <summary>
            /// The value of this cell.
            /// </summary>
            public object Value { get; set; }

            internal Cell(PARAMDEF.Field def, object value)
            {
                Def = def;
                Value = value;
            }

            internal Cell(Cell clone)
            {
                Def = clone.Def;
                Value = clone.Value;
            }

            /// <summary>
            /// Returns a string representation of the cell.
            /// </summary>
            public override string ToString()
            {
                return $"{Def.DisplayType} {Def.InternalName} = {Value}";
            }
        }
    }
}
