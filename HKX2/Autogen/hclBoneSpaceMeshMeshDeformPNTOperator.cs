using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPNTOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPNT> m_localPNTs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNT> m_localUnpackedPNTs;
    }
}
