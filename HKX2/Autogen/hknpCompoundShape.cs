using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompoundShape : hknpCompositeShape
    {
        public hkFreeListArrayhknpShapeInstancehkHandleshort32767hknpShapeInstanceIdDiscriminant8hknpShapeInstance m_instances;
        public hkAabb m_aabb;
        public bool m_isMutable;
    }
}
