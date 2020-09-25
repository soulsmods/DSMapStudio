using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VertexFloatType
    {
        VERTEX_FLOAT_CONSTANT = 0,
        VERTEX_FLOAT_CHANNEL = 1,
    }
    
    public class hclVertexFloatInput : IHavokObject
    {
        public VertexFloatType m_type;
        public float m_constantValue;
        public string m_channelName;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = (VertexFloatType)br.ReadUInt32();
            m_constantValue = br.ReadSingle();
            m_channelName = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_constantValue);
        }
    }
}
