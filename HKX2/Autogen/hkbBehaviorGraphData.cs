using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphData : hkReferencedObject
    {
        public List<float> m_attributeDefaults;
        public List<hkbVariableInfo> m_variableInfos;
        public List<hkbVariableInfo> m_characterPropertyInfos;
        public List<hkbEventInfo> m_eventInfos;
        public List<hkbVariableBounds> m_variableBounds;
        public hkbVariableValueSet m_variableInitialValues;
        public hkbBehaviorGraphStringData m_stringData;
    }
}
