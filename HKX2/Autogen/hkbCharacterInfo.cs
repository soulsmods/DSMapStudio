using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Event
    {
        REMOVED_FROM_WORLD = 0,
        SHOWN = 1,
        HIDDEN = 2,
        ACTIVATED = 3,
        DEACTIVATED = 4,
    }
    
    public class hkbCharacterInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public Event m_event;
        public int m_padding;
    }
}
