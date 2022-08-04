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
                // TODO: clone data
            }

            ~Row()
            {
                Parent.ParamData.RemoveAt(DataIndex);
            }

            public CellHandle? this[string field] => throw new NotImplementedException();
            public CellHandle this[Cell field] => throw new NotImplementedException();
        }

        /// <summary>
        /// Minimal handle of a cell in a row that contains enough to mutate the value of the cell and created
        /// on demand
        /// </summary>
        public struct CellHandle
        {
            public object Value { get; set; }
            public PARAMDEF.Field Def { get; }
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
            
            internal Cell(PARAMDEF.Field def, uint byteOffset)
            {
                Def = def;
                _byteOffset = byteOffset;
            }

            public object GetValue(Row row)
            {
                return null;
            }

            public void SetValue(Row row, object value)
            {
                
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