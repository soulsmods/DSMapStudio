using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbClipGeneratorInternalState : hkReferencedObject
    {
        public hkQTransform m_extractedMotion;
        public List<hkbClipGeneratorEcho> m_echos;
        public float m_localTime;
        public float m_time;
        public float m_previousUserControlledTimeFraction;
        public int m_bufferSize;
        public bool m_atEnd;
        public bool m_ignoreStartTime;
        public bool m_pingPongBackward;
    }
}
