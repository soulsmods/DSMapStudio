using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SimulationState
    {
        SIMULATION_STATE_PLAY = 0,
        SIMULATION_STATE_PAUSE = 1,
        SIMULATION_STATE_STEP = 2,
        SIMULATION_STATE_STOP = 3,
    }
    
    public enum AccumulateMotionState
    {
        ACCUMULATE_MOTION = 0,
        DO_NOT_ACCUMULATE_MOTION = 1,
    }
    
    public class hkbWorldEnums
    {
    }
}
