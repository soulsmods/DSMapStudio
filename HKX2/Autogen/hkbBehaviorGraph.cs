using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VariableMode
    {
        VARIABLE_MODE_DISCARD_WHEN_INACTIVE = 0,
        VARIABLE_MODE_MAINTAIN_VALUES_WHEN_INACTIVE = 1,
    }
    
    public class hkbBehaviorGraph : hkbGenerator
    {
        public VariableMode m_variableMode;
        public hkbGenerator m_rootGenerator;
        public hkbBehaviorGraphData m_data;
    }
}
