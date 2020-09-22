using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoweredRagdollControlsModifier : hkbModifier
    {
        public hkbPoweredRagdollControlData m_controlData;
        public hkbBoneIndexArray m_bones;
        public hkbWorldFromModelModeData m_worldFromModelModeData;
        public hkbBoneWeightArray m_boneWeights;
        public float m_animationBlendFraction;
    }
}
