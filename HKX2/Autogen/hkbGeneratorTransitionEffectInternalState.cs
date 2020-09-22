using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorTransitionEffectInternalState : hkReferencedObject
    {
        public float m_timeInTransition;
        public float m_duration;
        public float m_effectiveBlendInDuration;
        public float m_effectiveBlendOutDuration;
        public ToGeneratorState m_toGeneratorState;
        public bool m_echoTransitionGenerator;
        public SelfTransitionMode m_toGeneratorSelfTransitionMode;
        public bool m_justActivated;
        public bool m_updateActiveNodes;
        public Stage m_stage;
    }
}
