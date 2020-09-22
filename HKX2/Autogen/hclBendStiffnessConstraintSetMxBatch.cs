using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendStiffnessConstraintSetMxBatch
    {
        public float m_weightsA;
        public float m_weightsB;
        public float m_weightsC;
        public float m_weightsD;
        public float m_bendStiffnesses;
        public float m_restCurvatures;
        public float m_invMassesA;
        public float m_invMassesB;
        public float m_invMassesC;
        public float m_invMassesD;
        public ushort m_particlesA;
        public ushort m_particlesB;
        public ushort m_particlesC;
        public ushort m_particlesD;
    }
}
