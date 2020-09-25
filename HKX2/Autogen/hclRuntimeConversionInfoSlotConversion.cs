using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclRuntimeConversionInfoSlotConversion : IHavokObject
    {
        public byte m_elements;
        public byte m_numElements;
        public byte m_index;
        public bool m_partialWrite;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elements = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_numElements = br.ReadByte();
            m_index = br.ReadByte();
            m_partialWrite = br.ReadBoolean();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_elements);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteByte(m_numElements);
            bw.WriteByte(m_index);
            bw.WriteBoolean(m_partialWrite);
        }
    }
}
