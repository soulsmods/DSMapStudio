using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbManualSelectorGenerator : hkbGenerator
    {
        public List<hkbGenerator> m_generators;
        public short m_selectedGeneratorIndex;
        public hkbCustomIdSelector m_indexSelector;
        public bool m_selectedIndexCanChangeAfterActivate;
        public hkbTransitionEffect m_generatorChangedTransitionEffect;
    }
}
