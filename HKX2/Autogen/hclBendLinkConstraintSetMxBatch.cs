using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetMxBatch
    {
        public float m_bendMinLengths;
        public float m_stretchMaxLengths;
        public float m_stretchStiffnesses;
        public float m_bendStiffnesses;
        public float m_invMassesA;
        public float m_invMassesB;
        public ushort m_particlesA;
        public ushort m_particlesB;
    }
}
