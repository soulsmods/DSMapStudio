using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoseMatchingGeneratorInternalState : hkReferencedObject
    {
        public int m_currentMatch;
        public int m_bestMatch;
        public float m_timeSinceBetterMatch;
        public float m_error;
        public bool m_resetCurrentMatchLocalTime;
    }
}
