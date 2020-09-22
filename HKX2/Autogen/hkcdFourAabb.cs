using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BoundIndex
    {
        X_MIN = 0,
        X_MAX = 1,
        Y_MIN = 2,
        Y_MAX = 3,
        Z_MIN = 4,
        Z_MAX = 5,
    }
    
    public class hkcdFourAabb
    {
        public Vector4 m_lx;
        public Vector4 m_hx;
        public Vector4 m_ly;
        public Vector4 m_hy;
        public Vector4 m_lz;
        public Vector4 m_hz;
    }
}
