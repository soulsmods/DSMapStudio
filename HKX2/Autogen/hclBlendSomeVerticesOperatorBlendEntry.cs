using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBlendSomeVerticesOperatorBlendEntry : IHavokObject
    {
        public ushort m_vertexIndex;
        public float m_blendWeight;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vertexIndex = br.ReadUInt16();
            br.ReadUInt16();
            m_blendWeight = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_vertexIndex);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_blendWeight);
        }
    }
}
