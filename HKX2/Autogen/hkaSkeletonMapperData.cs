using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MappingType
    {
        HK_RAGDOLL_MAPPING = 0,
        HK_RETARGETING_MAPPING = 1,
    }
    
    public class hkaSkeletonMapperData
    {
        public hkaSkeleton m_skeletonA;
        public hkaSkeleton m_skeletonB;
        public List<short> m_partitionMap;
        public List<hkaSkeletonMapperDataPartitionMappingRange> m_simpleMappingPartitionRanges;
        public List<hkaSkeletonMapperDataPartitionMappingRange> m_chainMappingPartitionRanges;
        public List<hkaSkeletonMapperDataSimpleMapping> m_simpleMappings;
        public List<hkaSkeletonMapperDataChainMapping> m_chainMappings;
        public List<short> m_unmappedBones;
        public hkQTransform m_extractedMotionMapping;
        public bool m_keepUnmappedLocal;
        public MappingType m_mappingType;
    }
}
