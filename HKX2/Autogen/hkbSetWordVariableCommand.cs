using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetWordVariableCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public string m_variableName;
        public hkbVariableValue m_value;
        public Vector4 m_quadValue;
        public VariableType m_type;
        public bool m_global;
    }
}
