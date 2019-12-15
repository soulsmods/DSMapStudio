using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BXF3 containers.
    /// </summary>
    public class BXF3Reader : BinderReader, IBXF3
    {
        /// <summary>
        /// Reads a BXF3 from the given BHD and BDT paths.
        /// </summary>
        public BXF3Reader(string bhdPath, string bdtPath)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                FileStream fsData = File.OpenRead(bdtPath);
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, fsData);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD path and BDT bytes.
        /// </summary>
        public BXF3Reader(string bhdPath, byte[] bdtBytes)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var msData = new MemoryStream(bdtBytes);
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, msData);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD bytes and BDT path.
        /// </summary>
        public BXF3Reader(byte[] bhdBytes, string bdtPath)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                FileStream fsData = File.OpenRead(bdtPath);
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, fsData);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD and BDT bytes.
        /// </summary>
        public BXF3Reader(byte[] bhdBytes, byte[] bdtBytes)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var msData = new MemoryStream(bdtBytes);
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, msData);
                Read(brHeader, brData);
            }
        }

        private void Read(BinaryReaderEx brHeader, BinaryReaderEx brData)
        {
            BXF3.ReadBDFHeader(brData);
            Files = BXF3.ReadBHFHeader(this, brHeader);
            DataBR = brData;
        }
    }
}
