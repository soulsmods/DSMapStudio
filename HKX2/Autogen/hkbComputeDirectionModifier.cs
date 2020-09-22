using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeDirectionModifier : hkbModifier
    {
        public Vector4 m_pointIn;
        public Vector4 m_pointOut;
        public float m_groundAngleOut;
        public float m_upAngleOut;
        public float m_verticalOffset;
        public bool m_reverseGroundAngle;
        public bool m_reverseUpAngle;
        public bool m_projectPoint;
        public bool m_normalizePoint;
        public bool m_computeOnlyOnce;
        public bool m_computedOutput;
    }
}
