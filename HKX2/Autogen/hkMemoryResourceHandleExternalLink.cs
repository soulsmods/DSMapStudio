using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryResourceHandleExternalLink : IHavokObject
    {
        public string m_memberName;
        public string m_externalId;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_memberName = des.ReadStringPointer(br);
            m_externalId = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
