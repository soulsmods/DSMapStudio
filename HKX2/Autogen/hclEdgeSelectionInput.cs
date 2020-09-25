using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum EdgeSelectionType
    {
        EDGE_SELECTION_ALL = 0,
        EDGE_SELECTION_NONE = 1,
        EDGE_SELECTION_CHANNEL = 2,
        EDGE_SELECTION_INVERSE_CHANNEL = 3,
    }
    
    public class hclEdgeSelectionInput : IHavokObject
    {
        public EdgeSelectionType m_type;
        public string m_channelName;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = (EdgeSelectionType)br.ReadUInt32();
            br.AssertUInt32(0);
            m_channelName = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(0);
        }
    }
}
