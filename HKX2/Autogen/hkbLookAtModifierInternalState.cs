using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLookAtModifierInternalState : hkReferencedObject
    {
        public Vector4 m_lookAtLastTargetWS;
        public float m_lookAtWeight;
        public bool m_isTargetInsideLimitCone;
    }
}
