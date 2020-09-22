using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ToGeneratorState
    {
        STATE_INACTIVE = 0,
        STATE_READY_FOR_SET_LOCAL_TIME = 1,
        STATE_READY_FOR_APPLY_SELF_TRANSITION_MODE = 2,
        STATE_ACTIVE = 3,
    }
    
    public enum Stage
    {
        STAGE_BLENDING_IN = 0,
        STAGE_PLAYING_TRANSITION_GENERATOR = 1,
        STAGE_BLENDING_OUT = 2,
    }
    
    public enum ChildState
    {
        CHILD_FROM_GENERATOR = 0,
        CHILD_TRANSITION_GENERATOR = 1,
        CHILD_TO_GENERATOR = 2,
        CHILD_NONE = 3,
    }
    
    public class hkbGeneratorTransitionEffect : hkbTransitionEffect
    {
        public hkbGenerator m_transitionGenerator;
        public float m_blendInDuration;
        public float m_blendOutDuration;
        public bool m_syncToGeneratorStartTime;
    }
}
