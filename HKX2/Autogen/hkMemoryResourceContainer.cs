using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryResourceContainer : hkResourceContainer
    {
        public string m_name;
        public List<hkMemoryResourceHandle> m_resourceHandles;
        public List<hkMemoryResourceContainer> m_children;
    }
}
