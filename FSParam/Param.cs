using System.Data;
using System.Runtime.InteropServices;
using System.Security;
using SoulsFormats;
using StudioUtils;

namespace FSParam
{
    /// <summary>
    /// An alternative to the SoulsFormats param class that's designed to be faster to read/write and be
    /// much more memory efficient. This goes a bit against idiomatic C# in the name of efficiency, so the
    /// API is admittedly a little awkward.
    /// </summary>
    public class Param : SoulsFile<Param>
    {
        /// <summary>
        /// First set of flags indicating file format; highly speculative.
        /// </summary>
        [Flags]
        public enum FormatFlags1 : byte
        {
            /// <summary>
            /// No flags set.
            /// </summary>
            None = 0,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag01 = 0b0000_0001,

            /// <summary>
            /// Expanded header with 32-bit data offset.
            /// </summary>
            IntDataOffset = 0b0000_0010,

            /// <summary>
            /// Expanded header with 64-bit data offset.
            /// </summary>
            LongDataOffset = 0b0000_0100,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag08 = 0b0000_1000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag10 = 0b0001_0000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag20 = 0b0010_0000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag40 = 0b0100_0000,

            /// <summary>
            /// Param type string is written separately instead of fixed-width in the header.
            /// </summary>
            OffsetParamType = 0b1000_0000,
        }

        /// <summary>
        /// Second set of flags indicating file format; highly speculative.
        /// </summary>
        [Flags]
        public enum FormatFlags2 : byte
        {
            /// <summary>
            /// No flags set.
            /// </summary>
            None = 0,

            /// <summary>
            /// Row names are written as UTF-16.
            /// </summary>
            UnicodeRowNames = 0b0000_0001,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag02 = 0b0000_0010,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag04 = 0b0000_0100,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag08 = 0b0000_1000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag10 = 0b0001_0000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag20 = 0b0010_0000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag40 = 0b0100_0000,

            /// <summary>
            /// Unused?
            /// </summary>
            Flag80 = 0b1000_0000,
        }

        public class Row
        {
            internal Param Parent;
            public int ID { get; set; }
            public string? Name { get; set; }
            internal uint DataIndex;

            public IEnumerable<Cell> Cells => Parent.Cells;

            public PARAMDEF Def => Parent.AppliedParamdef;

            internal Row(int id, string? name, Param parent, uint dataIndex)
            {
                ID = id;
                Name = name;
                Parent = parent;
                DataIndex = dataIndex;
            }
            
            public Row(int id, string name, Param parent)
            {
                ID = id;
                Name = name;
                Parent = parent;
                DataIndex = parent.ParamData.AddZeroedElement();
            }

            public Row(Row clone)
            {
                Parent = clone.Parent;
                ID = clone.ID;
                Name = clone.Name;
                DataIndex = Parent.ParamData.AddZeroedElement();
                Parent.ParamData.CopyData(DataIndex, clone.DataIndex);
            }

            ~Row()
            {
                Parent.ParamData.RemoveAt(DataIndex);
            }

            public CellHandle? this[string field]
            {
                get
                {
                    var cell = Cells.FirstOrDefault(cell => cell.Def.InternalName == field);
                    return cell != null ? new CellHandle(this, cell) : null;
                }
            }
            public CellHandle this[Cell field] => new CellHandle(this, field);
        }

        /// <summary>
        /// Minimal handle of a cell in a row that contains enough to mutate the value of the cell and created
        /// on demand
        /// </summary>
        public struct CellHandle
        {
            private Row _row;
            private Cell _cell;

            internal CellHandle(Row row, Cell cell)
            {
                _row = row;
                _cell = cell;
            }

            public object Value
            {
                get => _cell.GetValue(_row);
                set => _cell.SetValue(_row, value);
            }

            public PARAMDEF.Field Def => _cell.Def;
        }
        
        /// <summary>
        /// Represents a Cell (key/value pair) in the param. Unlike the Soulsformats Cell, this one is stored
        /// completely separately, and reading/writing a value requires the Row to read/write from.
        /// </summary>
        public class Cell
        {
            public PARAMDEF.Field Def { get; }

            public Type ValueType { get; private set; }
            
