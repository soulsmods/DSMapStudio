using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpRagdollData : hknpPhysicsSystemData
    {
        public hkaSkeleton m_skeleton;
        public List<int> m_boneToBodyMap;
    }
}
