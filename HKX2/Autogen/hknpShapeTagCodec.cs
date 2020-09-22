using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpShapeTagCodec : hkReferencedObject
    {
        public enum Type
        {
            TYPE_NULL = 0,
            TYPE_MATERIAL_PALETTE = 1,
            TYPE_UFM = 2,
            TYPE_USER = 3,
        }
        
        public Type m_type;
    }
}
