using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetBehaviorCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraph m_behavior;
        public hkbGenerator m_rootGenerator;
        public List<hkbBehaviorGraph> m_referencedBehaviors;
        public int m_startStateIndex;
        public bool m_randomizeSimulation;
        public int m_padding;
    }
}
