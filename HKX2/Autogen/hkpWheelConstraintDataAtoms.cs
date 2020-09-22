using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_SUSPENSION = 0,
            AXIS_PERP_SUSPENSION = 1,
            AXIS_AXLE = 0,
            AXIS_STEERING = 1,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_suspensionBase;
        public hkpLinLimitConstraintAtom m_lin0Limit;
        public hkpLinSoftConstraintAtom m_lin0Soft;
        public hkpLinConstraintAtom m_lin1;
        public hkpLinConstraintAtom m_lin2;
        public hkpSetLocalRotationsConstraintAtom m_steeringBase;
        public hkp2dAngConstraintAtom m_2dAng;
    }
}
