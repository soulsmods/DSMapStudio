using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCircularSurfaceVelocity : hknpSurfaceVelocity
    {
        public bool m_velocityIsLocalSpace;
        public Vector4 m_pivot;
        public Vector4 m_angularVelocity;
    }
}
