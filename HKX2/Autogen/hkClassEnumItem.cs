using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkClassEnumItem : IHavokObject
    {
        public int m_value;
        public string m_name;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = br.ReadInt32();
            br.ReadUInt32();
            m_name = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_value);
            bw.WriteUInt32(0);
        }
    }
}
