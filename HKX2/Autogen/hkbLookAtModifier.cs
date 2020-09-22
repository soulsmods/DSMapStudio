using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLookAtModifier : hkbModifier
    {
        public Vector4 m_targetWS;
        public Vector4 m_headForwardLS;
        public Vector4 m_neckForwardLS;
        public Vector4 m_neckRightLS;
        public Vector4 m_eyePositionHS;
        public float m_newTargetGain;
        public float m_onGain;
        public float m_offGain;
        public float m_limitAngleDegrees;
        public float m_limitAngleLeft;
        public float m_limitAngleRight;
        public float m_limitAngleUp;
        public float m_limitAngleDown;
        public short m_headIndex;
        public short m_neckIndex;
        public bool m_isOn;
        public bool m_individualLimitsOn;
        public bool m_isTargetInsideLimitCone;
    }
}
