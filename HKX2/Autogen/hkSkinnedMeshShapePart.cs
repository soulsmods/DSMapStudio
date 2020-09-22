using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinnedMeshShapePart
    {
        public int m_startVertex;
        public int m_numVertices;
        public int m_startIndex;
        public int m_numIndices;
        public ushort m_boneSetId;
        public ushort m_meshSectionIndex;
        public Vector4 m_boundingSphere;
    }
}
