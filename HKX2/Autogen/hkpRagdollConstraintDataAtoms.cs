using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRagdollConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_TWIST = 0,
            AXIS_PLANES = 1,
            AXIS_CROSS_PRODUCT = 2,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkpRagdollMotorConstraintAtom m_ragdollMotors;
        public hkpAngFrictionConstraintAtom m_angFriction;
        public hkpTwistLimitConstraintAtom m_twistLimit;
        public hkpConeLimitConstraintAtom m_coneLimit;
        public hkpConeLimitConstraintAtom m_planesLimit;
        public hkpBallSocketConstraintAtom m_ballSocket;
    }
}
