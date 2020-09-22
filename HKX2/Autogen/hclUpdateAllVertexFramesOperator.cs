using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclUpdateAllVertexFramesOperator : hclOperator
    {
        public List<ushort> m_vertToNormalID;
        public List<byte> m_triangleFlips;
        public List<ushort> m_referenceVertices;
        public List<float> m_tangentEdgeCosAngle;
        public List<float> m_tangentEdgeSinAngle;
        public List<float> m_biTangentFlip;
        public uint m_bufferIdx;
        public uint m_numUniqueNormalIDs;
        public bool m_updateNormals;
        public bool m_updateTangents;
        public bool m_updateBiTangents;
    }
}
