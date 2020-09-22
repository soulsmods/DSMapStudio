using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbVariableValueSet : hkReferencedObject
    {
        public List<hkbVariableValue> m_wordVariableValues;
        public List<Vector4> m_quadVariableValues;
        public List<hkReferencedObject> m_variantVariableValues;
    }
}
