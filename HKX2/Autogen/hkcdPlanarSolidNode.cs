using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarSolidNode
    {
        public uint m_parent;
        public uint m_left;
        public uint m_right;
        public uint m_nextFreeNodeId;
        public uint m_planeId;
        public uint m_data;
        public uint m_typeAndFlags;
    }
}
