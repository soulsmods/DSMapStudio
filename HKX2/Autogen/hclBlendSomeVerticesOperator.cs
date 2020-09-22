using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VectorContext
    {
        VEC_POSITION = 0,
        VEC_DIRECTION = 1,
    }
    
    public class hclBlendSomeVerticesOperator : hclOperator
    {
        public List<hclBlendSomeVerticesOperatorBlendEntry> m_blendEntries;
        public uint m_bufferIdx_A;
        public uint m_bufferIdx_B;
        public uint m_bufferIdx_C;
        public bool m_blendNormals;
        public bool m_blendTangents;
        public bool m_blendBitangents;
    }
}
