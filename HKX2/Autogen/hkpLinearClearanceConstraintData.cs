using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinearClearanceConstraintData : hkpConstraintData
    {
        public enum Type
        {
            PRISMATIC = 0,
            HINGE = 1,
            BALL_SOCKET = 2,
        }
        
        public hkpLinearClearanceConstraintDataAtoms m_atoms;
    }
}
