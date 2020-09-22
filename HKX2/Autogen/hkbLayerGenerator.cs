using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum LayerFlagBits
    {
        FLAG_SYNC = 1,
    }
    
    public class hkbLayerGenerator : hkbGenerator
    {
        public List<hkbLayer> m_layers;
        public short m_indexOfSyncMasterChild;
        public uint m_flags;
    }
}
