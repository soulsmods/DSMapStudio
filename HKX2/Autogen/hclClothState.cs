using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothState : hkReferencedObject
    {
        public string m_name;
        public List<uint> m_operators;
        public List<hclClothStateBufferAccess> m_usedBuffers;
        public List<hclClothStateTransformSetAccess> m_usedTransformSets;
        public List<uint> m_usedSimCloths;
    }
}
