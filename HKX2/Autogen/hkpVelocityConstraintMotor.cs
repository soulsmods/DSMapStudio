using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVelocityConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public float m_tau;
        public float m_velocityTarget;
        public bool m_useVelocityTargetFromConstraintTargets;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_velocityTarget = br.ReadSingle();
            m_useVelocityTargetFromConstraintTargets = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_velocityTarget);
            bw.WriteBoolean(m_useVelocityTargetFromConstraintTargets);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
