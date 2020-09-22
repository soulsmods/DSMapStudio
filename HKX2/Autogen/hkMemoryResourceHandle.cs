using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryResourceHandle : hkResourceHandle
    {
        public hkReferencedObject m_variant;
        public string m_name;
        public List<hkMemoryResourceHandleExternalLink> m_references;
    }
}
