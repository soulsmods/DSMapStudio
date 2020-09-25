using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpAngFrictionConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_firstFrictionAxis;
        public byte m_numFrictionAxes;
        public float m_maxFrictionTorque;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_firstFrictionAxis = br.ReadByte();
            m_numFrictionAxes = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_maxFrictionTorque = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_firstFrictionAxis);
            bw.WriteByte(m_numFrictionAxes);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_maxFrictionTorque);
            bw.WriteUInt32(0);
        }
    }
}
