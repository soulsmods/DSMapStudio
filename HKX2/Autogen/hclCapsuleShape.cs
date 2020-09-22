using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclCapsuleShape : hclShape
    {
        public Vector4 m_start;
        public Vector4 m_end;
        public Vector4 m_dir;
        public float m_radius;
        public float m_capLenSqrdInv;
    }
}
