using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendStiffnessConstraintSet : hclConstraintSet
    {
        public List<hclBendStiffnessConstraintSetLink> m_links;
        public bool m_useRestPoseConfig;
    }
}
