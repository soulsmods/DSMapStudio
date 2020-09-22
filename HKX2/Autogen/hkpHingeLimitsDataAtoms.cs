using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpHingeLimitsDataAtoms
    {
        public enum Axis
        {
            AXIS_AXLE = 0,
            AXIS_PERP_TO_AXLE_1 = 1,
            AXIS_PERP_TO_AXLE_2 = 2,
        }
        
        public hkpSetLocalRotationsConstraintAtom m_rotations;
        public hkpAngLimitConstraintAtom m_angLimit;
        public hkp2dAngConstraintAtom m_2dAng;
    }
}
