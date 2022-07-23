using System.Runtime.InteropServices;
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

        public struct Row
        {
            public int ID;
            public string Name;
            public uint DataIndex;
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

        public StridedByteArray? ParamData { get; private set; } = null;
        
        public List<Row> Rows { get; private set; }

        /// <summary>
        /// Get the rows as a span to allow in place modification of the structure. You must call
        /// this to get a new span if you add or delete any members to the underlying Rows list
        /// </summary>
        public Span<Row> RowsAsSpan => CollectionsMarshal.AsSpan(Rows);

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
            var pendingRow = new Row();
            for (var i = 0; i < rowCount; i++)
            {
                long nameOffset;
                if (Format2D.HasFlag(FormatFlags1.LongDataOffset))
                {
                    pendingRow.ID = br.ReadInt32();
                    br.ReadInt32(); // I would like to assert 0, but some of the generatordbglocation params in DS2S have garbage here
                    pendingRow.DataIndex = (uint)br.ReadInt64();
                    nameOffset = br.ReadInt64();
                }
                else
                {
                    pendingRow.ID = br.ReadInt32();
                    pendingRow.DataIndex = br.ReadUInt32();
                    nameOffset = br.ReadUInt32();
                }

                if (nameOffset != 0)
                {
                    if (actualStringsOffset == 0 || nameOffset < actualStringsOffset)
                        actualStringsOffset = nameOffset;

                    if (Format2E.HasFlag(FormatFlags2.UnicodeRowNames))
                        pendingRow.Name = br.GetUTF16(nameOffset);
                    else
                        pendingRow.Name = br.GetShiftJIS(nameOffset);
                }
                Rows.Add(pendingRow);
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
            }
        }

        protected override void Write(BinaryWriterEx bw)
        {
            
        }
    }
}