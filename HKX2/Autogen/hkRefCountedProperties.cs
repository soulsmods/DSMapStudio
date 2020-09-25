using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ReferenceCountHandling
    {
        REFERENCE_COUNT_INCREMENT = 0,
        REFERENCE_COUNT_IGNORE = 1,
    }
    
    public class hkRefCountedProperties : IHavokObject
    {
        public List<hkRefCountedPropertiesEntry> m_entries;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_entries = des.ReadClassArray<hkRefCountedPropertiesEntry>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
