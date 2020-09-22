using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum InitialVelocityCoordinates
    {
        INITIAL_VELOCITY_IN_WORLD_COORDINATES = 0,
        INITIAL_VELOCITY_IN_MODEL_COORDINATES = 1,
    }
    
    public enum MotionMode
    {
        MOTION_MODE_FOLLOW_ANIMATION = 0,
        MOTION_MODE_DYNAMIC = 1,
    }
    
    public class hkbCharacterControllerModifier : hkbModifier
    {
        public hkbCharacterControllerModifierControlData m_controlData;
        public Vector4 m_initialVelocity;
        public InitialVelocityCoordinates m_initialVelocityCoordinates;
        public MotionMode m_motionMode;
        public bool m_forceDownwardMomentum;
        public bool m_applyGravity;
        public bool m_setInitialVelocity;
        public bool m_isTouchingGround;
    }
}
