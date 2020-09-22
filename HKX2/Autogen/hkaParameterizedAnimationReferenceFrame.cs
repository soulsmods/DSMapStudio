using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ParameterType
    {
        UNKNOWN = 0,
        LINEAR_SPEED = 1,
        LINEAR_DIRECTION = 2,
        TURN_SPEED = 3,
    }
    
    public class hkaParameterizedAnimationReferenceFrame : hkaDefaultAnimatedReferenceFrame
    {
        public List<float> m_parameterValues;
        public List<int> m_parameterTypes;
    }
}
