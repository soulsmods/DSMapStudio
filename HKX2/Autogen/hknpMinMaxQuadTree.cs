using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMinMaxQuadTree
    {
        public List<hknpMinMaxQuadTreeMinMaxLevel> m_coarseTreeData;
        public Vector4 m_offset;
        public float m_multiplier;
        public float m_invMultiplier;
    }
}
