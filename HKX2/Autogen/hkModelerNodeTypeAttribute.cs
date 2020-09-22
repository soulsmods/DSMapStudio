using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ModelerType
    {
        DEFAULT = 0,
        LOCATOR = 1,
    }
    
    public class hkModelerNodeTypeAttribute
    {
        public ModelerType m_type;
    }
}
