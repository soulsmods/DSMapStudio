using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaMeshBinding : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public string m_name;
        public hkaSkeleton m_skeleton;
        public List<hkaMeshBindingMapping> m_mappings;
        public List<Matrix4x4> m_boneFromSkinMeshTransforms;
    }
}
