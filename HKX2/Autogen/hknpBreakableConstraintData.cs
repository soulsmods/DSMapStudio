using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBreakableConstraintData : hkpWrappedConstraintData
    {
        public float m_threshold;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_threshold = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_threshold);
            bw.WriteUInt32(0);
        }
    }
}
