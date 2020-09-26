using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAuxiliaryNodeInfo : hkReferencedObject
    {
        public NodeType m_type;
        public byte m_depth;
        public string m_referenceBehaviorName;
        public List<string> m_selfTransitionNames;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (NodeType)br.ReadUInt16();
            m_depth = br.ReadByte();
            br.ReadUInt32();
            br.ReadByte();
            m_referenceBehaviorName = des.ReadStringPointer(br);
            m_selfTransitionNames = des.ReadStringPointerArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_depth);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
