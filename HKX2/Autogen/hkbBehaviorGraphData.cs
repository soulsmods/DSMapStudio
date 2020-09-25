using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_attributeDefaults = des.ReadSingleArray(br);
            m_variableInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_characterPropertyInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_eventInfos = des.ReadClassArray<hkbEventInfo>(br);
            m_variableBounds = des.ReadClassArray<hkbVariableBounds>(br);
            m_variableInitialValues = des.ReadClassPointer<hkbVariableValueSet>(br);
            m_stringData = des.ReadClassPointer<hkbBehaviorGraphStringData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
