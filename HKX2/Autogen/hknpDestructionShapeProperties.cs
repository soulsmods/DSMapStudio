using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpDestructionShapeProperties : hkReferencedObject
    {
        public Matrix4x4 m_worldFromShape;
        public bool m_isHierarchicalCompound;
        public bool m_hasDestructionShapes;
    }
}
