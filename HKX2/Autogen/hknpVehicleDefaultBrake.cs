using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDefaultBrake : hknpVehicleBrake
    {
        public List<hknpVehicleDefaultBrakeWheelBrakingProperties> m_wheelBrakingProperties;
        public float m_wheelsMinTimeToBlock;
    }
}
