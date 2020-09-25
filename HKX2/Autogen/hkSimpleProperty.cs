using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSimpleProperty : IHavokObject
    {
        public uint m_key;
        public hkSimplePropertyValue m_value;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_key = br.ReadUInt32();
            br.AssertUInt32(0);
            m_value = new hkSimplePropertyValue();
            m_value.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_key);
            bw.WriteUInt32(0);
            m_value.Write(bw);
        }
    }
}