            private uint _byteOffset;
            private uint _arrayLength;
            private int _bitSize;
            private uint _bitOffset;
            
            internal Cell(PARAMDEF.Field def, uint byteOffset, uint arrayLength = 1)
            {
                Def = def;
                _byteOffset = byteOffset;
                _arrayLength = arrayLength;
                _bitSize = -1;
                _bitOffset = 0;
            }

            internal Cell(PARAMDEF.Field def, uint byteOffset, int bitSize, uint bitOffset)
            {
                Def = def;
                _byteOffset = byteOffset;
                _arrayLength = 1;
                _bitSize = bitSize;
                _bitOffset = bitOffset;
            }

            public object GetValue(Row row)
            {
                var data = row.Parent.ParamData;
                switch (Def.DisplayType)
                {
                    case PARAMDEF.DefType.s8:
                        return data.ReadValueAtElementOffset<sbyte>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.s16:
                        return data.ReadValueAtElementOffset<short>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.s32:
                        return data.ReadValueAtElementOffset<int>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.f32:
                        return data.ReadValueAtElementOffset<float>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.u8:
                    case PARAMDEF.DefType.dummy8:
                        var value8 = data.ReadValueAtElementOffset<byte>(row.DataIndex, _byteOffset);
                        if (_bitSize != -1)
                            value8 = (byte)((value8 >> (int)_bitOffset) & (0xFF >> (8 - _bitSize)));
                        return value8;
                    case PARAMDEF.DefType.u16:
                        var value16 = data.ReadValueAtElementOffset<ushort>(row.DataIndex, _byteOffset);
                        if (_bitSize != -1)
                            value16 = (ushort)((value16 >> (int)_bitOffset) & (0xFFFF >> (16 - _bitSize)));
                        return value16;
                    case PARAMDEF.DefType.u32:
                        var value32 = data.ReadValueAtElementOffset<uint>(row.DataIndex, _byteOffset);
                        if (_bitSize != -1)
                            value32 = (uint)((value32 >> (int)_bitOffset) & (0xFFFFFFFF >> (32 - _bitSize)));
                        return value32;
                    case PARAMDEF.DefType.fixstr:
                        return data.ReadFixedStringAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                    case PARAMDEF.DefType.fixstrW:
                        return data.ReadFixedStringWAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                    default:
                        throw new NotImplementedException($"Unsupported field type: {Def.DisplayType}");
                }
            }

