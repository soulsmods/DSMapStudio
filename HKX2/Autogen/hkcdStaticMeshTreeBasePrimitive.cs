using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBasePrimitive
    {
        public enum Type
        {
            INVALID = 0,
            TRIANGLE = 1,
            QUAD = 2,
            CUSTOM = 3,
            NUM_TYPES = 4,
        }
        
        public byte m_indices;
    }
}
