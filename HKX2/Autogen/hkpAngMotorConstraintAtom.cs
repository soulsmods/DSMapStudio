using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpAngMotorConstraintAtom : hkpConstraintAtom
    {
        public bool m_isEnabled;
        public byte m_motorAxis;
        public float m_targetAngle;
        public hkpConstraintMotor m_motor;
    }
}
