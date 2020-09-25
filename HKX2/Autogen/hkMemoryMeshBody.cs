using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshBody : hkMeshBody
    {
        public Matrix4x4 m_transform;
        public hkIndexedTransformSet m_transformSet;
        public hkMeshShape m_shape;
        public List<hkMeshVertexBuffer> m_vertexBuffers;
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transform = des.ReadMatrix4(br);
            m_transformSet = des.ReadClassPointer<hkIndexedTransformSet>(br);
            m_shape = des.ReadClassPointer<hkMeshShape>(br);
            m_vertexBuffers = des.ReadClassPointerArray<hkMeshVertexBuffer>(br);
            m_name = des.ReadStringPointer(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
