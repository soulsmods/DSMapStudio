using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSoftContactModifierConstraintAtom : hkpModifierConstraintAtom
    {
        public float m_tau;
        public float m_maxAcceleration;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tau = br.ReadSingle();
            m_maxAcceleration = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_maxAcceleration);
            bw.WriteUInt64(0);
        }
    }
}
