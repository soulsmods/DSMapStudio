using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceSkinPNTOperator : hclBoneSpaceSkinOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPNT> m_localPNTs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNT> m_localUnpackedPNTs;
    }
}
