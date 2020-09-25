using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDeformableLinConstraintAtom : hkpConstraintAtom
    {
        public Vector4 m_offset;
        public Vector4 m_yieldStrengthDiag;
        public Vector4 m_yieldStrengthOffDiag;
        public Vector4 m_ultimateStrengthDiag;
        public Vector4 m_ultimateStrengthOffDiag;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_offset = des.ReadVector4(br);
            m_yieldStrengthDiag = des.ReadVector4(br);
            m_yieldStrengthOffDiag = des.ReadVector4(br);
            m_ultimateStrengthDiag = des.ReadVector4(br);
            m_ultimateStrengthOffDiag = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
