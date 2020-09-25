using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticPvsBlockHeader : IHavokObject
    {
        public uint m_offset;
        public uint m_length;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = br.ReadUInt32();
            m_length = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_offset);
            bw.WriteUInt32(m_length);
        }
    }
}
