using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbExtrapolatingTransitionEffectInternalState : hkReferencedObject
    {
        public hkbGeneratorSyncInfo m_fromGeneratorSyncInfo;
        public hkbGeneratorPartitionInfo m_fromGeneratorPartitionInfo;
        public hkQTransform m_worldFromModel;
        public hkQTransform m_motion;
        public List<hkQTransform> m_pose;
        public List<hkQTransform> m_additivePose;
        public List<float> m_boneWeights;
        public float m_toGeneratorDuration;
        public bool m_isFromGeneratorActive;
        public bool m_gotPose;
        public bool m_gotAdditivePose;
    }
}
