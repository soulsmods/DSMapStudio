using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpAngMotorConstraintAtom : hkpConstraintAtom
    {
        public bool m_isEnabled;
        public byte m_motorAxis;
        public float m_targetAngle;
        public hkpConstraintMotor m_motor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadBoolean();
            m_motorAxis = br.ReadByte();
            br.AssertUInt64(0);
            m_targetAngle = br.ReadSingle();
            m_motor = des.ReadClassPointer<hkpConstraintMotor>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteByte(m_motorAxis);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_targetAngle);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
