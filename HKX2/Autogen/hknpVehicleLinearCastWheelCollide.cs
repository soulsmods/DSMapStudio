using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleLinearCastWheelCollide : hknpVehicleWheelCollide
    {
        public List<hknpVehicleLinearCastWheelCollideWheelState> m_wheelStates;
        public float m_maxExtraPenetration;
        public float m_startPointTolerance;
        public uint m_chassisBody;
    }
}
