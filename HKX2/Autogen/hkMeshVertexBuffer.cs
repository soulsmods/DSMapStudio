using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum LockResult
    {
        RESULT_FAILURE = 0,
        RESULT_SUCCESS = 1,
        RESULT_IN_USE = 2,
    }
    
    public class hkMeshVertexBuffer : hkReferencedObject
    {
        public enum Flags
        {
            ACCESS_READ = 1,
            ACCESS_WRITE = 2,
            ACCESS_READ_WRITE = 3,
            ACCESS_WRITE_DISCARD = 4,
            ACCESS_ELEMENT_ARRAY = 8,
        }
        
    }
}
