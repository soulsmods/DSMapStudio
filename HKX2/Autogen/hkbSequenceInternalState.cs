using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSequenceInternalState : hkReferencedObject
    {
        public List<int> m_nextSampleEvents;
        public List<int> m_nextSampleReals;
        public List<int> m_nextSampleBools;
        public List<int> m_nextSampleInts;
        public float m_time;
        public bool m_isEnabled;
    }
}
