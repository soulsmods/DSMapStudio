using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbJigglerModifierInternalState : hkReferencedObject
    {
        public List<Vector4> m_currentVelocitiesWS;
        public List<Vector4> m_currentPositions;
        public float m_timeStep;
        public bool m_initNextModify;
    }
}
