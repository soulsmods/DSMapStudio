using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpTyremarksInfo : hkReferencedObject
    {
        public float m_minTyremarkEnergy;
        public float m_maxTyremarkEnergy;
        public List<hknpTyremarksWheel> m_tyremarksWheel;
    }
}
