using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelConstraintData : hkpConstraintData
    {
        public hkpWheelConstraintDataAtoms m_atoms;
        public Vector4 m_initialAxleInB;
        public Vector4 m_initialSteeringAxisInB;
    }
}
