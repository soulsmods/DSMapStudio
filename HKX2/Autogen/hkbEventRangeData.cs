using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum EventRangeMode
    {
        EVENT_MODE_SEND_ON_ENTER_RANGE = 0,
        EVENT_MODE_SEND_WHEN_IN_RANGE = 1,
    }
    
    public class hkbEventRangeData
    {
        public float m_upperBound;
        public hkbEventProperty m_event;
        public EventRangeMode m_eventMode;
    }
}
