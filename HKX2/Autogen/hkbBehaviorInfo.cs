using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraphData m_data;
        public List<hkbBehaviorInfoIdToNamePair> m_idToNamePairs;
    }
}
