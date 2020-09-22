using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinFrictionConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_frictionAxis;
        public float m_maxFrictionForce;
    }
}
