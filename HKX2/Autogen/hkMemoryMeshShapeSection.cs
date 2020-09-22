using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshShapeSection
    {
        public hkMeshVertexBuffer m_vertexBuffer;
        public hkMeshMaterial m_material;
        public hkMeshBoneIndexMapping m_boneMatrixMap;
        public PrimitiveType m_primitiveType;
        public int m_numPrimitives;
        public MeshSectionIndexType m_indexType;
        public int m_vertexStartIndex;
        public int m_transformIndex;
        public int m_indexBufferOffset;
    }
}
