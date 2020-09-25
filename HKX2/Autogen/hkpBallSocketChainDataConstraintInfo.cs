using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBallSocketChainDataConstraintInfo : IHavokObject
    {
        public Vector4 m_pivotInA;
        public Vector4 m_pivotInB;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivotInA = des.ReadVector4(br);
            m_pivotInB = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
