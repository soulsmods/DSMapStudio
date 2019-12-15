using static SoulsFormats.Binder;

namespace SoulsFormats
{
    /// <summary>
    /// Metadata for a file in a binder container.
    /// </summary>
    public class BinderFileHeader
    {
        /// <summary>
        /// Flags indicating compression, and possibly other things.
        /// </summary>
        public FileFlags Flags { get; set; }

        /// <summary>
        /// ID of the file, or -1 for none.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name of the file, or null for none.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If compressed, which type of compression to use.
        /// </summary>
        public DCX.Type CompressionType { get; set; }

        internal long CompressedSize;
        internal long UncompressedSize;
        internal long DataOffset;

        /// <summary>
        /// Creates a BinderFileHeader with the given ID and name.
        /// </summary>
        public BinderFileHeader(int id, string name) : this(FileFlags.Flag1, id, name) { }

        /// <summary>
        /// Creates a BinderFileHeader with the given flags, ID, and name.
        /// </summary>
        public BinderFileHeader(FileFlags flags, int id, string name) : this(flags, id, name, -1, -1, -1) { }

        internal BinderFileHeader(BinderFile file) : this(file.Flags, file.ID, file.Name, -1, -1, -1)
        {
            CompressionType = file.CompressionType;
        }

        private BinderFileHeader(FileFlags flags, int id, string name, long compressedSize, long uncompressedSize, long dataOffset)
        {
            Flags = flags;
            ID = id;
            Name = name;
            CompressionType = DCX.Type.Zlib;
            CompressedSize = compressedSize;
            UncompressedSize = uncompressedSize;
            DataOffset = dataOffset;
        }

        internal static BinderFileHeader ReadBinder3FileHeader(BinaryReaderEx br, Format format, bool bitBigEndian)
        {
            FileFlags flags = ReadFileFlags(br, bitBigEndian, format);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);

            int compressedSize = br.ReadInt32();

            long dataOffset;
            if (HasLongOffsets(format))
                dataOffset = br.ReadInt64();
            else
                dataOffset = br.ReadUInt32();

            int id = -1;
            if (HasIDs(format))
                id = br.ReadInt32();

            string name = null;
            if (HasNames(format))
            {
                int nameOffset = br.ReadInt32();
                name = br.GetShiftJIS(nameOffset);
            }

            int uncompressedSize = -1;
            if (HasCompression(format))
                uncompressedSize = br.ReadInt32();

            return new BinderFileHeader(flags, id, name, compressedSize, uncompressedSize, dataOffset);
        }

        internal static BinderFileHeader ReadBinder4FileHeader(BinaryReaderEx br, Format format, bool bitBigEndian, bool unicode)
        {
            FileFlags flags = ReadFileFlags(br, bitBigEndian, format);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(-1);

            long compressedSize = br.ReadInt64();

            long uncompressedSize = -1;
            if (HasCompression(format))
                uncompressedSize = br.ReadInt64();

            long dataOffset;
            if (HasLongOffsets(format))
                dataOffset = br.ReadInt64();
            else
                dataOffset = br.ReadUInt32();

            int id = -1;
            if (HasIDs(format))
                id = br.ReadInt32();

            string name = null;
            if (HasNames(format))
            {
                uint nameOffset = br.ReadUInt32();
                if (unicode)
                    name = br.GetUTF16(nameOffset);
                else
                    name = br.GetShiftJIS(nameOffset);
            }

            return new BinderFileHeader(flags, id, name, compressedSize, uncompressedSize, dataOffset);
        }

        internal BinderFile ReadFileData(BinaryReaderEx br)
        {
            byte[] bytes;
            DCX.Type compressionType = DCX.Type.Zlib;
            if (IsCompressed(Flags))
            {
                bytes = br.GetBytes(DataOffset, (int)CompressedSize);
                bytes = DCX.Decompress(bytes, out compressionType);
            }
            else
            {
                bytes = br.GetBytes(DataOffset, (int)CompressedSize);
            }

            return new BinderFile(Flags, ID, Name, bytes)
            {
                CompressionType = compressionType,
            };
        }

        internal void WriteBinder3FileHeader(BinaryWriterEx bw, Format format, bool bitBigEndian, int index)
        {
            WriteFileFlags(bw, bitBigEndian, format, Flags);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.ReserveInt32($"FileCompressedSize{index}");

            if (HasLongOffsets(format))
                bw.ReserveInt64($"FileDataOffset{index}");
            else
                bw.ReserveUInt32($"FileDataOffset{index}");

            if (HasIDs(format))
                bw.WriteInt32(ID);

            if (HasNames(format))
                bw.ReserveInt32($"FileNameOffset{index}");

            if (HasCompression(format))
                bw.ReserveInt32($"FileUncompressedSize{index}");
        }

        internal void WriteBinder4FileHeader(BinaryWriterEx bw, Format format, bool bitBigEndian, int index)
        {
            WriteFileFlags(bw, bitBigEndian, format, Flags);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteInt32(-1);

            bw.ReserveInt64($"FileCompressedSize{index}");

            if (HasCompression(format))
                bw.ReserveInt64($"FileUncompressedSize{index}");

            if (HasLongOffsets(format))
                bw.ReserveInt64($"FileDataOffset{index}");
            else
                bw.ReserveUInt32($"FileDataOffset{index}");

            if (HasIDs(format))
                bw.WriteInt32(ID);

            if (HasNames(format))
                bw.ReserveInt32($"FileNameOffset{index}");
        }

        private void WriteFileData(BinaryWriterEx bw, byte[] bytes)
        {
            if (bytes.LongLength > 0)
                bw.Pad(0x10);

            DataOffset = bw.Position;
            UncompressedSize = bytes.LongLength;
            if (IsCompressed(Flags))
            {
                byte[] compressed = DCX.Compress(bytes, CompressionType);
                CompressedSize = compressed.LongLength;
                bw.WriteBytes(compressed);
            }
            else
            {
                CompressedSize = bytes.LongLength;
                bw.WriteBytes(bytes);
            }
        }

        internal void WriteBinder3FileData(BinaryWriterEx bwHeader, BinaryWriterEx bwData, Format format, int index, byte[] bytes)
        {
            WriteFileData(bwData, bytes);

            bwHeader.FillInt32($"FileCompressedSize{index}", (int)CompressedSize);

            if (HasCompression(format))
                bwHeader.FillInt32($"FileUncompressedSize{index}", (int)UncompressedSize);

            if (HasLongOffsets(format))
                bwHeader.FillInt64($"FileDataOffset{index}", DataOffset);
            else
                bwHeader.FillUInt32($"FileDataOffset{index}", (uint)DataOffset);
        }

        internal void WriteBinder4FileData(BinaryWriterEx bwHeader, BinaryWriterEx bwData, Format format, int index, byte[] bytes)
        {
            WriteFileData(bwData, bytes);

            bwHeader.FillInt64($"FileCompressedSize{index}", CompressedSize);

            if (HasCompression(format))
                bwHeader.FillInt64($"FileUncompressedSize{index}", UncompressedSize);

            if (HasLongOffsets(format))
                bwHeader.FillInt64($"FileDataOffset{index}", DataOffset);
            else
                bwHeader.FillUInt32($"FileDataOffset{index}", (uint)DataOffset);
        }

        internal void WriteFileName(BinaryWriterEx bw, Format format, bool unicode, int index)
        {
            if (HasNames(format))
            {
                bw.FillInt32($"FileNameOffset{index}", (int)bw.Position);
                if (unicode)
                    bw.WriteUTF16(Name, true);
                else
                    bw.WriteShiftJIS(Name, true);
            }
        }
    }
}
