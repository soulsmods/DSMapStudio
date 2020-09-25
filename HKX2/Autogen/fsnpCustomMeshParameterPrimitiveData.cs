using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class fsnpCustomMeshParameterPrimitiveData : IHavokObject
    {
        public List<byte> m_vertexData;
        public List<byte> m_triangleData;
        public List<byte> m_primitiveData;
        public uint m_materialNameData;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vertexData = des.ReadByteArray(br);
            m_triangleData = des.ReadByteArray(br);
            m_primitiveData = des.ReadByteArray(br);
            m_materialNameData = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_materialNameData);
            bw.WriteUInt32(0);
        }
    }
}
