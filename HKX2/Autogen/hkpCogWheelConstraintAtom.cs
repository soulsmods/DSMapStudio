using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCogWheelConstraintAtom : hkpConstraintAtom
    {
        public float m_cogWheelRadiusA;
        public float m_cogWheelRadiusB;
        public bool m_isScrew;
        public sbyte m_memOffsetToInitialAngleOffset;
        public sbyte m_memOffsetToPrevAngle;
        public sbyte m_memOffsetToRevolutionCounter;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt16();
            m_cogWheelRadiusA = br.ReadSingle();
            m_cogWheelRadiusB = br.ReadSingle();
            m_isScrew = br.ReadBoolean();
            m_memOffsetToInitialAngleOffset = br.ReadSByte();
            m_memOffsetToPrevAngle = br.ReadSByte();
            m_memOffsetToRevolutionCounter = br.ReadSByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_cogWheelRadiusA);
            bw.WriteSingle(m_cogWheelRadiusB);
            bw.WriteBoolean(m_isScrew);
            bw.WriteSByte(m_memOffsetToInitialAngleOffset);
            bw.WriteSByte(m_memOffsetToPrevAngle);
            bw.WriteSByte(m_memOffsetToRevolutionCounter);
        }
    }
}
