using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkUuid : IHavokObject
    {
        public uint m_data;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_data = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_data);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