            public void SetValue(Row row, object value)
            {
                var data = row.Parent.ParamData;
                switch (Def.DisplayType)
                {
                    case PARAMDEF.DefType.s8:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (sbyte)value);
                        break;
                    case PARAMDEF.DefType.s16:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (short)value);
                        break;
                    case PARAMDEF.DefType.s32:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (int)value);
                        break;
                    case PARAMDEF.DefType.f32:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (float)value);
                        break;
                    case PARAMDEF.DefType.u8:
                    case PARAMDEF.DefType.dummy8:
                        var value8 = (byte)value;
                        if (_bitSize != -1)
                        {
                            var o8 = data.ReadValueAtElementOffset<byte>(row.DataIndex, _byteOffset);
                            var mask8 = (byte)(0xFF >> (8 - _bitSize) << (int)_bitOffset);
                            value8 = (byte)((o8 & ~mask8) | ((value8 << (int)_bitOffset) & mask8));
                        }
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value8);
                        break;
                    case PARAMDEF.DefType.u16:
                        var value16 = (ushort)value;
                        if (_bitSize != -1)
                        {
                            var o16 = data.ReadValueAtElementOffset<ushort>(row.DataIndex, _byteOffset);
                            var mask16 = (ushort)(0xFFFF >> (16 - _bitSize) << (int)_bitOffset);
                            value16 = (ushort)((o16 & ~mask16) | ((value16 << (int)_bitOffset) & mask16));
                        }
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value16);
                        break;
                    case PARAMDEF.DefType.u32:
                        var value32 = (uint)value;
                        if (_bitSize != -1)
                        {
                            var o32 = data.ReadValueAtElementOffset<uint>(row.DataIndex, _byteOffset);
                            var mask32 = (uint)(0xFFFFFFFF >> (32 - _bitSize) << (int)_bitOffset);
                            value32 = (uint)((o32 & ~mask32) | ((value32 << (int)_bitOffset) & mask32));
                        }
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value32);
                        break;
                    case PARAMDEF.DefType.fixstr:
                        data.WriteFixedStringAtElementOffset(row.DataIndex, _byteOffset, (string)value, _arrayLength);
                        break;
                    case PARAMDEF.DefType.fixstrW:
                        data.WriteFixedStringWAtElementOffset(row.DataIndex, _byteOffset, (string)value, _arrayLength);
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported field type: {Def.DisplayType}");
                }
            }
        }
        
        /// <summary>
        /// Whether the file is big-endian; true for PS3/360 files, false otherwise.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// Flags indicating format of the file.
        /// </summary>
        public FormatFlags1 Format2D { get; set; }

        /// <summary>
        /// More flags indicating format of the file.
        /// </summary>
        public FormatFlags2 Format2E { get; set; }

        /// <summary>
        /// Originally matched the paramdef for version 101, but since is always 0 or 0xFF.
        /// </summary>
        public byte ParamdefFormatVersion { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public short Unk06 { get; set; }

        /// <summary>
        /// Indicates a revision of the row data structure.
        /// </summary>
        public short ParamdefDataVersion { get; set; }

        /// <summary>
        /// Identifies corresponding params and paramdefs.
        /// </summary>
        public string ParamType { get; set; }

        /// <summary>
        /// Automatically determined based on spacing of row offsets; 0 if param had no rows.
        /// </summary>
        public uint DetectedSize { get; private set; }

        public StridedByteArray ParamData { get; private set; } = new StridedByteArray(0, 1);

        private List<Row> _rows = new List<Row>();
        public List<Row> Rows 
        { 
            get => _rows;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                
                if (Rows.Any(r => r.Parent != this))
                {
                    throw new ArgumentException("Attempting to add rows created from another Param");
                }
                _rows = value;
            } 
        }
        
        public IReadOnlyList<Cell> Cells { get; private set; }

        public PARAMDEF AppliedParamdef { get; private set; }

        public void ApplyParamdef(PARAMDEF def)
        {
            AppliedParamdef = def;
            var cells = new List<Cell>(def.Fields.Count);
            
            int bitOffset = -1;
            uint byteOffset = 0;
            uint lastSize = 0;
            PARAMDEF.DefType bitType = PARAMDEF.DefType.u8;

            for (int i = 0; i < def.Fields.Count; i++)
            {
                PARAMDEF.Field field = def.Fields[i];
                PARAMDEF.DefType type = field.DisplayType;
                bool isBitType = ParamUtil.IsBitType(type);
                if (!isBitType || (isBitType && field.BitSize == -1))
                {
                    // Advance the offset if we were last reading bits
                    if (bitOffset != -1)
                        byteOffset += lastSize;
                    
                    cells.Add(ParamUtil.IsArrayType(type)
                        ? new Cell(field, byteOffset, (uint)field.ArrayLength)
                        : new Cell(field, byteOffset));
                    switch (type)
                    {
                        case PARAMDEF.DefType.s8:
                        case PARAMDEF.DefType.u8:
                            byteOffset += 1;
                            break;
                        case PARAMDEF.DefType.s16:
                        case PARAMDEF.DefType.u16:
                            byteOffset += 2;
                            break;
                        case PARAMDEF.DefType.s32:
                        case PARAMDEF.DefType.u32:
                        case PARAMDEF.DefType.f32:
                            byteOffset += 4;
                            break;
                        case PARAMDEF.DefType.fixstr:
                        case PARAMDEF.DefType.dummy8:
                            byteOffset += (uint)field.ArrayLength;
                            break;
                        case PARAMDEF.DefType.fixstrW:
                            byteOffset += (uint)field.ArrayLength * 2;
                            break;
                        default:
                            throw new NotImplementedException($"Unsupported field type: {type}");
                    }
                    
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

                    lastSize = (uint)ParamUtil.GetValueSize(newBitType);
                    if (bitOffset == -1 || newBitType != bitType || bitOffset + field.BitSize > bitLimit)
                    {
                        if (bitOffset != -1)
                            byteOffset += lastSize;
                        bitOffset = 0;
                        bitType = newBitType;
                    }
                    
                    cells.Add(new Cell(field, byteOffset, field.BitSize, (uint)bitOffset));
                    bitOffset += field.BitSize;
                }
            }

            Cells = cells;
        }
        
        protected override void Read(BinaryReaderEx br)
        {
            br.Position = 0x2C;
            br.BigEndian = BigEndian = br.AssertByte(0, 0xFF) == 0xFF;
            Format2D = (FormatFlags1)br.ReadByte();
            Format2E = (FormatFlags2)br.ReadByte();
            ParamdefFormatVersion = br.ReadByte();
            br.Position = 0;
            
            // The strings offset in the header is highly unreliable; only use it as a last resort
            long actualStringsOffset = 0;
            long stringsOffset = br.ReadUInt32();
            if (Format2D.HasFlag(FormatFlags1.Flag01) && Format2D.HasFlag(FormatFlags1.IntDataOffset) || Format2D.HasFlag(FormatFlags1.LongDataOffset))
            {
                br.AssertInt16(0);
            }
            else
            {
                br.ReadUInt16(); // Data start
            }
            Unk06 = br.ReadInt16();
            ParamdefDataVersion = br.ReadInt16();
            ushort rowCount = br.ReadUInt16();
            if (Format2D.HasFlag(FormatFlags1.OffsetParamType))
            {
                br.AssertInt32(0);
                long paramTypeOffset = br.ReadInt64();
                br.AssertPattern(0x14, 0x00);
                ParamType = br.GetASCII(paramTypeOffset);
                actualStringsOffset = paramTypeOffset;
            }
            else
            {
                ParamType = br.ReadFixStr(0x20);
            }
            br.Skip(4); // Format
            if (Format2D.HasFlag(FormatFlags1.Flag01) && Format2D.HasFlag(FormatFlags1.IntDataOffset))
            {
                br.ReadInt32(); // Data start
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);
            }
            else if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
            {
                br.ReadInt64(); // Data start
                br.AssertInt64(0);
            }

            Rows = new List<Row>(rowCount);
            for (var i = 0; i < rowCount; i++)
            {
                long nameOffset;
                int id;
                string? name = null;
                uint dataIndex;
                if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                {
                    id = br.ReadInt32();
                    br.ReadInt32(); // I would like to assert 0, but some of the generatordbglocation params in DS2S have garbage here
                    dataIndex = (uint)br.ReadInt64();
                    nameOffset = br.ReadInt64();
                }
                else
                {
                    id = br.ReadInt32();
                    dataIndex = br.ReadUInt32();
                    nameOffset = br.ReadUInt32();
                }

                if (nameOffset != 0)
                {
                    if (actualStringsOffset == 0 || nameOffset < actualStringsOffset)
                        actualStringsOffset = nameOffset;

                    if (Format2E.HasFlag(FormatFlags2.UnicodeRowNames))
                        name = br.GetUTF16(nameOffset);
                    else
                        name = br.GetShiftJIS(nameOffset);
                }
                Rows.Add(new Row(id, name, this, dataIndex));
            }
            
            if (Rows.Count > 1)
                DetectedSize = Rows[1].DataIndex - Rows[0].DataIndex;
            else if (Rows.Count == 1)
                DetectedSize = (actualStringsOffset == 0 ? (uint)stringsOffset : (uint)actualStringsOffset) - Rows[0].DataIndex;
            else
                DetectedSize = 0;

            if (Rows.Count > 0)
            {
                var dataStart = Rows.Min(row => row.DataIndex);
                br.Position = dataStart;
                var rowData = br.ReadBytes(Rows.Count * (int)DetectedSize);
                ParamData = new StridedByteArray(rowData, DetectedSize, BigEndian);
                
                // Convert raw data offsets into indices
                foreach (var r in Rows)
                {
                    r.DataIndex = (r.DataIndex - dataStart) / DetectedSize;
                }
            }
        }

        protected override void Write(BinaryWriterEx bw)
        {
            
        }

        /// <summary>
        /// Gets the index of the Row with ID id or returns null
        /// </summary>
        /// <param name="id">The ID of the row to find</param>
        public Row? this[int id]
        {
            get
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i].ID == id)
                        return Rows[i];
                }

                return null;
            }
        }

        public Cell? this[string name] => Cells.FirstOrDefault(cell => cell.Def.InternalName == name);
    }
}