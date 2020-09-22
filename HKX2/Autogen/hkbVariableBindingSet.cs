using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbVariableBindingSet : hkReferencedObject
    {
        public List<hkbVariableBindingSetBinding> m_bindings;
        public int m_indexOfBindingToEnable;
    }
}
