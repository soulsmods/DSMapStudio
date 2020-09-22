using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ConstraintSource
    {
        NO_CONSTRAINTS = 0,
        REFERENCE_POSE = 1,
        CURRENT_POSE = 2,
    }
    
    public class hkaSkeletonMapper : hkReferencedObject
    {
        public hkaSkeletonMapperData m_mapping;
    }
}
