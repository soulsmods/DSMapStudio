using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BlendHint
    {
        NORMAL = 0,
        ADDITIVE_DEPRECATED = 1,
        ADDITIVE = 2,
    }
    
    public class hkaAnimationBinding : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public hkaAnimation m_animation;
        public List<short> m_transformTrackToBoneIndices;
        public List<short> m_floatTrackToFloatSlotIndices;
        public List<short> m_partitionIndices;
        public BlendHint m_blendHint;
    }
}
