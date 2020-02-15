using System;
using System.Collections.Generic;
using System.Text;

namespace SoulsFormats
{
    /// <summary>
    /// New HKX class. Unlike the old one, this represents havok objects as a linear array
    /// of bytes which are then viewed as structs. This is closer to how C++ packfiles are
    /// represented and also simplifies the serialization and deserialization process. The
    /// tricky parts is handling 32-bit and big endian packfiles on a 64-bit machine
    /// </summary>
    public partial class HKX2
    {
        public class HKXClassName
        {
            public uint Signature;
            public string ClassName;
            public uint SectionOffset;

            internal HKXClassName(BinaryReaderEx br)
            {
                Signature = br.ReadUInt32();
                br.AssertByte(0x09); // Seems random but ok
                SectionOffset = (uint)br.Position;
                ClassName = br.ReadASCII();
            }

            public void Write(BinaryWriterEx bw, uint sectionBaseOffset)
            {
                bw.WriteUInt32(Signature);
                bw.WriteByte(0x09);
                bw.WriteASCII(ClassName, true);
            }
        }

        /// <summary>
        /// Allocations for Havok objects are stored on unmanaged heaps that are managed in
        /// the context of this container. This allows for C++ havok style serialization
        /// and deserialization of the classes
        /// </summary>
        private unsafe List<IntPtr> _allocations;

    }
}
