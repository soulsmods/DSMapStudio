using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSpringDamperConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public float m_springConstant;
        public float m_springDamping;
    }
}
