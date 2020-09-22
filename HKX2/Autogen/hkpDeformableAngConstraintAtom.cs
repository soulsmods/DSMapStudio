using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDeformableAngConstraintAtom : hkpConstraintAtom
    {
        public Quaternion m_offset;
        public Vector4 m_yieldStrengthDiag;
        public Vector4 m_yieldStrengthOffDiag;
        public Vector4 m_ultimateStrengthDiag;
        public Vector4 m_ultimateStrengthOffDiag;
    }
}
