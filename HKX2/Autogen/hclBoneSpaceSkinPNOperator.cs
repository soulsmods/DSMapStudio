using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceSkinPNOperator : hclBoneSpaceSkinOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPN> m_localPNs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPN> m_localUnpackedPNs;
    }
}
