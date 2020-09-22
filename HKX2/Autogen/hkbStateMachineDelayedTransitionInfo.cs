using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateMachineDelayedTransitionInfo
    {
        public hkbStateMachineProspectiveTransitionInfo m_delayedTransition;
        public float m_timeDelayed;
        public bool m_isDelayedTransitionReturnToPreviousState;
        public bool m_wasInAbutRangeLastFrame;
    }
}
