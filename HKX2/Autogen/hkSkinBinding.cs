using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinBinding : hkMeshShape
    {
        public hkMeshShape m_skin;
        public List<Matrix4x4> m_worldFromBoneTransforms;
        public List<string> m_boneNames;
    }
}
