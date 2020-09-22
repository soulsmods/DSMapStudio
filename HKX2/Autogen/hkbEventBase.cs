using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SystemEventIds
    {
        EVENT_ID_NULL = -1,
    }
    
    public class hkbEventBase
    {
        public int m_id;
        public hkbEventPayload m_payload;
    }
}
