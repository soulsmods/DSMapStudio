using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BindingType
    {
        BINDING_TYPE_VARIABLE = 0,
        BINDING_TYPE_CHARACTER_PROPERTY = 1,
    }
    
    public enum InternalBindingFlags
    {
        FLAG_NONE = 0,
        FLAG_OUTPUT = 1,
    }
    
    public class hkbVariableBindingSetBinding
    {
        public string m_memberPath;
        public int m_variableIndex;
        public char m_bitIndex;
        public BindingType m_bindingType;
    }
}
