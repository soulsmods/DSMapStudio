using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSkeleton : hkReferencedObject
    {
        public string m_name;
        public List<short> m_parentIndices;
        public List<hkaBone> m_bones;
        public List<hkQTransform> m_referencePose;
        public List<float> m_referenceFloats;
        public List<string> m_floatSlots;
        public List<hkaSkeletonLocalFrameOnBone> m_localFrames;
        public List<hkaSkeletonPartition> m_partitions;
    }
}
