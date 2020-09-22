using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTaperedCapsuleShape : hclShape
    {
        public Vector4 m_small;
        public Vector4 m_big;
        public Vector4 m_coneApex;
        public Vector4 m_coneAxis;
        public Vector4 m_lVec;
        public Vector4 m_dVec;
        public Vector4 m_tanThetaVecNeg;
        public float m_smallRadius;
        public float m_bigRadius;
        public float m_l;
        public float m_d;
        public float m_cosTheta;
        public float m_sinTheta;
        public float m_tanTheta;
        public float m_tanThetaSqr;
    }
}
