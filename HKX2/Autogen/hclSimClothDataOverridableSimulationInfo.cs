using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothDataOverridableSimulationInfo
    {
        public Vector4 m_gravity;
        public float m_globalDampingPerSecond;
        public float m_collisionTolerance;
        public uint m_subSteps;
        public bool m_pinchDetectionEnabled;
        public bool m_landscapeCollisionEnabled;
        public bool m_transferMotionEnabled;
    }
}
