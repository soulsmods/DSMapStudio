using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStiffSpringChainDataConstraintInfo : IHavokObject
    {
        public Vector4 m_pivotInA;
        public Vector4 m_pivotInB;
        public float m_springLength;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivotInA = des.ReadVector4(br);
            m_pivotInB = des.ReadVector4(br);
            m_springLength = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_springLength);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
