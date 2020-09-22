using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMultipleVertexBuffer : hkMeshVertexBuffer
    {
        public hkVertexFormat m_vertexFormat;
        public List<hkMultipleVertexBufferLockedElement> m_lockedElements;
        public hkMemoryMeshVertexBuffer m_lockedBuffer;
        public List<hkMultipleVertexBufferElementInfo> m_elementInfos;
        public List<hkMultipleVertexBufferVertexBufferInfo> m_vertexBufferInfos;
        public int m_numVertices;
        public bool m_isLocked;
        public uint m_updateCount;
        public bool m_writeLock;
        public bool m_isSharable;
        public bool m_constructionComplete;
    }
}
