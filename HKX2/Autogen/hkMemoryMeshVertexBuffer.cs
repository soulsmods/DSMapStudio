using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_format = new hkVertexFormat();
            m_format.Read(des, br);
            m_elementOffsets = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_memory = des.ReadByteArray(br);
            m_vertexStride = br.ReadInt32();
            m_locked = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_numVertices = br.ReadInt32();
            m_isBigEndian = br.ReadBoolean();
            m_isSharable = br.ReadBoolean();
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_format.Write(bw);
            bw.WriteInt32(m_elementOffsets);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_vertexStride);
            bw.WriteBoolean(m_locked);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteInt32(m_numVertices);
            bw.WriteBoolean(m_isBigEndian);
            bw.WriteBoolean(m_isSharable);
            bw.WriteUInt16(0);
        }
    }
}
