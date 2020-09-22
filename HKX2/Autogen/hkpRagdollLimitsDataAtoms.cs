using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRagdollLimitsDataAtoms
    {
        public enum Axis
        {
            AXIS_TWIST = 0,
            AXIS_PLANES = 1,
            AXIS_CROSS_PRODUCT = 2,
        }
        
        public hkpSetLocalRotationsConstraintAtom m_rotations;
        public hkpTwistLimitConstraintAtom m_twistLimit;
        public hkpConeLimitConstraintAtom m_coneLimit;
        public hkpConeLimitConstraintAtom m_planesLimit;
    }
}
