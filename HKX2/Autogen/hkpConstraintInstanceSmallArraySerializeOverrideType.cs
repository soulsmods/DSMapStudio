using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConstraintInstanceSmallArraySerializeOverrideType : IHavokObject
    {
        public ushort m_size;
        public ushort m_capacityAndFlags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt64(0);
            m_size = br.ReadUInt16();
            m_capacityAndFlags = br.ReadUInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt16(m_size);
            bw.WriteUInt16(m_capacityAndFlags);
            bw.WriteUInt32(0);
        }
    }
}
