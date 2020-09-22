using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimpleWindAction : hclAction
    {
        public Vector4 m_windDirection;
        public float m_windMinSpeed;
        public float m_windMaxSpeed;
        public float m_windFrequency;
        public float m_maximumDrag;
        public Vector4 m_airVelocity;
        public float m_currentTime;
    }
}
