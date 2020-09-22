using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGeneratorNestedTypes : hkbCustomTestGeneratorNestedTypesBase
    {
        public hkbCustomTestGeneratorNestedTypesBase m_nestedTypeStruct;
        public List<hkbCustomTestGeneratorNestedTypesBase> m_nestedTypeArrayStruct;
    }
}
