using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStretchLinkConstraintSetMx : hclConstraintSet
    {
        public List<hclStretchLinkConstraintSetMxBatch> m_batches;
        public List<hclStretchLinkConstraintSetMxSingle> m_singles;
    }
}
