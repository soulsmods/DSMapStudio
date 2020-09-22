using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclUpdateSomeVertexFramesOperator : hclOperator
    {
        public List<hclUpdateSomeVertexFramesOperatorTriangle> m_involvedTriangles;
        public List<ushort> m_involvedVertices;
        public List<ushort> m_selectionVertexToInvolvedVertex;
        public List<ushort> m_involvedVertexToNormalID;
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
