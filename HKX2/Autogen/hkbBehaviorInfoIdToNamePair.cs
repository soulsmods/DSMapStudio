using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorInfoIdToNamePair : IHavokObject
    {
        public string m_behaviorName;
        public string m_nodeName;
        public NodeType m_toolType;
        public ushort m_id;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_behaviorName = des.ReadStringPointer(br);
            m_nodeName = des.ReadStringPointer(br);
            m_toolType = (NodeType)br.ReadUInt16();
            m_id = br.ReadUInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_id);
            bw.WriteUInt32(0);
        }
    }
}
