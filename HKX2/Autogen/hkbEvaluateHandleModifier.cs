using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEvaluateHandleModifier : hkbModifier
    {
        public enum HandleChangeMode
        {
            HANDLE_CHANGE_MODE_ABRUPT = 0,
            HANDLE_CHANGE_MODE_CONSTANT_VELOCITY = 1,
        }
        
        public hkbHandle m_handle;
        public Vector4 m_handlePositionOut;
        public Quaternion m_handleRotationOut;
        public bool m_isValidOut;
        public float m_extrapolationTimeStep;
        public float m_handleChangeSpeed;
        public HandleChangeMode m_handleChangeMode;
    }
}
