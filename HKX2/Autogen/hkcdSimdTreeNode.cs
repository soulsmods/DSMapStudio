using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdSimdTreeNode : hkcdFourAabb
    {
        public enum Flags
        {
            HAS_INTERNALS = 1,
            HAS_LEAVES = 2,
            HAS_NULLS = 4,
        }
        
        public uint m_data;
    }
}
