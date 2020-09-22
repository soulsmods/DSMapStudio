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
    }
}
