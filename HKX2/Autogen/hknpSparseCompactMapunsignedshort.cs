using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpSparseCompactMapunsignedshort
    {
        public uint m_secondaryKeyMask;
        public uint m_sencondaryKeyBits;
        public List<ushort> m_primaryKeyToIndex;
        public List<ushort> m_valueAndSecondaryKeys;
    }
}
