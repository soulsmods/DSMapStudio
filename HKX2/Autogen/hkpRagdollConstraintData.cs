using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MotorIndex
    {
        MOTOR_TWIST = 0,
        MOTOR_PLANE = 1,
        MOTOR_CONE = 2,
    }
    
    public class hkpRagdollConstraintData : hkpConstraintData
    {
        public hkpRagdollConstraintDataAtoms m_atoms;
    }
}
