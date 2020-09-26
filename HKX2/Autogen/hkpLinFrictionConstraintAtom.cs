using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinFrictionConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_frictionAxis;
        public float m_maxFrictionForce;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_frictionAxis = br.ReadByte();
            m_maxFrictionForce = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_frictionAxis);
            bw.WriteSingle(m_maxFrictionForce);
            bw.WriteUInt64(0);
        }
    }
}
