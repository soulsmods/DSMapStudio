using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLayerGeneratorInternalState : hkReferencedObject
    {
        public int m_numActiveLayers;
        public List<hkbLayerGeneratorLayerInternalState> m_layerInternalStates;
        public bool m_initSync;
    }
}
