using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterSetup : hkReferencedObject
    {
        public List<hkaSkeletonMapper> m_retargetingSkeletonMappers;
        public hkaSkeleton m_animationSkeleton;
        public hkaSkeletonMapper m_ragdollToAnimationSkeletonMapper;
        public hkaSkeletonMapper m_animationToRagdollSkeletonMapper;
        public hkbCharacterData m_data;
    }
}
