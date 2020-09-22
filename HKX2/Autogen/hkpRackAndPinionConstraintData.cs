using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRackAndPinionConstraintData : hkpConstraintData
    {
        public enum Type
        {
            TYPE_RACK_AND_PINION = 0,
            TYPE_SCREW = 1,
        }
        
        public hkpRackAndPinionConstraintDataAtoms m_atoms;
    }
}
