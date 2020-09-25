using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxAttributeGroup : IHavokObject
    {
        public string m_name;
        public List<hkxAttribute> m_attributes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_attributes = des.ReadClassArray<hkxAttribute>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
