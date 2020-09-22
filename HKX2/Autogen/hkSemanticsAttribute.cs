using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Semantics
    {
        UNKNOWN = 0,
        DISTANCE = 1,
        ANGLE = 2,
        NORMAL = 3,
        POSITION = 4,
        COSINE_ANGLE = 5,
    }
    
    public class hkSemanticsAttribute
    {
        public Semantics m_type;
    }
}
