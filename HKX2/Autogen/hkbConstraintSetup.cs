using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbConstraintSetup
    {
        public enum Type
        {
            BALL_AND_SOCKET = 0,
            RAGDOLL = 1,
        }
        
        public Type m_type;
    }
}
