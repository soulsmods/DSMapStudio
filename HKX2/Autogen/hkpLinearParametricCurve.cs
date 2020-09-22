using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinearParametricCurve : hkpParametricCurve
    {
        public float m_smoothingFactor;
        public bool m_closedLoop;
        public Vector4 m_dirNotParallelToTangentAlongWholePath;
        public List<Vector4> m_points;
        public List<float> m_distance;
    }
}
