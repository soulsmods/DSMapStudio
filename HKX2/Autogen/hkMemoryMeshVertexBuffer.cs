using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshVertexBuffer : hkMeshVertexBuffer
    {
        public hkVertexFormat m_format;
        public int m_elementOffsets;
        public List<byte> m_memory;
        public int m_vertexStride;
        public bool m_locked;
        public int m_numVertices;
        public bool m_isBigEndian;
        public bool m_isSharable;
    }
}
