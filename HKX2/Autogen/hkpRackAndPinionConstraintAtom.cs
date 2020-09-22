using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRackAndPinionConstraintAtom : hkpConstraintAtom
    {
        public float m_pinionRadiusOrScrewPitch;
        public bool m_isScrew;
        public char m_memOffsetToInitialAngleOffset;
        public char m_memOffsetToPrevAngle;
        public char m_memOffsetToRevolutionCounter;
    }
}
