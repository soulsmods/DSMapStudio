using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetLink
    {
        public ushort m_particleA;
        public ushort m_particleB;
        public float m_bendMinLength;
        public float m_stretchMaxLength;
        public float m_bendStiffness;
        public float m_stretchStiffness;
    }
}
