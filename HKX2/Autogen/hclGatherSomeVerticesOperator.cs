using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclGatherSomeVerticesOperator : hclOperator
    {
        public List<hclGatherSomeVerticesOperatorVertexPair> m_vertexPairs;
        public uint m_inputBufferIdx;
        public uint m_outputBufferIdx;
        public bool m_gatherNormals;
    }
}
