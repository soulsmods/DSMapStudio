using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ReferenceCountHandling
    {
        REFERENCE_COUNT_INCREMENT = 0,
        REFERENCE_COUNT_IGNORE = 1,
    }
    
    public class hkRefCountedProperties
    {
        public List<hkRefCountedPropertiesEntry> m_entries;
    }
}
