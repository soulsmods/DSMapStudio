using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetMxSingle
    {
        public float m_bendMinLength;
        public float m_stretchMaxLength;
        public float m_stretchStiffness;
        public float m_bendStiffness;
        public float m_invMassA;
        public float m_invMassB;
        public ushort m_particleA;
        public ushort m_particleB;
    }
}
