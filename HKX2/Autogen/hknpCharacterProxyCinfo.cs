using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCharacterProxyCinfo : hkReferencedObject
    {
        public Vector4 m_position;
        public Quaternion m_orientation;
        public Vector4 m_velocity;
        public float m_dynamicFriction;
        public float m_staticFriction;
        public float m_keepContactTolerance;
        public Vector4 m_up;
        public hknpShape m_shape;
        public ulong m_userData;
        public uint m_collisionFilterInfo;
        public float m_keepDistance;
        public float m_contactAngleSensitivity;
        public uint m_userPlanes;
        public float m_maxCharacterSpeedForSolver;
        public float m_characterStrength;
        public float m_characterMass;
        public float m_maxSlope;
        public float m_penetrationRecoverySpeed;
        public int m_maxCastIterations;
        public bool m_refreshManifoldInCheckSupport;
        public bool m_presenceInWorld;
    }
}
