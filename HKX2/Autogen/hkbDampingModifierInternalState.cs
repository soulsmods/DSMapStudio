using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDampingModifierInternalState : hkReferencedObject
    {
        public Vector4 m_dampedVector;
        public Vector4 m_vecErrorSum;
        public Vector4 m_vecPreviousError;
        public float m_dampedValue;
        public float m_errorSum;
        public float m_previousError;
    }
}
