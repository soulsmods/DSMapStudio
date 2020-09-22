using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphInternalState : hkReferencedObject
    {
        public List<hkbNodeInternalStateInfo> m_nodeInternalStateInfos;
        public hkbVariableValueSet m_variableValueSet;
    }
}
