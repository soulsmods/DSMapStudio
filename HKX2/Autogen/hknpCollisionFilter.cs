using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCollisionFilter : hkReferencedObject
    {
        public enum Type
        {
            TYPE_UNKNOWN = 0,
            TYPE_CONSTRAINT = 1,
            TYPE_GROUP = 2,
            TYPE_PAIR = 3,
            TYPE_USER = 4,
        }
        
        public Type m_type;
    }
}
