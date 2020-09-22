using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBufferUsage
    {
        public enum Component
        {
            COMPONENT_POSITION = 0,
            COMPONENT_NORMAL = 1,
            COMPONENT_TANGENT = 2,
            COMPONENT_BITANGENT = 3,
        }
        
        public enum InternalFlags
        {
            USAGE_NONE = 0,
            USAGE_READ = 1,
            USAGE_WRITE = 2,
            USAGE_FULL_WRITE = 4,
            USAGE_READ_BEFORE_WRITE = 8,
        }
        
        public byte m_perComponentFlags;
        public bool m_trianglesRead;
    }
}
