using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConvexPolytopeShape : hknpConvexShape
    {
        public List<Vector4> m_planes;
        public List<hknpConvexPolytopeShapeFace> m_faces;
        public List<byte> m_indices;
    }
}
