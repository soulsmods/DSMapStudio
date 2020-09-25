using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBufferLayoutBufferElement : IHavokObject
    {
        public VectorConversion m_vectorConversion;
        public byte m_vectorSize;
        public byte m_slotId;
        public byte m_slotStart;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vectorConversion = (VectorConversion)br.ReadByte();
            m_vectorSize = br.ReadByte();
            m_slotId = br.ReadByte();
            m_slotStart = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_vectorSize);
            bw.WriteByte(m_slotId);
            bw.WriteByte(m_slotStart);
        }
    }
}
