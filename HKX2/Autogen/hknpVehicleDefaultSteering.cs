using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDefaultSteering : hknpVehicleSteering
    {
        public float m_maxSteeringAngle;
        public float m_maxSpeedFullSteeringAngle;
        public List<bool> m_doesWheelSteer;
    }
}
