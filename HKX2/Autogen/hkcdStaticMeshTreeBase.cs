using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CompressionMode
    {
        CM_GLOBAL = 0,
        CM_LOCAL_4 = 1,
        CM_LOCAL_2 = 2,
        CM_AUTO = 3,
    }
    
    public class hkcdStaticMeshTreeBase : hkcdStaticTreeTreehkcdStaticTreeDynamicStorage5
    {
        public int m_numPrimitiveKeys;
        public int m_bitsPerKey;
        public uint m_maxKeyValue;
        public List<hkcdStaticMeshTreeBaseSection> m_sections;
        public List<hkcdStaticMeshTreeBasePrimitive> m_primitives;
        public List<ushort> m_sharedVerticesIndex;
    }
}
