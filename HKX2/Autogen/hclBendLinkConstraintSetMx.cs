using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclBendLinkConstraintSetMxBatch> m_batches;
        public List<hclBendLinkConstraintSetMxSingle> m_singles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_batches = des.ReadClassArray<hclBendLinkConstraintSetMxBatch>(br);
            m_singles = des.ReadClassArray<hclBendLinkConstraintSetMxSingle>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
