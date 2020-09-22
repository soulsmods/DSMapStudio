using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDampingModifier : hkbModifier
    {
        public float m_kP;
        public float m_kI;
        public float m_kD;
        public bool m_enableScalarDamping;
        public bool m_enableVectorDamping;
        public float m_rawValue;
        public float m_dampedValue;
        public Vector4 m_rawVector;
        public Vector4 m_dampedVector;
        public Vector4 m_vecErrorSum;
        public Vector4 m_vecPreviousError;
        public float m_errorSum;
        public float m_previousError;
    }
}
