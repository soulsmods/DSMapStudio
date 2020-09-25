using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclUpdateSomeVertexFramesOperatorTriangle : IHavokObject
    {
        public ushort m_indices;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_indices = br.ReadUInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_indices);
            bw.WriteUInt32(0);
        }
    }
}
