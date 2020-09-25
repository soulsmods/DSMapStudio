using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphInternalStateInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraphInternalState m_internalState;
        public List<hkbAuxiliaryNodeInfo> m_auxiliaryNodeInfo;
        public List<short> m_activeEventIds;
        public List<short> m_activeVariableIds;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_internalState = des.ReadClassPointer<hkbBehaviorGraphInternalState>(br);
            m_auxiliaryNodeInfo = des.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(br);
            m_activeEventIds = des.ReadInt16Array(br);
            m_activeVariableIds = des.ReadInt16Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            // Implement Write
        }
    }
}
