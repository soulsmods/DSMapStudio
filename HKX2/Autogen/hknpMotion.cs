using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMotion
    {
        public Vector4 m_centerOfMassAndMassFactor;
        public Quaternion m_orientation;
        public ushort m_inverseInertia;
        public uint m_firstAttachedBodyId;
        public ushort m_linearVelocityCage;
        public ushort m_integrationFactor;
        public ushort m_motionPropertiesId;
        public ushort m_maxLinearAccelerationDistancePerStep;
        public ushort m_maxRotationToPreventTunneling;
        public byte m_cellIndex;
        public byte m_spaceSplitterWeight;
        public Vector4 m_linearVelocity;
        public Vector4 m_angularVelocity;
        public Vector4 m_previousStepLinearVelocity;
        public Vector4 m_previousStepAngularVelocity;
    }
}
