using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator : IHavokObject
    {
        public List<uint> m_words;
        public int m_numBits;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_words = des.ReadUInt32Array(br);
            m_numBits = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_numBits);
            bw.WriteUInt32(0);
        }
    }
}
