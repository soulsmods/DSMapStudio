using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBallSocketConstraintAtom : hkpConstraintAtom
    {
        public SolvingMethod m_solvingMethod;
        public byte m_bodiesToNotify;
        public hkUFloat8 m_velocityStabilizationFactor;
        public bool m_enableLinearImpulseLimit;
        public float m_breachImpulse;
        public float m_inertiaStabilizationFactor;
    }
}
