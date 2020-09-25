using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCollidableBoundingVolumeData : IHavokObject
    {
        public uint m_min;
        public byte m_expansionMin;
        public byte m_expansionShift;
        public uint m_max;
        public byte m_expansionMax;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = br.ReadUInt32();
            br.AssertUInt64(0);
            m_expansionMin = br.ReadByte();
            br.AssertUInt16(0);
            m_expansionShift = br.ReadByte();
            m_max = br.ReadUInt32();
            br.AssertUInt64(0);
            m_expansionMax = br.ReadByte();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_min);
            bw.WriteUInt64(0);
            bw.WriteByte(m_expansionMin);
            bw.WriteUInt16(0);
            bw.WriteByte(m_expansionShift);
            bw.WriteUInt32(m_max);
            bw.WriteUInt64(0);
            bw.WriteByte(m_expansionMax);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
