using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPoweredChainDataConstraintInfo : IHavokObject
    {
        public Vector4 m_pivotInA;
        public Vector4 m_pivotInB;
        public Quaternion m_aTc;
        public Quaternion m_bTc;
        public hkpConstraintMotor m_motors;
        public bool m_switchBodies;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivotInA = des.ReadVector4(br);
            m_pivotInB = des.ReadVector4(br);
            m_aTc = des.ReadQuaternion(br);
            m_bTc = des.ReadQuaternion(br);
            m_motors = des.ReadClassPointer<hkpConstraintMotor>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_switchBodies = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteBoolean(m_switchBodies);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
