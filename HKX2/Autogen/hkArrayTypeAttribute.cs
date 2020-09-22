using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ArrayType
    {
        NONE = 0,
        POINTSOUP = 1,
        ENTITIES = 2,
    }
    
    public class hkArrayTypeAttribute
    {
        public ArrayType m_type;
    }
}
