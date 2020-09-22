using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum NodeTypes
    {
        NODE_TYPE_INTERNAL = 0,
        NODE_TYPE_IN = 1,
        NODE_TYPE_OUT = 2,
        NODE_TYPE_UNKNOWN = 3,
        NODE_TYPE_INVALID = 4,
        NODE_TYPE_FREE = 15,
    }
    
    public class hkcdPlanarSolid : hkcdPlanarEntity
    {
        public hkcdPlanarSolidNodeStorage m_nodes;
        public hkcdPlanarGeometryPlanesCollection m_planes;
        public uint m_rootNodeId;
    }
}
