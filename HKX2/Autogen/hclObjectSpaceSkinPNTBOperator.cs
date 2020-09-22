using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinPNTBOperator : hclObjectSpaceSkinOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
    }
}
