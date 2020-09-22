using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPositionConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public float m_tau;
        public float m_damping;
        public float m_proportionalRecoveryVelocity;
        public float m_constantRecoveryVelocity;
    }
}
