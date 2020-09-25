using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStandardLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclStandardLinkConstraintSetMxBatch> m_batches;
        public List<hclStandardLinkConstraintSetMxSingle> m_singles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_batches = des.ReadClassArray<hclStandardLinkConstraintSetMxBatch>(br);
            m_singles = des.ReadClassArray<hclStandardLinkConstraintSetMxSingle>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
