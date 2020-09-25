using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkCustomAttributes : IHavokObject
    {
        public List<hkCustomAttributesAttribute> m_attributes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            // Read TYPE_SIMPLEARRAY
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Read TYPE_SIMPLEARRAY
        }
    }
}
