using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbManualSelectorTransitionEffect : hkbTransitionEffect
    {
        public List<hkbTransitionEffect> m_transitionEffects;
        public byte m_selectedIndex;
        public hkbCustomIdSelector m_indexSelector;
    }
}
