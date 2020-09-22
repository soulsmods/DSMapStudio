using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkClassEnum
    {
        public enum FlagValues
        {
            FLAGS_NONE = 0,
        }
        
        public string m_name;
        public List<hkClassEnumItem> m_items;
        public uint m_flags;
    }
}
