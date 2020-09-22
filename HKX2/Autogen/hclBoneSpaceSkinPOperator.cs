using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceSkinPOperator : hclBoneSpaceSkinOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockP> m_localPs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedP> m_localUnpackedPs;
    }
}
