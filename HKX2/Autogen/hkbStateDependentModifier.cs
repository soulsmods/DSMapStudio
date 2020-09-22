using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateDependentModifier : hkbModifier
    {
        public bool m_applyModifierDuringTransition;
        public List<int> m_stateIds;
        public hkbModifier m_modifier;
        public bool m_isActive;
        public hkbStateMachine m_stateMachine;
    }
}
