using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransformSetUsageTransformTracker
    {
        public hkBitField m_read;
        public hkBitField m_readBeforeWrite;
        public hkBitField m_written;
    }
}
