using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Result
    {
        RESULT_OK = 0,
        RESULT_REMOVE = 1,
    }
    
    public class hknpAction : hkReferencedObject
    {
        public ulong m_userData;
    }
}
