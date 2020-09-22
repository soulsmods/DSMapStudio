using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleData : hkReferencedObject
    {
        public Vector4 m_gravity;
        public char m_numWheels;
        public Quaternion m_chassisOrientation;
        public float m_torqueRollFactor;
        public float m_torquePitchFactor;
        public float m_torqueYawFactor;
        public float m_extraTorqueFactor;
        public float m_maxVelocityForPositionalFriction;
        public float m_chassisUnitInertiaYaw;
        public float m_chassisUnitInertiaRoll;
        public float m_chassisUnitInertiaPitch;
        public float m_frictionEqualizer;
        public float m_normalClippingAngleCos;
        public float m_maxFrictionSolverMassRatio;
        public List<hknpVehicleDataWheelComponentParams> m_wheelParams;
        public List<char> m_numWheelsPerAxle;
        public hkpVehicleFrictionDescription m_frictionDescription;
        public Vector4 m_chassisFrictionInertiaInvDiag;
        public bool m_alreadyInitialised;
    }
}
