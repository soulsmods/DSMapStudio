using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCdBody : IHavokObject
    {
        public hkpShape m_shape;
        public uint m_shapeKey;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_shapeKey = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt32(m_shapeKey);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
