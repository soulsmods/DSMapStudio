using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMotionCinfo
    {
        public ushort m_motionPropertiesId;
        public bool m_enableDeactivation;
        public float m_inverseMass;
        public float m_massFactor;
        public float m_maxLinearAccelerationDistancePerStep;
        public float m_maxRotationToPreventTunneling;
        public Vector4 m_inverseInertiaLocal;
        public Vector4 m_centerOfMassWorld;
        public Quaternion m_orientation;
        public Vector4 m_linearVelocity;
        public Vector4 m_angularVelocity;
    }
}
