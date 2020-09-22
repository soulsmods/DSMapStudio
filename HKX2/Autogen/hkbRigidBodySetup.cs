using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRigidBodySetup
    {
        public enum Type
        {
            INVALID = -1,
            KEYFRAMED = 0,
            DYNAMIC = 1,
            FIXED = 2,
        }
        
        public uint m_collisionFilterInfo;
        public Type m_type;
        public hkbShapeSetup m_shapeSetup;
    }
}
