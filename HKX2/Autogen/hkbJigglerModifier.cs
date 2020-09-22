using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum JiggleCoordinates
    {
        JIGGLE_IN_WORLD_COORDINATES = 0,
        JIGGLE_IN_MODEL_COORDINATES = 1,
    }
    
    public class hkbJigglerModifier : hkbModifier
    {
        public List<hkbJigglerGroup> m_jigglerGroups;
        public JiggleCoordinates m_jiggleCoordinates;
    }
}
