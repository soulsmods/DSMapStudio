using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkCustomAttributesAttribute : IHavokObject
    {
        public string m_name;
        public ulong m_value;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            // Read TYPE_VARIANT
            br.ReadUInt64();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Read TYPE_VARIANT
            bw.WriteUInt64(0);
        }
    }
}
