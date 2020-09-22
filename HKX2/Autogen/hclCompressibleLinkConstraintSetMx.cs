using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclCompressibleLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclCompressibleLinkConstraintSetMxBatch> m_batches;
        public List<hclCompressibleLinkConstraintSetMxSingle> m_singles;
    }
}
