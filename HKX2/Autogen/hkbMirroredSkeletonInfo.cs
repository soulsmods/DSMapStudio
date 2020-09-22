using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbMirroredSkeletonInfo : hkReferencedObject
    {
        public Vector4 m_mirrorAxis;
        public List<short> m_bonePairMap;
        public List<short> m_partitionPairMap;
    }
}
