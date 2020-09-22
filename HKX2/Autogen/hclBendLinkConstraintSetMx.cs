using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclBendLinkConstraintSetMxBatch> m_batches;
        public List<hclBendLinkConstraintSetMxSingle> m_singles;
    }
}
