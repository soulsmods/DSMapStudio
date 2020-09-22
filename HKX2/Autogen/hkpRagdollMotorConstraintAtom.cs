using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRagdollMotorConstraintAtom : hkpConstraintAtom
    {
        public bool m_isEnabled;
        public Matrix4x4 m_target_bRca;
        public hkpConstraintMotor m_motors;
    }
}
