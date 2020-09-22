using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransitionConstraintSet : hclConstraintSet
    {
        public List<hclTransitionConstraintSetPerParticle> m_perParticleData;
        public float m_toAnimPeriod;
        public float m_toAnimPlusDelayPeriod;
        public float m_toSimPeriod;
        public float m_toSimPlusDelayPeriod;
        public uint m_referenceMeshBufferIdx;
    }
}
