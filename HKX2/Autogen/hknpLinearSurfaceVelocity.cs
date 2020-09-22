using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ProjectMethod
    {
        VELOCITY_PROJECT = 0,
        VELOCITY_RESCALE = 1,
    }
    
    public class hknpLinearSurfaceVelocity : hknpSurfaceVelocity
    {
        public Space m_space;
        public ProjectMethod m_projectMethod;
        public float m_maxVelocityScale;
        public Vector4 m_velocityMeasurePlane;
        public Vector4 m_velocity;
    }
}
