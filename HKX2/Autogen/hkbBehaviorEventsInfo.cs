using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorEventsInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public List<short> m_externalEventIds;
        public int m_padding;
    }
}
