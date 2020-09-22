using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStaticShadowBufferDefinition : hclBufferDefinition
    {
        public List<Vector4> m_staticPositions;
        public List<Vector4> m_staticNormals;
        public List<Vector4> m_staticTangents;
        public List<Vector4> m_staticBiTangents;
        public List<ushort> m_triangleIndices;
    }
}
