using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBlenderGeneratorInternalState : hkReferencedObject
    {
        public List<hkbBlenderGeneratorChildInternalState> m_childrenInternalStates;
        public List<short> m_sortedChildren;
        public float m_endIntervalWeight;
        public int m_numActiveChildren;
        public short m_beginIntervalIndex;
        public short m_endIntervalIndex;
        public bool m_initSync;
        public bool m_doSubtractiveBlend;
    }
}
