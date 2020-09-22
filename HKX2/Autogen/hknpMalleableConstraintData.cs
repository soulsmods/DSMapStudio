using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMalleableConstraintData : hkpWrappedConstraintData
    {
        public hknpBridgeConstraintAtom m_atom;
        public bool m_wantsRuntime;
        public float m_strength;
    }
}
