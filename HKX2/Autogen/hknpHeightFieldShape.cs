using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpHeightFieldShape : hknpCompositeShape
    {
        public hkAabb m_aabb;
        public Vector4 m_floatToIntScale;
        public Vector4 m_intToFloatScale;
        public int m_intSizeX;
        public int m_intSizeZ;
        public int m_numBitsX;
        public int m_numBitsZ;
        public hknpMinMaxQuadTree m_minMaxTree;
        public int m_minMaxTreeCoarseness;
        public bool m_includeShapeKeyInSdfContacts;
    }
}
