using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshMeshDeformOperator : hclOperator
    {
        public enum ScaleNormalBehaviour
        {
            SCALE_NORMAL_IGNORE = 0,
            SCALE_NORMAL_APPLY = 1,
            SCALE_NORMAL_INVERT = 2,
        }
        
        public List<ushort> m_inputTrianglesSubset;
        public List<hclMeshMeshDeformOperatorTriangleVertexPair> m_triangleVertexPairs;
        public List<ushort> m_triangleVertexStartForVertex;
        public uint m_inputBufferIdx;
        public uint m_outputBufferIdx;
        public ushort m_startVertex;
        public ushort m_endVertex;
        public ScaleNormalBehaviour m_scaleNormalBehaviour;
        public bool m_deformNormals;
        public bool m_partialDeform;
    }
}
