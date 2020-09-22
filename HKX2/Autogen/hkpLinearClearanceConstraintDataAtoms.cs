using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinearClearanceConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_SHAFT = 0,
            AXIS_PERP_TO_SHAFT = 1,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpLinMotorConstraintAtom m_motor;
        public hkpLinFrictionConstraintAtom m_friction0;
        public hkpLinFrictionConstraintAtom m_friction1;
        public hkpLinFrictionConstraintAtom m_friction2;
        public hkpAngConstraintAtom m_ang;
        public hkpLinLimitConstraintAtom m_linLimit0;
        public hkpLinLimitConstraintAtom m_linLimit1;
        public hkpLinLimitConstraintAtom m_linLimit2;
    }
}
