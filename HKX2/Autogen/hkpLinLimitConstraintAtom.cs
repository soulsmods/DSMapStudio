using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinLimitConstraintAtom : hkpConstraintAtom
    {
        public byte m_axisIndex;
        public float m_min;
        public float m_max;
    }
}
