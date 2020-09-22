using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCogWheelConstraintAtom : hkpConstraintAtom
    {
        public float m_cogWheelRadiusA;
        public float m_cogWheelRadiusB;
        public bool m_isScrew;
        public char m_memOffsetToInitialAngleOffset;
        public char m_memOffsetToPrevAngle;
        public char m_memOffsetToRevolutionCounter;
    }
}
