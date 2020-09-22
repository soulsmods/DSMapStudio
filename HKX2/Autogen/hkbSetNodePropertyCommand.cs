using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetNodePropertyCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public string m_nodeName;
        public string m_propertyName;
        public hkbVariableValue m_propertyValue;
        public int m_padding;
    }
}
