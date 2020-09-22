using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceMeshMeshDeformPOperator : hclObjectSpaceMeshMeshDeformOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockP> m_localPs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedP> m_localUnpackedPs;
    }
}
