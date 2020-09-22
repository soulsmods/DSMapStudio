using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeDirectionModifierInternalState : hkReferencedObject
    {
        public Vector4 m_pointOut;
        public float m_groundAngleOut;
        public float m_upAngleOut;
        public bool m_computedOutput;
    }
}
