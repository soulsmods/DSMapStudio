using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinPNOperator : hclObjectSpaceSkinOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPN> m_localPNs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPN> m_localUnpackedPNs;
    }
}
