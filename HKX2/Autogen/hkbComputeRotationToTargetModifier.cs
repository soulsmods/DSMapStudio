using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeRotationToTargetModifier : hkbModifier
    {
        public Quaternion m_rotationOut;
        public Vector4 m_targetPosition;
        public Vector4 m_currentPosition;
        public Quaternion m_currentRotation;
        public Vector4 m_localAxisOfRotation;
        public Vector4 m_localFacingDirection;
        public bool m_resultIsDelta;
    }
}
