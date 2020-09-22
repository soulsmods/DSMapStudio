using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclCollidable : hkReferencedObject
    {
        public string m_name;
        public Matrix4x4 m_transform;
        public Vector4 m_linearVelocity;
        public Vector4 m_angularVelocity;
        public bool m_pinchDetectionEnabled;
        public char m_pinchDetectionPriority;
        public float m_pinchDetectionRadius;
        public hclShape m_shape;
    }
}
