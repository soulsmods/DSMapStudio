using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbFootIkGains
    {
        public float m_onOffGain;
        public float m_groundAscendingGain;
        public float m_groundDescendingGain;
        public float m_footPlantedGain;
        public float m_footRaisedGain;
        public float m_footLockingGain;
        public float m_worldFromModelFeedbackGain;
        public float m_errorUpDownBias;
        public float m_alignWorldFromModelGain;
        public float m_hipOrientationGain;
        public float m_maxKneeAngleDifference;
        public float m_ankleOrientationGain;
    }
}
