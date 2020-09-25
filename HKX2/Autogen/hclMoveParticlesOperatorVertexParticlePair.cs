using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMoveParticlesOperatorVertexParticlePair : IHavokObject
    {
        public ushort m_vertexIndex;
        public ushort m_particleIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vertexIndex = br.ReadUInt16();
            m_particleIndex = br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_vertexIndex);
            bw.WriteUInt16(m_particleIndex);
        }
    }
}
