using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRigidBodyRagdollControlsModifier : hkbModifier
    {
        public hkbRigidBodyRagdollControlData m_controlData;
        public hkbBoneIndexArray m_bones;
        public float m_animationBlendFraction;
    }
}
