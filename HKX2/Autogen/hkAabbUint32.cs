using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkAabbUint32
    {
        public uint m_min;
        public byte m_expansionMin;
        public byte m_expansionShift;
        public uint m_max;
        public byte m_expansionMax;
        public byte m_shapeKeyByte;
    }
}
