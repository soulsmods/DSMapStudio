using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothDataCollidableTransformMap
    {
        public int m_transformSetIndex;
        public List<uint> m_transformIndices;
        public List<Matrix4x4> m_offsets;
    }
}
