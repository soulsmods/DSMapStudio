using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpHingeConstraintDataAtoms
    {
        public enum Axis
        {
            AXIS_AXLE = 0,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkp2dAngConstraintAtom m_2dAng;
        public hkpBallSocketConstraintAtom m_ballSocket;
    }
}
