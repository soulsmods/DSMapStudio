using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum TransitionFlags
    {
        FLAG_USE_TRIGGER_INTERVAL = 1,
        FLAG_USE_INITIATE_INTERVAL = 2,
        FLAG_UNINTERRUPTIBLE_WHILE_PLAYING = 4,
        FLAG_UNINTERRUPTIBLE_WHILE_DELAYED = 8,
        FLAG_DELAY_STATE_CHANGE = 16,
        FLAG_DISABLED = 32,
        FLAG_DISALLOW_RETURN_TO_PREVIOUS_STATE = 64,
        FLAG_DISALLOW_RANDOM_TRANSITION = 128,
        FLAG_DISABLE_CONDITION = 256,
        FLAG_ALLOW_SELF_TRANSITION_BY_TRANSITION_FROM_ANY_STATE = 512,
        FLAG_IS_GLOBAL_WILDCARD = 1024,
        FLAG_IS_LOCAL_WILDCARD = 2048,
        FLAG_FROM_NESTED_STATE_ID_IS_VALID = 4096,
        FLAG_TO_NESTED_STATE_ID_IS_VALID = 8192,
        FLAG_ABUT_AT_END_OF_FROM_GENERATOR = 16384,
    }
    
    public enum InternalFlagBits
    {
        FLAG_INTERNAL_IN_TRIGGER_INTERVAL = 1,
        FLAG_INTERNAL_IN_INITIATE_INTERVAL = 2,
    }
    
    public class hkbStateMachineTransitionInfo
    {
        public hkbStateMachineTimeInterval m_triggerInterval;
        public hkbStateMachineTimeInterval m_initiateInterval;
        public hkbTransitionEffect m_transition;
        public hkbCondition m_condition;
        public int m_eventId;
        public int m_toStateId;
        public int m_fromNestedStateId;
        public int m_toNestedStateId;
        public short m_priority;
        public uint m_flags;
    }
}
