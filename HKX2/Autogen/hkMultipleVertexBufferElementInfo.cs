using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMultipleVertexBufferElementInfo : IHavokObject
    {
        public byte m_vertexBufferIndex;
        public byte m_elementIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vertexBufferIndex = br.ReadByte();
            m_elementIndex = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_vertexBufferIndex);
            bw.WriteByte(m_elementIndex);
        }
    }
}
