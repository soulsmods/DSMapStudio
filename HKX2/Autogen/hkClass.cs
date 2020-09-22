using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SignatureFlags
    {
        SIGNATURE_LOCAL = 1,
    }
    
    public class hkClass
    {
        public enum FlagValues
        {
            FLAGS_NONE = 0,
            FLAGS_NOT_SERIALIZABLE = 1,
        }
        
        public string m_name;
        public hkClass m_parent;
        public int m_objectSize;
        public int m_numImplementedInterfaces;
        public List<hkClassEnum> m_declaredEnums;
        public List<hkClassMember> m_declaredMembers;
        public uint m_flags;
        public int m_describedVersion;
    }
}
