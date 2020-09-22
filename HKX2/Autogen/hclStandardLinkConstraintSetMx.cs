using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStandardLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclStandardLinkConstraintSetMxBatch> m_batches;
        public List<hclStandardLinkConstraintSetMxSingle> m_singles;
    }
}
