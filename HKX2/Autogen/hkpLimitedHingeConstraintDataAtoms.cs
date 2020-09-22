using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLimitedHingeConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_AXLE = 0,
            AXIS_PERP_TO_AXLE_1 = 1,
            AXIS_PERP_TO_AXLE_2 = 2,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkpAngMotorConstraintAtom m_angMotor;
        public hkpAngFrictionConstraintAtom m_angFriction;
        public hkpAngLimitConstraintAtom m_angLimit;
        public hkp2dAngConstraintAtom m_2dAng;
        public hkpBallSocketConstraintAtom m_ballSocket;
    }
}
