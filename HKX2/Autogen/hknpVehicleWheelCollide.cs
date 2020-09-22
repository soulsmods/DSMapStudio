using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum WheelCollideType
    {
        INVALID_WHEEL_COLLIDE = 0,
        RAY_CAST_WHEEL_COLLIDE = 1,
        LINEAR_CAST_WHEEL_COLLIDE = 2,
        USER_WHEEL_COLLIDE1 = 3,
        USER_WHEEL_COLLIDE2 = 4,
        USER_WHEEL_COLLIDE3 = 5,
        USER_WHEEL_COLLIDE4 = 6,
        USER_WHEEL_COLLIDE5 = 7,
    }
    
    public class hknpVehicleWheelCollide : hkReferencedObject
    {
        public bool m_alreadyUsed;
    }
}
