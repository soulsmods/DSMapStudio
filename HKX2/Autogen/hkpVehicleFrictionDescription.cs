using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVehicleFrictionDescription : hkReferencedObject
    {
        public float m_wheelDistance;
        public float m_chassisMassInv;
        public hkpVehicleFrictionDescriptionAxisDescription m_axleDescr;
    }
}
