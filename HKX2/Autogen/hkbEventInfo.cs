using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventInfo
    {
        public enum Flags
        {
            FLAG_SILENT = 1,
            FLAG_SYNC_POINT = 2,
        }
        
        public uint m_flags;
    }
}
