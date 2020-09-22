using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkVertexFormatElement
    {
        public ComponentType m_dataType;
        public byte m_numValues;
        public ComponentUsage m_usage;
        public byte m_subUsage;
        public uint m_flags;
        public byte m_pad;
    }
}
