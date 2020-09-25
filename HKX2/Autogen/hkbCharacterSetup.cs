using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_retargetingSkeletonMappers = des.ReadClassPointerArray<hkaSkeletonMapper>(br);
            m_animationSkeleton = des.ReadClassPointer<hkaSkeleton>(br);
            m_ragdollToAnimationSkeletonMapper = des.ReadClassPointer<hkaSkeletonMapper>(br);
            m_animationToRagdollSkeletonMapper = des.ReadClassPointer<hkaSkeletonMapper>(br);
            br.AssertUInt64(0);
            m_data = des.ReadClassPointer<hkbCharacterData>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
