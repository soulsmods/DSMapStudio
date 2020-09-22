using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGeneratorHiddenTypes : hkbReferencePoseGenerator
    {
        public bool m_inheritedHiddenMember;
        public bool m_protectedInheritedHiddenMember;
        public bool m_privateInheritedHiddenMember;
    }
}
