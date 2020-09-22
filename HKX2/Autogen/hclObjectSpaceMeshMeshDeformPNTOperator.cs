using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceMeshMeshDeformPNTOperator : hclObjectSpaceMeshMeshDeformOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPNT> m_localPNTs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPNT> m_localUnpackedPNTs;
    }
}
