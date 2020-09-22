using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSetupStabilizationAtom : hkpConstraintAtom
    {
        public bool m_enabled;
        public float m_maxLinImpulse;
        public float m_maxAngImpulse;
        public float m_maxAngle;
    }
}
