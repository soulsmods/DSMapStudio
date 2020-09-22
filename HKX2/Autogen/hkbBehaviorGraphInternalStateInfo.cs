using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphInternalStateInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraphInternalState m_internalState;
        public List<hkbAuxiliaryNodeInfo> m_auxiliaryNodeInfo;
        public List<short> m_activeEventIds;
        public List<short> m_activeVariableIds;
    }
}
