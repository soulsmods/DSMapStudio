using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventsFromRangeModifier : hkbModifier
    {
        public float m_inputValue;
        public float m_lowerBound;
        public hkbEventRangeDataArray m_eventRanges;
    }
}
