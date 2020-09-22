using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinSoftConstraintAtom : hkpConstraintAtom
    {
        public byte m_axisIndex;
        public float m_tau;
        public float m_damping;
    }
}
