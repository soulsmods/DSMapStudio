using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBaseSection : hkcdStaticTreeTreehkcdStaticTreeDynamicStorage4
    {
        public enum Flags
        {
            SF_REQUIRE_TREE = 1,
        }
        
        public float m_codecParms;
        public uint m_firstPackedVertex;
        public hkcdStaticMeshTreeBaseSectionSharedVertices m_sharedVertices;
        public hkcdStaticMeshTreeBaseSectionPrimitives m_primitives;
        public hkcdStaticMeshTreeBaseSectionDataRuns m_dataRuns;
        public byte m_numPackedVertices;
        public byte m_numSharedIndices;
        public ushort m_leafIndex;
        public byte m_page;
        public byte m_flags;
        public byte m_layerData;
        public byte m_unusedData;
    }
}
