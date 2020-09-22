using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPNTBOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
    }
}
