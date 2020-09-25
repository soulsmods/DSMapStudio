using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum RayHitType
    {
        HIT_CAP0 = 0,
        HIT_CAP1 = 1,
        HIT_BODY = 2,
    }
    
    public class hkpCapsuleShape : hkpConvexShape
    {
        public Vector4 m_vertexA;
        public Vector4 m_vertexB;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_vertexA = des.ReadVector4(br);
            m_vertexB = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
