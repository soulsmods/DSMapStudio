using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStretchLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclStretchLinkConstraintSetMxBatch> m_batches;
        public List<hclStretchLinkConstraintSetMxSingle> m_singles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_batches = des.ReadClassArray<hclStretchLinkConstraintSetMxBatch>(br);
            m_singles = des.ReadClassArray<hclStretchLinkConstraintSetMxSingle>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
