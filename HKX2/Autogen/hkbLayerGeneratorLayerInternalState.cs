using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum FadingState
    {
        FADING_STATE_NONE = 0,
        FADING_STATE_IN = 1,
        FADING_STATE_OUT = 2,
    }
    
    public class hkbLayerGeneratorLayerInternalState
    {
        public float m_weight;
        public float m_timeElapsed;
        public float m_onFraction;
        public FadingState m_fadingState;
        public bool m_useMotion;
        public bool m_syncNextFrame;
        public bool m_isActive;
    }
}
