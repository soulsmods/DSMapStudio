using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransitionConstraintSetPerParticle
    {
        public ushort m_particleIndex;
        public ushort m_referenceVertex;
        public float m_toAnimDelay;
        public float m_toSimDelay;
        public float m_toSimMaxDistance;
    }
}
