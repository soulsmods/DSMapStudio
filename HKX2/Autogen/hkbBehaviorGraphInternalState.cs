using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphInternalState : hkReferencedObject
    {
        public List<hkbNodeInternalStateInfo> m_nodeInternalStateInfos;
        public hkbVariableValueSet m_variableValueSet;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nodeInternalStateInfos = des.ReadClassPointerArray<hkbNodeInternalStateInfo>(br);
            m_variableValueSet = des.ReadClassPointer<hkbVariableValueSet>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
