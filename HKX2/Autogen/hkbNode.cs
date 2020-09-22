using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum GetChildrenFlagBits
    {
        FLAG_ACTIVE_ONLY = 1,
        FLAG_GENERATORS_ONLY = 4,
        FLAG_IGNORE_REFERENCED_BEHAVIORS = 8,
    }
    
    public enum CloneState
    {
        CLONE_STATE_DEFAULT = 0,
        CLONE_STATE_TEMPLATE = 1,
        CLONE_STATE_CLONE = 2,
    }
    
    public enum TemplateOrClone
    {
        NODE_IS_TEMPLATE = 0,
        NODE_IS_CLONE = 1,
    }
    
    public class hkbNode : hkbBindable
    {
        public ulong m_userData;
        public string m_name;
    }
}
