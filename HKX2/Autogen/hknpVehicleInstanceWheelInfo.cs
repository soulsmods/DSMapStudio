using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleInstanceWheelInfo
    {
        public hkContactPoint m_contactPoint;
        public float m_contactFriction;
        public uint m_contactShapeKey;
        public Vector4 m_hardPointWs;
        public Vector4 m_rayEndPointWs;
        public float m_currentSuspensionLength;
        public Vector4 m_suspensionDirectionWs;
        public Vector4 m_spinAxisChassisSpace;
        public Vector4 m_spinAxisWs;
        public Quaternion m_steeringOrientationChassisSpace;
        public float m_spinVelocity;
        public float m_noSlipIdealSpinVelocity;
        public float m_spinAngle;
        public float m_skidEnergyDensity;
        public float m_sideForce;
        public float m_forwardSlipVelocity;
        public float m_sideSlipVelocity;
    }
}
