using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBufferLayoutSlot : IHavokObject
    {
        public SlotFlags m_flags;
        public byte m_stride;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_flags = (SlotFlags)br.ReadByte();
            m_stride = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_stride);
        }
    }
}
