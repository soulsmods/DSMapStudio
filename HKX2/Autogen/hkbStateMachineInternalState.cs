using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateMachineInternalState : hkReferencedObject
    {
        public List<hkbStateMachineActiveTransitionInfo> m_activeTransitions;
        public List<byte> m_transitionFlags;
        public List<byte> m_wildcardTransitionFlags;
        public List<hkbStateMachineDelayedTransitionInfo> m_delayedTransitions;
        public float m_timeInState;
        public float m_lastLocalTime;
        public int m_currentStateId;
        public int m_previousStateId;
        public int m_nextStartStateIndexOverride;
        public bool m_stateOrTransitionChanged;
        public bool m_echoNextUpdate;
        public bool m_hasEventlessWildcardTransitions;
    }
}
