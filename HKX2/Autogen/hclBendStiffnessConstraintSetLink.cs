using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendStiffnessConstraintSetLink
    {
        public float m_weightA;
        public float m_weightB;
        public float m_weightC;
        public float m_weightD;
        public float m_bendStiffness;
        public float m_restCurvature;
        public ushort m_particleA;
        public ushort m_particleB;
        public ushort m_particleC;
        public ushort m_particleD;
    }
}
