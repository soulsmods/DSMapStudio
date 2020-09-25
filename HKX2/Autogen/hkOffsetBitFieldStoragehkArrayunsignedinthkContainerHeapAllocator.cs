using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkOffsetBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator : IHavokObject
    {
        public List<uint> m_words;
        public int m_offset;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_words = des.ReadUInt32Array(br);
            m_offset = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_offset);
            bw.WriteUInt32(0);
        }
    }
}
