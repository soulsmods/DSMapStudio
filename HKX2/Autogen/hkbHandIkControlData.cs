using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkControlData
    {
        public enum HandleChangeMode
        {
            HANDLE_CHANGE_MODE_ABRUPT = 0,
            HANDLE_CHANGE_MODE_CONSTANT_VELOCITY = 1,
        }
        
        public Vector4 m_targetPosition;
        public Quaternion m_targetRotation;
        public Vector4 m_targetNormal;
        public hkbHandle m_targetHandle;
        public float m_transformOnFraction;
        public float m_normalOnFraction;
        public float m_fadeInDuration;
        public float m_fadeOutDuration;
        public float m_extrapolationTimeStep;
        public float m_handleChangeSpeed;
        public HandleChangeMode m_handleChangeMode;
        public bool m_fixUp;
    }
}
