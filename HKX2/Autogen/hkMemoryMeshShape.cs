using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshShape : hkMeshShape
    {
        public List<hkMemoryMeshShapeSection> m_sections;
        public List<ushort> m_indices16;
        public List<uint> m_indices32;
        public string m_name;
    }
}
