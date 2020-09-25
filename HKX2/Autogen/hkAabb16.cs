using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkAabb16 : IHavokObject
    {
        public ushort m_min;
        public ushort m_key;
        public ushort m_max;
        public ushort m_key1;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = br.ReadUInt16();
            br.AssertUInt32(0);
            m_key = br.ReadUInt16();
            m_max = br.ReadUInt16();
            br.AssertUInt32(0);
            m_key1 = br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_min);
            bw.WriteUInt32(0);
            bw.WriteUInt16(m_key);
            bw.WriteUInt16(m_max);
            bw.WriteUInt32(0);
            bw.WriteUInt16(m_key1);
        }
    }
}
