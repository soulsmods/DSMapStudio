using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Bounds
    {
        BOUND_POS_X = 0,
        BOUND_NEG_X = 1,
        BOUND_POS_Y = 2,
        BOUND_NEG_Y = 3,
        BOUND_POS_Z = 4,
        BOUND_NEG_Z = 5,
        NUM_BOUNDS = 6,
    }
    
    public class hkcdPlanarGeometryPlanesCollection : hkReferencedObject
    {
        public Vector4 m_offsetAndScale;
        public List<hkcdPlanarGeometryPrimitivesPlane> m_planes;
    }
}
