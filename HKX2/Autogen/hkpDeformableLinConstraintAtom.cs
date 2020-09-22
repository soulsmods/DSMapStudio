using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDeformableLinConstraintAtom : hkpConstraintAtom
    {
        public Vector4 m_offset;
        public Vector4 m_yieldStrengthDiag;
        public Vector4 m_yieldStrengthOffDiag;
        public Vector4 m_ultimateStrengthDiag;
        public Vector4 m_ultimateStrengthOffDiag;
    }
}
