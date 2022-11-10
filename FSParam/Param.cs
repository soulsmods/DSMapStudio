using System.Data;
using System.Runtime.InteropServices;
using System.Security;
using SoulsFormats;
using StudioUtils;

namespace FSParam
{
    /// <summary>
    /// An alternative to the SoulsFormats param class that's designed to be faster to read/write and be
    /// much more memory efficient. This tries to match the SoulsFormats PARAM API as much as possible but
    /// has some differences out of necessity. The main difference is rows and cells are separate rather
    /// than each row having an array of cells. For convenience, a CellHandle struct was added that provides
    /// a similar API to the SoulsFormats Cell.
    ///
    /// A lot of this code is based off the SoulsFormats PARAM class (especially the read/write), so thanks TKGP.
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

            public IEnumerable<Column> Cells => Parent.Cells;

            public IReadOnlyList<Cell> CellHandles
            {
                get
                {
                    var cells = new List<Cell>(Cells.Count());
                    foreach (var cell in Cells)
                    {
                        cells.Add(new Cell(this, cell));
                    }

                    return cells;
                }
            }

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
                DataIndex = parent._paramData.AddZeroedElement();
            }

            public Row(Row clone)
            {
                Parent = clone.Parent;
                ID = clone.ID;
                Name = clone.Name;
                DataIndex = Parent._paramData.AddZeroedElement();
                Parent._paramData.CopyData(DataIndex, clone.DataIndex);
            }
            
            public Row(Row clone, Param newParent)
            {
                Parent = newParent;
                ID = clone.ID;
                Name = clone.Name;
                DataIndex = Parent._paramData.AddZeroedElement();
                clone.Parent._paramData.CopyData(Parent._paramData, DataIndex, clone.DataIndex);
            }

            public bool DataEquals(Row? other)
            {
                if (other == null)
                    return false;
                if (ID != other.ID)
                    return false;

                return Parent._paramData.DataEquals(other.Parent._paramData, other.DataIndex, DataIndex);
            }

            ~Row()
            {
                Parent._paramData.RemoveAt(DataIndex);
            }

            /// <summary>
            /// Gets a cell handle from a name or throw an exception if the field name is not found
            /// </summary>
            /// <param name="field">The field to look for</param>
            /// <returns>A cell handle for the field</returns>
            /// <exception cref="ArgumentException">Throws if field name doesn't exist</exception>
            public Cell GetCellHandleOrThrow(string field)
            {
                var cell = Cells.FirstOrDefault(cell => cell.Def.InternalName == field);
                if (cell == null)
                    throw new ArgumentException();
                return new Cell(this, cell);
            }
            
            public Cell? this[string field]
            {
                get
                {
                    var cell = Cells.FirstOrDefault(cell => cell.Def.InternalName == field);
                    return cell != null ? new Cell(this, cell) : null;
                }
            }
            public Cell this[Column field] => new Cell(this, field);
        }

        /// <summary>
        /// Minimal handle of a cell in a row that contains enough to mutate the value of the cell and created
        /// on demand
        /// </summary>
        public struct Cell
        {
            private Row _row;
            private Column _column;

            internal Cell(Row row, Column column)
            {
                _row = row;
                _column = column;
            }

            public object Value
            {
                get => _column.GetValue(_row);
                set => _column.SetValue(_row, value);
            }

            public void SetValue(object value)
            {
                _column.SetValue(_row, value);
            }

            public PARAMDEF.Field Def => _column.Def;
        }
        
        /// <summary>
        /// Represents a Column (param field) in the param. Unlike the Soulsformats Cell, this one is stored
        /// completely separately, and reading/writing a value requires the Row to read/write from.
        /// </summary>
        public class Column
        {
            public PARAMDEF.Field Def { get; }

            public Type ValueType { get; private set; }
            
            private uint _byteOffset;
            private uint _arrayLength;
            private int _bitSize;
            private uint _bitOffset;
            
            internal Column(PARAMDEF.Field def, uint byteOffset, uint arrayLength = 1)
            {
                Def = def;
                _byteOffset = byteOffset;
                _arrayLength = arrayLength;
                _bitSize = -1;
                _bitOffset = 0;
                ValueType = TypeForParamDefType(def.DisplayType, arrayLength > 1);
            }

            internal Column(PARAMDEF.Field def, uint byteOffset, int bitSize, uint bitOffset)
            {
                Def = def;
                _byteOffset = byteOffset;
                _arrayLength = 1;
                _bitSize = bitSize;
                _bitOffset = bitOffset;
                ValueType = TypeForParamDefType(def.DisplayType, false);
            }
            
            private static Type TypeForParamDefType(PARAMDEF.DefType type, bool isArray)
            {
                switch (type)
                {
                    case PARAMDEF.DefType.s8:
                        return typeof(sbyte);
                    case PARAMDEF.DefType.u8:
                        return typeof(byte);
                    case PARAMDEF.DefType.s16:
                        return typeof(short);
                    case PARAMDEF.DefType.u16:
                        return typeof(ushort);
                    case PARAMDEF.DefType.s32:
                    case PARAMDEF.DefType.b32:
                        return typeof(int);
                    case PARAMDEF.DefType.u32:
                        return typeof(uint);
                    case PARAMDEF.DefType.f32: 
                    case PARAMDEF.DefType.angle32:
                        return typeof(float);
                    case PARAMDEF.DefType.f64:
                        return typeof(double);
                    case PARAMDEF.DefType.dummy8:
                        return isArray ? typeof(byte[]) : typeof(byte);
                    case PARAMDEF.DefType.fixstr:
                    case PARAMDEF.DefType.fixstrW:
                        return typeof(string);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }

            public object GetValue(Row row)
            {
                var data = row.Parent._paramData;
                switch (Def.DisplayType)
                {
                    case PARAMDEF.DefType.s8:
                        return data.ReadValueAtElementOffset<sbyte>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.s16:
                        return data.ReadValueAtElementOffset<short>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.s32:
                    case PARAMDEF.DefType.b32:
                        return data.ReadValueAtElementOffset<int>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.f32:
                    case PARAMDEF.DefType.angle32:
                        return data.ReadValueAtElementOffset<float>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.f64:
                        return data.ReadValueAtElementOffset<double>(row.DataIndex, _byteOffset);
                    case PARAMDEF.DefType.u8:
                    case PARAMDEF.DefType.dummy8:
                        if (_arrayLength > 1)
                        {
                            return data.ReadByteArrayAtElementOffset(row.DataIndex, _byteOffset, _arrayLength);
                        }
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
                var data = row.Parent._paramData;
                switch (Def.DisplayType)
                {
                    case PARAMDEF.DefType.s8:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (sbyte)value);
                        break;
                    case PARAMDEF.DefType.s16:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (short)value);
                        break;
                    case PARAMDEF.DefType.s32:
                    case PARAMDEF.DefType.b32:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (int)value);
                        break;
                    case PARAMDEF.DefType.f32:
                    case PARAMDEF.DefType.angle32:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (float)value);
                        break;
                    case PARAMDEF.DefType.f64:
                        data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, (double)value);
                        break;
                    case PARAMDEF.DefType.u8:
                    case PARAMDEF.DefType.dummy8:
                        if (_arrayLength > 1)
                        {
                            data.WriteByteArrayAtElementOffset(row.DataIndex, _byteOffset, (byte[])value);
                        }
                        else
                        {
                            var value8 = (byte)value;
                            if (_bitSize != -1)
                            {
                                var o8 = data.ReadValueAtElementOffset<byte>(row.DataIndex, _byteOffset);
                                var mask8 = (byte)(0xFF >> (8 - _bitSize) << (int)_bitOffset);
                                value8 = (byte)((o8 & ~mask8) | ((value8 << (int)_bitOffset) & mask8));
                            }

                            data.WriteValueAtElementOffset(row.DataIndex, _byteOffset, value8);
                        }

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

        private StridedByteArray _paramData = new StridedByteArray(0, 1);

        private List<Row> _rows = new List<Row>();
        public IReadOnlyList<Row> Rows 
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

                _rows = new List<Row>(value);
            } 
        }
        
        public IReadOnlyList<Column> Cells { get; private set; }

        public PARAMDEF AppliedParamdef { get; private set; }

        public Param()
        {
            
        }
        
        /// <summary>
        /// Creates a new empty param inheriting config/paramdef from a source
        /// </summary>
        /// <param name="source"></param>
        public Param(Param source)
        {
            BigEndian = source.BigEndian;
            Format2D = source.Format2D;
            Format2E = source.Format2E;
            ParamdefFormatVersion = source.ParamdefFormatVersion;
            Unk06 = source.Unk06;
            ParamdefDataVersion = source.ParamdefDataVersion;
            ParamType = source.ParamType;
            DetectedSize = source.DetectedSize;
            _paramData = new StridedByteArray((uint)source._rows.Count, DetectedSize, BigEndian);
            AppliedParamdef = source.AppliedParamdef;
            ApplyParamdef(AppliedParamdef);
        }
        
        public void ClearRows()
        {
            _rows.Clear();
        }
        
        public void AddRow(Row row)
        {
            if (row.Parent != this)
                throw new ArgumentException();
            _rows.Add(row);
        }

        public void InsertRow(int index, Row row)
        {
            if (row.Parent != this)
                throw new ArgumentException();
            _rows.Insert(index, row);
        }

        public int IndexOfRow(Row? row)
        {
            if (row == null || row.Parent != this)
                throw new ArgumentException();
            return _rows.IndexOf(row);
        }

        public void RemoveRow(Row row)
        {
            if (row.Parent != this)
                throw new ArgumentException();
            _rows.Remove(row);
        }

        public void RemoveRowAt(int index)
        {
            _rows.RemoveAt(index);
        }

        public void ApplyParamdef(PARAMDEF def)
        {
            AppliedParamdef = def;
            var cells = new List<Column>(def.Fields.Count);
            
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
                        ? new Column(field, byteOffset, (uint)field.ArrayLength)
                        : new Column(field, byteOffset));
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
                        case PARAMDEF.DefType.b32:
                        case PARAMDEF.DefType.angle32:
                            byteOffset += 4;
                            break;
                        case PARAMDEF.DefType.f64:
                            byteOffset += 8;
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
                    
                    cells.Add(new Column(field, byteOffset, field.BitSize, (uint)bitOffset));
                    bitOffset += field.BitSize;
                }
            }
            
            // Get the final size and sanity check against our calculated row size
            if (bitOffset != -1)
                byteOffset += lastSize;
            if (byteOffset != DetectedSize)
                throw new Exception($@"Row size paramdef mismatch for {ParamType}");

            Cells = cells;
        }

        /// <summary>
        /// A bug in prior versions of DSMS and other param editors would save soundCutsceneParam as
        /// 32 bytes instead of 36 bytes. Fortunately appending 0s at the end should be enough to fix
        /// these params.
        /// </summary>
        private void FixupERSoundCutsceneParam()
        {
            StridedByteArray newData = new StridedByteArray((uint)Rows.Count, 36, BigEndian);
            for (int i = 0; i < Rows.Count; i++)
            {
                newData.AddZeroedElement();
                _paramData.CopyData(newData, (uint)i, (uint)i);
            }

            _paramData = newData;
            DetectedSize = 36;
        }
        
        /// <summary>
        /// People were using Yapped and other param editors to save botched ER 1.06 params, so we need
        /// to fix them up again. Fortunately the only modified paramdef was ChrModelParam, and the new
        /// field is always 0, so we can easily fix them.
        /// </summary>
        public void FixupERChrModelParam()
        {
            if (DetectedSize != 12)
                return;
            StridedByteArray newData = new StridedByteArray((uint)Rows.Count, 16, BigEndian);
            for (int i = 0; i < Rows.Count; i++)
            {
                newData.AddZeroedElement();
                _paramData.CopyData(newData, (uint)i, (uint)i);
            }

            _paramData = newData;
            DetectedSize = 16;
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
                _rows.Add(new Row(id, name, this, dataIndex));
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
                _paramData = new StridedByteArray(rowData, DetectedSize, BigEndian);
                
                // Convert raw data offsets into indices
                foreach (var r in Rows)
                {
                    r.DataIndex = (r.DataIndex - dataStart) / DetectedSize;
                }
            }

            if (ParamType == "SOUND_CUTSCENE_PARAM_ST" && ParamdefDataVersion == 6 && DetectedSize == 32)
            {
                FixupERSoundCutsceneParam();
            }
        }

        protected override void Write(BinaryWriterEx bw)
        {
            if (AppliedParamdef == null)
                throw new InvalidOperationException("Params cannot be written without applying a paramdef.");
            
            bw.BigEndian = BigEndian;
            
            bw.ReserveUInt32("StringsOffset");
            if (Format2D.HasFlag(FormatFlags1.Flag01) && Format2D.HasFlag(FormatFlags1.IntDataOffset) || Format2D.HasFlag(FormatFlags1.LongDataOffset))
            {
                bw.WriteInt16(0);
            }
            else
            {
                bw.ReserveUInt16("DataStart");
            }
            bw.WriteInt16(Unk06);
            bw.WriteInt16(ParamdefDataVersion);
            bw.WriteUInt16((ushort)Rows.Count);
            if (Format2D.HasFlag(FormatFlags1.OffsetParamType))
            {
                bw.WriteInt32(0);
                bw.ReserveInt64("ParamTypeOffset");
                bw.WritePattern(0x14, 0x00);
            }
            else
            {
                // This padding heuristic isn't completely accurate, not that it matters
                bw.WriteFixStr(ParamType, 0x20, (byte)(Format2D.HasFlag(FormatFlags1.Flag01) ? 0x20 : 0x00));
            }
            bw.WriteByte((byte)(BigEndian ? 0xFF : 0x00));
            bw.WriteByte((byte)Format2D);
            bw.WriteByte((byte)Format2E);
            bw.WriteByte(ParamdefFormatVersion);
            if (Format2D.HasFlag(FormatFlags1.Flag01) && Format2D.HasFlag(FormatFlags1.IntDataOffset))
            {
                bw.ReserveUInt32("DataStart");
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
            else if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
            {
                bw.ReserveInt64("DataStart");
                bw.WriteInt64(0);
            }
            
            // Write row headers
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                {
                    bw.WriteInt32(Rows[i].ID);
                    bw.WriteInt32(0);
                    bw.ReserveInt64($"RowOffset{i}");
                    bw.ReserveInt64($"NameOffset{i}");
                }
                else
                {
                    bw.WriteInt32(Rows[i].ID);
                    bw.ReserveUInt32($"RowOffset{i}");
                    bw.ReserveUInt32($"NameOffset{i}");
                }
            }
            
            // This is probably pretty stupid
            if (Format2D == FormatFlags1.Flag01)
                bw.WritePattern(0x20, 0x00);

            if (Format2D.HasFlag(FormatFlags1.Flag01) && Format2D.HasFlag(FormatFlags1.IntDataOffset))
                bw.FillUInt32("DataStart", (uint)bw.Position);
            else if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                bw.FillInt64("DataStart", bw.Position);
            else
                bw.FillUInt16("DataStart", (ushort)bw.Position);
            
            // Write row data
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                    bw.FillInt64($"RowOffset{i}", bw.Position);
                else
                    bw.FillUInt32($"RowOffset{i}", (uint)bw.Position);
                
                var data = _paramData.DataForElement(Rows[i].DataIndex);
                bw.WriteBytes(data);
            }
            
            bw.FillUInt32("StringsOffset", (uint)bw.Position);
            
            if (Format2D.HasFlag(FormatFlags1.OffsetParamType))
            {
                bw.FillInt64("ParamTypeOffset", bw.Position);
                bw.WriteASCII(ParamType, true);
            }
            
            // Write row names
            Dictionary<string, long> stringOffsetDictionary = new Dictionary<string, long>();
            
            for (int i = 0; i < Rows.Count; i++) 
            {
                string rowName = Rows[i].Name ?? string.Empty;

                stringOffsetDictionary.TryGetValue(rowName, out long nameOffset);
                if (nameOffset == 0) 
                {
                    nameOffset = bw.Position;
                    if (Format2E.HasFlag(FormatFlags2.UnicodeRowNames))
                        bw.WriteUTF16(rowName, true);
                    else
                        bw.WriteShiftJIS(rowName, true);

                    stringOffsetDictionary.Add(rowName, nameOffset);
                }

                if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                    bw.FillInt64($"NameOffset{i}", nameOffset);
                else
                    bw.FillUInt32($"NameOffset{i}", (uint) nameOffset);
            }
            
            bw.WriteInt16(0); //FS Seems to end their params with an empty string
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

        public Column? this[string name] => Cells.FirstOrDefault(cell => cell.Def.InternalName == name);
    }
}