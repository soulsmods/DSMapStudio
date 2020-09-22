using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum FlagBits
    {
        FLAG_NONE = 0,
        FLAG_IGNORE_FROM_WORLD_FROM_MODEL = 1,
        FLAG_SYNC = 2,
        FLAG_IGNORE_TO_WORLD_FROM_MODEL = 4,
        FLAG_IGNORE_TO_WORLD_FROM_MODEL_ROTATION = 8,
    }
    
    public enum EndMode
    {
        END_MODE_NONE = 0,
        END_MODE_TRANSITION_UNTIL_END_OF_FROM_GENERATOR = 1,
        END_MODE_CAP_DURATION_AT_END_OF_FROM_GENERATOR = 2,
    }
    
    public class hkbBlendingTransitionEffect : hkbTransitionEffect
    {
        public float m_duration;
        public float m_toGeneratorStartTimeFraction;
        public uint m_flags;
        public EndMode m_endMode;
        public BlendCurve m_blendCurve;
        public short m_alignmentBone;
    }
}
