using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkClassEnum : IHavokObject
    {
        public enum FlagValues
        {
            FLAGS_NONE = 0,
        }
        
        public string m_name;
        public List<hkClassEnumItem> m_items;
        public uint m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            // Read TYPE_SIMPLEARRAY
            br.AssertUInt64(0);
            m_flags = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Read TYPE_SIMPLEARRAY
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
