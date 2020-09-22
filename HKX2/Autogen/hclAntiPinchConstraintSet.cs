using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclAntiPinchConstraintSet : hclConstraintSet
    {
        public List<hclAntiPinchConstraintSetPerParticle> m_perParticleData;
        public float m_toAnimPeriod;
        public float m_toSimPeriod;
        public float m_toSimMaxDistance;
        public uint m_referenceMeshBufferIdx;
    }
}
