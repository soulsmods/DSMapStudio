using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbManualSelectorGeneratorInternalState : hkReferencedObject
    {
        public short m_currentGeneratorIndex;
        public short m_generatorIndexAtActivate;
        public List<hkbStateMachineActiveTransitionInfo> m_activeTransitions;
    }
}
