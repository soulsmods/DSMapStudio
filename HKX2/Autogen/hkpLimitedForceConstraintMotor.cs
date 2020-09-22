using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLimitedForceConstraintMotor : hkpConstraintMotor
    {
        public float m_minForce;
        public float m_maxForce;
    }
}
