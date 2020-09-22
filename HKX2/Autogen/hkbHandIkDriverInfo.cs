using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkDriverInfo : hkReferencedObject
    {
        public List<hkbHandIkDriverInfoHand> m_hands;
        public BlendCurve m_fadeInOutCurve;
    }
}
