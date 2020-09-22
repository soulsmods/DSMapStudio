using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleInstance : hknpUnaryAction
    {
        public hknpVehicleData m_data;
        public hknpVehicleDriverInput m_driverInput;
        public hknpVehicleSteering m_steering;
        public hknpVehicleEngine m_engine;
        public hknpVehicleTransmission m_transmission;
        public hknpVehicleBrake m_brake;
        public hknpVehicleSuspension m_suspension;
        public hknpVehicleAerodynamics m_aerodynamics;
        public hknpVehicleWheelCollide m_wheelCollide;
        public hknpTyremarksInfo m_tyreMarks;
        public hknpVehicleVelocityDamper m_velocityDamper;
        public List<hknpVehicleInstanceWheelInfo> m_wheelsInfo;
        public hkpVehicleFrictionStatus m_frictionStatus;
        public hknpVehicleDriverInputStatus m_deviceStatus;
        public List<bool> m_isFixed;
        public float m_wheelsTimeSinceMaxPedalInput;
        public bool m_tryingToReverse;
        public float m_torque;
        public float m_rpm;
        public float m_mainSteeringAngle;
        public float m_mainSteeringAngleAssumingNoReduction;
        public List<float> m_wheelsSteeringAngle;
        public bool m_isReversing;
        public char m_currentGear;
        public bool m_delayed;
        public float m_clutchDelayCountdown;
    }
}
