using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Link
    {
        NONE = 0,
        DIRECT_LINK = 1,
        CHILD = 2,
        MESH = 3,
        PARENT_NAME = 4,
        IMGSELECT = 5,
    }
    
    public class hkLinkAttribute
    {
        public Link m_type;
    }
}
