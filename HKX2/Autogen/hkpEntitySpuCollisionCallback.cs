using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpEntitySpuCollisionCallback : IHavokObject
    {
        public byte m_eventFilter;
        public byte m_userFilter;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            m_eventFilter = br.ReadByte();
            m_userFilter = br.ReadByte();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(m_eventFilter);
            bw.WriteByte(m_userFilter);
            bw.WriteUInt32(0);
        }
    }
}
