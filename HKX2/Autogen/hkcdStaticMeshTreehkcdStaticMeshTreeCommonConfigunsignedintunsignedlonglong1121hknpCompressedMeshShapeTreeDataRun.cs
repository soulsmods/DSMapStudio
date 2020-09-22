using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum TriangleMaterial
    {
        TM_SET_FROM_TRIANGLE_DATA_TYPE = 0,
        TM_SET_FROM_PRIMITIVE_KEY = 1,
    }
    
    public class hkcdStaticMeshTreehkcdStaticMeshTreeCommonConfigunsignedintunsignedlonglong1121hknpCompressedMeshShapeTreeDataRun : hkcdStaticMeshTreeBase
    {
        public List<uint> m_packedVertices;
        public List<ulong> m_sharedVertices;
        public List<hknpCompressedMeshShapeTreeDataRun> m_primitiveDataRuns;
    }
}
