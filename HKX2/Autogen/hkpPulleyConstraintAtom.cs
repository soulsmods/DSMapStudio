using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPulleyConstraintAtom : hkpConstraintAtom
    {
        public Vector4 m_fixedPivotAinWorld;
        public Vector4 m_fixedPivotBinWorld;
        public float m_ropeLength;
        public float m_leverageOnBodyB;
    }
}
