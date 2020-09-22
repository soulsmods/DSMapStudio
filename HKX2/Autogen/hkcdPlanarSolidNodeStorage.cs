using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarSolidNodeStorage : hkReferencedObject
    {
        public List<hkcdPlanarSolidNode> m_storage;
        public uint m_firstFreeNodeId;
    }
}
