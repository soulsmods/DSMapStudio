using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BND3, BND4, BXF3, or BXF4 containers.
    /// </summary>
    public abstract class BinderReader : IDisposable
    {
        /// <summary>
        /// A timestamp or version number, 8 characters maximum.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Indicates the format of the binder.
        /// </summary>
        public Binder.Format Format { get; set; }

        /// <summary>
        /// Write bytes in big-endian order for PS3/X360.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// Determines ordering of flag bits.
        /// </summary>
        public bool BitBigEndian { get; set; }

        /// <summary>
        /// Metadata for files present in the binder.
        /// </summary>
        public List<BinderFileHeader> Files { get; set; }

        /// <summary>
        /// Reader to read file data from.
        /// </summary>
        protected BinaryReaderEx DataBR;

        /// <summary>
        /// Reads file data according to the header at the given index in Files.
        /// </summary>
        public byte[] ReadFile(int index)
        {
            return ReadFile(Files[index]);
        }

        /// <summary>
        /// Reads file data according to the given header.
        /// </summary>
        public byte[] ReadFile(BinderFileHeader fileHeader)
        {
            BinderFile file = fileHeader.ReadFileData(DataBR);
            return file.Bytes;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases the unmanaged resources used by the BinderReader and optionally releases the managed resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DataBR?.Stream?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the BinderReader.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
