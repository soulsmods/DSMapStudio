using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorSyncInfo
    {
        public hkbGeneratorSyncInfoSyncPoint m_syncPoints;
        public float m_duration;
        public float m_localTime;
        public float m_playbackSpeed;
        public char m_numSyncPoints;
        public bool m_isCyclic;
        public bool m_isMirrored;
        public bool m_isAdditive;
        public hkbGeneratorSyncInfoActiveInterval m_activeInterval;
        public float m_nativePlaybackSpeed;
    }
}
