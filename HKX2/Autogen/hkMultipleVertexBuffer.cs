using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexFormat = new hkVertexFormat();
            m_vertexFormat.Read(des, br);
            br.AssertUInt32(0);
            m_lockedElements = des.ReadClassArray<hkMultipleVertexBufferLockedElement>(br);
            m_lockedBuffer = des.ReadClassPointer<hkMemoryMeshVertexBuffer>(br);
            m_elementInfos = des.ReadClassArray<hkMultipleVertexBufferElementInfo>(br);
            m_vertexBufferInfos = des.ReadClassArray<hkMultipleVertexBufferVertexBufferInfo>(br);
            m_numVertices = br.ReadInt32();
            m_isLocked = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_updateCount = br.ReadUInt32();
            m_writeLock = br.ReadBoolean();
            m_isSharable = br.ReadBoolean();
            m_constructionComplete = br.ReadBoolean();
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_vertexFormat.Write(bw);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteInt32(m_numVertices);
            bw.WriteBoolean(m_isLocked);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteUInt32(m_updateCount);
            bw.WriteBoolean(m_writeLock);
            bw.WriteBoolean(m_isSharable);
            bw.WriteBoolean(m_constructionComplete);
            bw.WriteByte(0);
        }
    }
}
