using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStiffSpringConstraintAtom : hkpConstraintAtom
    {
        public float m_length;
        public float m_maxLength;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt16(0);
            m_length = br.ReadSingle();
            m_maxLength = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_length);
            bw.WriteSingle(m_maxLength);
        }
    }
}
