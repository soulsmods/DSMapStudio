using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclGatherAllVerticesOperator : hclOperator
    {
        public List<short> m_vertexInputFromVertexOutput;
        public uint m_inputBufferIdx;
        public uint m_outputBufferIdx;
        public bool m_gatherNormals;
        public bool m_partialGather;
    }
}
