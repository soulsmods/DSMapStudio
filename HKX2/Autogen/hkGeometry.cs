using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum GeometryType
    {
        GEOMETRY_STATIC = 0,
        GEOMETRY_DYNAMIC_VERTICES = 1,
    }
    
    public class hkGeometry : hkReferencedObject
    {
        public List<Vector4> m_vertices;
        public List<hkGeometryTriangle> m_triangles;
    }
}
