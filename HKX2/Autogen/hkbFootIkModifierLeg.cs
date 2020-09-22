using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbFootIkModifierLeg
    {
        public hkQTransform m_originalAnkleTransformMS;
        public Vector4 m_kneeAxisLS;
        public Vector4 m_footEndLS;
        public hkbEventProperty m_ungroundedEvent;
        public float m_footPlantedAnkleHeightMS;
        public float m_footRaisedAnkleHeightMS;
        public float m_maxAnkleHeightMS;
        public float m_minAnkleHeightMS;
        public float m_maxKneeAngleDegrees;
        public float m_minKneeAngleDegrees;
        public float m_verticalError;
        public short m_hipIndex;
        public short m_kneeIndex;
        public short m_ankleIndex;
        public bool m_hitSomething;
        public bool m_isPlantedMS;
        public bool m_isOriginalAnkleTransformMSSet;
    }
}
