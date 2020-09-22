using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum FlagEnum
    {
        CONTACT_IS_NEW = 1,
        CONTACT_USES_SOLVER_PATH2 = 2,
        CONTACT_BREAKOFF_OBJECT_ID_SMALLER = 4,
        CONTACT_IS_DISABLED = 8,
    }
    
    public class hkContactPointMaterial
    {
        public ulong m_userData;
        public hkUFloat8 m_friction;
        public byte m_restitution;
        public hkUFloat8 m_maxImpulse;
        public byte m_flags;
    }
}
