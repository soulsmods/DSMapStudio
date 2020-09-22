using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPNOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPN> m_localPNs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPN> m_localUnpackedPNs;
    }
}
