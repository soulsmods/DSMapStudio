using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkModifier : hkbModifier
    {
        public List<hkbHandIkModifierHand> m_hands;
        public BlendCurve m_fadeInOutCurve;
    }
}
