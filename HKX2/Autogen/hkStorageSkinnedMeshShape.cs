using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkStorageSkinnedMeshShape : hkSkinnedMeshShape
    {
        public List<short> m_bonesBuffer;
        public List<hkSkinnedMeshShapeBoneSet> m_boneSets;
        public List<hkSkinnedMeshShapeBoneSection> m_boneSections;
        public List<hkSkinnedMeshShapePart> m_parts;
        public string m_name;
    }
}
