using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclRuntimeConversionInfoElementConversion : IHavokObject
    {
        public byte m_index;
        public byte m_offset;
        public VectorConversion m_conversion;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_index = br.ReadByte();
            m_offset = br.ReadByte();
            m_conversion = (VectorConversion)br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_index);
            bw.WriteByte(m_offset);
        }
    }
}
