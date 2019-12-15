using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BND4 containers.
    /// </summary>
    public class BND4Reader : BinderReader, IBND4
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk04 { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk05 { get; set; }

        /// <summary>
        /// Whether to encode filenames as UTF-8 or Shift JIS.
        /// </summary>
        public bool Unicode { get; set; }

        /// <summary>
        /// Indicates presence of filename hash table.
        /// </summary>
        public byte Extended { get; set; }

        /// <summary>
        /// Type of compression used, if any.
        /// </summary>
        public DCX.Type Compression { get; set; }

        /// <summary>
        /// Reads a BND4 from the given path, decompressing if necessary.
        /// </summary>
        public BND4Reader(string path)
        {
            FileStream fs = File.OpenRead(path);
            var br = new BinaryReaderEx(false, fs);
            Read(br);
        }

        /// <summary>
        /// Reads a BND4 from the given bytes, decompressing if necessary.
        /// </summary>
        public BND4Reader(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            var br = new BinaryReaderEx(false, ms);
            Read(br);
        }

        private void Read(BinaryReaderEx br)
        {
            br = SFUtil.GetDecompressedBR(br, out DCX.Type compression);
            Compression = compression;
            Files = BND4.ReadHeader(this, br);
            DataBR = br;
        }
    }
}
