using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceMeshMeshDeformPNTBOperator : hclObjectSpaceMeshMeshDeformOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
    }
}
