using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclConvexGeometryShape : hclShape
    {
        public List<ushort> m_tetrahedraGrid;
        public List<byte> m_gridCells;
        public List<Matrix4x4> m_tetrahedraEquations;
        public Matrix4x4 m_localFromWorld;
        public Matrix4x4 m_worldFromLocal;
        public hkAabb m_objAabb;
        public Vector4 m_geomCentroid;
        public Vector4 m_invCellSize;
        public ushort m_gridRes;
    }
}
