using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventDrivenModifier : hkbModifierWrapper
    {
        public int m_activateEventId;
        public int m_deactivateEventId;
        public bool m_activeByDefault;
    }
}
