using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPrismaticConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_SHAFT = 0,
            AXIS_PERP_TO_SHAFT = 1,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpLinMotorConstraintAtom m_motor;
        public hkpLinFrictionConstraintAtom m_friction;
        public hkpAngConstraintAtom m_ang;
        public hkpLinConstraintAtom m_lin0;
        public hkpLinConstraintAtom m_lin1;
        public hkpLinLimitConstraintAtom m_linLimit;
    }
}
