using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendStiffnessConstraintSetMx : hclConstraintSet
    {
        public List<hclBendStiffnessConstraintSetMxBatch> m_batches;
        public List<hclBendStiffnessConstraintSetMxSingle> m_singles;
        public bool m_useRestPoseConfig;
    }
}
