using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterControllerModifierInternalState : hkReferencedObject
    {
        public Vector4 m_gravity;
        public bool m_isInitialVelocityAdded;
        public bool m_isTouchingGround;
    }
}
