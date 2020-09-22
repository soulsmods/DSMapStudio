using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclScratchBufferDefinition : hclBufferDefinition
    {
        public List<ushort> m_triangleIndices;
        public bool m_storeNormals;
        public bool m_storeTangentsAndBiTangents;
    }
}
