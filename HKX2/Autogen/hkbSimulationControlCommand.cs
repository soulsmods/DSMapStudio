using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SimulationControlCommand
    {
        COMMAND_PLAY = 0,
        COMMAND_PAUSE = 1,
        COMMAND_STEP = 2,
        COMMAND_STOP = 3,
        COMMAND_ACCUMULATE_MOTION = 4,
        COMMAND_DO_NOT_ACCUMULATE_MOTION = 5,
    }
    
    public class hkbSimulationControlCommand : hkReferencedObject
    {
        public SimulationControlCommand m_command;
    }
}
