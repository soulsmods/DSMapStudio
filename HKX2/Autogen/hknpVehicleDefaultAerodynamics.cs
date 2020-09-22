using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDefaultAerodynamics : hknpVehicleAerodynamics
    {
        public float m_airDensity;
        public float m_frontalArea;
        public float m_dragCoefficient;
        public float m_liftCoefficient;
        public Vector4 m_extraGravityws;
    }
}
