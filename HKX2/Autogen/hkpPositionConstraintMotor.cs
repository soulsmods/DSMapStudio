using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPositionConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public float m_tau;
        public float m_damping;
        public float m_proportionalRecoveryVelocity;
        public float m_constantRecoveryVelocity;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_proportionalRecoveryVelocity = br.ReadSingle();
            m_constantRecoveryVelocity = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_proportionalRecoveryVelocity);
            bw.WriteSingle(m_constantRecoveryVelocity);
        }
    }
}
