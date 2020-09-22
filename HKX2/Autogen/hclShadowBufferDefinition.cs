using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclShadowBufferDefinition : hclBufferDefinition
    {
        public List<ushort> m_triangleIndices;
        public bool m_shadowPositions;
        public bool m_shadowNormals;
        public bool m_shadowTangents;
        public bool m_shadowBiTangents;
    }
}
