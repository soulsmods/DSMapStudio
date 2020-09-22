using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAuxiliaryNodeInfo : hkReferencedObject
    {
        public NodeType m_type;
        public byte m_depth;
        public string m_referenceBehaviorName;
        public List<string> m_selfTransitionNames;
    }
}
