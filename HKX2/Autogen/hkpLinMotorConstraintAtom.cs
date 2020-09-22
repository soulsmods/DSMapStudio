using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinMotorConstraintAtom : hkpConstraintAtom
    {
        public bool m_isEnabled;
        public byte m_motorAxis;
        public float m_targetPosition;
        public hkpConstraintMotor m_motor;
    }
}
