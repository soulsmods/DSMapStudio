using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkDriverInfoHand
    {
        public Vector4 m_elbowAxisLS;
        public Vector4 m_backHandNormalLS;
        public Vector4 m_handOffsetLS;
        public Quaternion m_handOrienationOffsetLS;
        public float m_maxElbowAngleDegrees;
        public float m_minElbowAngleDegrees;
        public short m_shoulderIndex;
        public short m_shoulderSiblingIndex;
        public short m_elbowIndex;
        public short m_elbowSiblingIndex;
        public short m_wristIndex;
        public bool m_enforceEndPosition;
        public bool m_enforceEndRotation;
        public string m_localFrameName;
    }
}
