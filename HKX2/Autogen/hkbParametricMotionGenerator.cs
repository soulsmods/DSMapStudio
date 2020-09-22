using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MotionSpaceType
    {
        MST_UNKNOWN = 0,
        MST_ANGULAR = 1,
        MST_DIRECTIONAL = 2,
    }
    
    public class hkbParametricMotionGenerator : hkbProceduralBlenderGenerator
    {
        public MotionSpaceType m_motionSpace;
        public List<hkbGenerator> m_generators;
        public float m_xAxisParameterValue;
        public float m_yAxisParameterValue;
    }
}
