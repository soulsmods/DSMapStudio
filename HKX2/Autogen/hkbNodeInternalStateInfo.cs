using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbNodeInternalStateInfo : hkReferencedObject
    {
        public hkbReferencedGeneratorSyncInfo m_syncInfo;
        public string m_name;
        public hkReferencedObject m_internalState;
        public ushort m_nodeId;
        public bool m_hasActivateBeenCalled;
        public bool m_isModifierEnabled;
    }
}
