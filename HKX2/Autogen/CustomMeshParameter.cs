using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class CustomMeshParameter : hkReferencedObject
    {
        public uint m_version;
        public List<byte> m_vertexDataBuffer;
        public int m_vertexDataStride;
        public List<byte> m_primitiveDataBuffer;
        public uint m_materialNameData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_version = br.ReadUInt32();
            br.AssertUInt32(0);
            // Read TYPE_SIMPLEARRAY
            m_vertexDataStride = br.ReadInt32();
            // Read TYPE_SIMPLEARRAY
            m_materialNameData = br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_version);
            bw.WriteUInt32(0);
            // Read TYPE_SIMPLEARRAY
            bw.WriteInt32(m_vertexDataStride);
            // Read TYPE_SIMPLEARRAY
            bw.WriteUInt32(m_materialNameData);
        }
    }
}
