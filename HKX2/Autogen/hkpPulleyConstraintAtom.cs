using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPulleyConstraintAtom : hkpConstraintAtom
    {
        public Vector4 m_fixedPivotAinWorld;
        public Vector4 m_fixedPivotBinWorld;
        public float m_ropeLength;
        public float m_leverageOnBodyB;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            m_fixedPivotAinWorld = des.ReadVector4(br);
            m_fixedPivotBinWorld = des.ReadVector4(br);
            m_ropeLength = br.ReadSingle();
            m_leverageOnBodyB = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_ropeLength);
            bw.WriteSingle(m_leverageOnBodyB);
            bw.WriteUInt64(0);
        }
    }
}
