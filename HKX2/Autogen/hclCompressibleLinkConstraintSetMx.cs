using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclCompressibleLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclCompressibleLinkConstraintSetMxBatch> m_batches;
        public List<hclCompressibleLinkConstraintSetMxSingle> m_singles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_batches = des.ReadClassArray<hclCompressibleLinkConstraintSetMxBatch>(br);
            m_singles = des.ReadClassArray<hclCompressibleLinkConstraintSetMxSingle>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
