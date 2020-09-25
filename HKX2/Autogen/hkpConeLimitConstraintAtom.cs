using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MeasurementMode
    {
        ZERO_WHEN_VECTORS_ALIGNED = 0,
        ZERO_WHEN_VECTORS_PERPENDICULAR = 1,
    }
    
    public class hkpConeLimitConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_twistAxisInA;
        public byte m_refAxisInB;
        public MeasurementMode m_angleMeasurementMode;
        public byte m_memOffsetToAngleOffset;
        public float m_minAngle;
        public float m_maxAngle;
        public float m_angularLimitsTauFactor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_twistAxisInA = br.ReadByte();
            m_refAxisInB = br.ReadByte();
            m_angleMeasurementMode = (MeasurementMode)br.ReadByte();
            m_memOffsetToAngleOffset = br.ReadByte();
            br.AssertByte(0);
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
            m_angularLimitsTauFactor = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_twistAxisInA);
            bw.WriteByte(m_refAxisInB);
            bw.WriteByte(m_memOffsetToAngleOffset);
            bw.WriteByte(0);
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
            bw.WriteSingle(m_angularLimitsTauFactor);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
