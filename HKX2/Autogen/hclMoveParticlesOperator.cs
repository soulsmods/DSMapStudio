using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ForceUpgrade610
    {
        HCL_FORCE_UPGRADE610 = 0,
    }
    
    public class hclMoveParticlesOperator : hclOperator
    {
        public List<hclMoveParticlesOperatorVertexParticlePair> m_vertexParticlePairs;
        public uint m_simClothIndex;
        public uint m_refBufferIdx;
    }
}
