using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum StartStateMode
    {
        START_STATE_MODE_DEFAULT = 0,
        START_STATE_MODE_SYNC = 1,
        START_STATE_MODE_RANDOM = 2,
        START_STATE_MODE_CHOOSER = 3,
    }
    
    public enum StateMachineSelfTransitionMode
    {
        SELF_TRANSITION_MODE_NO_TRANSITION = 0,
        SELF_TRANSITION_MODE_TRANSITION_TO_START_STATE = 1,
        SELF_TRANSITION_MODE_FORCE_TRANSITION_TO_START_STATE = 2,
    }
    
    public class hkbStateMachine : hkbGenerator
    {
        public hkbEvent m_eventToSendWhenStateOrTransitionChanges;
        public hkbCustomIdSelector m_startStateIdSelector;
        public int m_startStateId;
        public int m_returnToPreviousStateEventId;
        public int m_randomTransitionEventId;
        public int m_transitionToNextHigherStateEventId;
        public int m_transitionToNextLowerStateEventId;
        public int m_syncVariableIndex;
        public bool m_wrapAroundStateId;
        public char m_maxSimultaneousTransitions;
        public StartStateMode m_startStateMode;
        public StateMachineSelfTransitionMode m_selfTransitionMode;
        public List<hkbStateMachineStateInfo> m_states;
        public hkbStateMachineTransitionInfoArray m_wildcardTransitions;
    }
}
