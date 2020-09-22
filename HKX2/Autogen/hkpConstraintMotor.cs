using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MotorType
    {
        TYPE_INVALID = 0,
        TYPE_POSITION = 1,
        TYPE_VELOCITY = 2,
        TYPE_SPRING_DAMPER = 3,
        TYPE_CALLBACK = 4,
        TYPE_MAX = 5,
    }
    
    public class hkpConstraintMotor : hkReferencedObject
    {
        public MotorType m_type;
    }
}
