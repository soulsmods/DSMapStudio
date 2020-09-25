using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLimitedForceConstraintMotor : hkpConstraintMotor
    {
        public float m_minForce;
        public float m_maxForce;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_minForce = br.ReadSingle();
            m_maxForce = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_minForce);
            bw.WriteSingle(m_maxForce);
        }
    }
}
