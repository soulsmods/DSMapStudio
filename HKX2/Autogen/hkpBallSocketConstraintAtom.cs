using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBallSocketConstraintAtom : hkpConstraintAtom
    {
        public SolvingMethod m_solvingMethod;
        public byte m_bodiesToNotify;
        public hkUFloat8 m_velocityStabilizationFactor;
        public bool m_enableLinearImpulseLimit;
        public float m_breachImpulse;
        public float m_inertiaStabilizationFactor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_solvingMethod = (SolvingMethod)br.ReadByte();
            m_bodiesToNotify = br.ReadByte();
            m_velocityStabilizationFactor = new hkUFloat8();
            m_velocityStabilizationFactor.Read(des, br);
            m_enableLinearImpulseLimit = br.ReadBoolean();
            br.ReadUInt16();
            m_breachImpulse = br.ReadSingle();
            m_inertiaStabilizationFactor = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_bodiesToNotify);
            m_velocityStabilizationFactor.Write(bw);
            bw.WriteBoolean(m_enableLinearImpulseLimit);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_breachImpulse);
            bw.WriteSingle(m_inertiaStabilizationFactor);
        }
    }
}
