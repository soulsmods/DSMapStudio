using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateMachineActiveTransitionInfo
    {
        public hkbNodeInternalStateInfo m_transitionEffectInternalStateInfo;
        public hkbStateMachineTransitionInfoReference m_transitionInfoReference;
        public hkbStateMachineTransitionInfoReference m_transitionInfoReferenceForTE;
        public int m_fromStateId;
        public int m_toStateId;
        public bool m_isReturnToPreviousState;
    }
}
