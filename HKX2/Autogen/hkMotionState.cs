using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMotionState
    {
        public Matrix4x4 m_transform;
        public Vector4 m_sweptTransform;
        public Vector4 m_deltaAngle;
        public float m_objectRadius;
        public ushort m_linearDamping;
        public ushort m_angularDamping;
        public ushort m_timeFactor;
        public hkUFloat8 m_maxLinearVelocity;
        public hkUFloat8 m_maxAngularVelocity;
        public byte m_deactivationClass;
    }
}
