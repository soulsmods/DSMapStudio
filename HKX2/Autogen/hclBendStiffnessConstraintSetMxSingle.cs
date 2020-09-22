using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendStiffnessConstraintSetMxSingle
    {
        public float m_weightA;
        public float m_weightB;
        public float m_weightC;
        public float m_weightD;
        public float m_bendStiffness;
        public float m_restCurvature;
        public float m_invMassA;
        public float m_invMassB;
        public float m_invMassC;
        public float m_invMassD;
        public ushort m_particleA;
        public ushort m_particleB;
        public ushort m_particleC;
        public ushort m_particleD;
    }
}
