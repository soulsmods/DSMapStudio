using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclConvexPlanesShape : hclShape
    {
        public List<Vector4> m_planeEquations;
        public Matrix4x4 m_localFromWorld;
        public Matrix4x4 m_worldFromLocal;
        public hkAabb m_objAabb;
        public Vector4 m_geomCentroid;
    }
}
