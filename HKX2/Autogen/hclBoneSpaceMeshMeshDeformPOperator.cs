using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockP> m_localPs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedP> m_localUnpackedPs;
    }
}
