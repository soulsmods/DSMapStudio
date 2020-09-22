using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeRotationFromAxisAngleModifier : hkbModifier
    {
        public Quaternion m_rotationOut;
        public Vector4 m_axis;
        public float m_angleDegrees;
    }
}
