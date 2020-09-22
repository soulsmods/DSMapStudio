using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpLodShape : hknpCompositeShape
    {
        public int m_numLevelsOfDetail;
        public hknpLodShapeLevelOfDetailInfo m_infos;
        public hknpShape m_shapes;
        public uint m_shapesMemorySizes;
        public int m_indexCurrentShapeOnSpu;
        public hknpShape m_currentShapePpuAddress;
        public hkAabb m_maximumAabb;
    }
}
