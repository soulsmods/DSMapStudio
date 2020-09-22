using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVelocityConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public float m_tau;
        public float m_velocityTarget;
        public bool m_useVelocityTargetFromConstraintTargets;
    }
}
