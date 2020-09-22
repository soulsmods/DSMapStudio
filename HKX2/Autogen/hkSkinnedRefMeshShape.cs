using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinnedRefMeshShape : hkMeshShape
    {
        public hkSkinnedMeshShape m_skinnedMeshShape;
        public List<short> m_bones;
        public List<Vector4> m_localFromRootTransforms;
        public string m_name;
    }
}
