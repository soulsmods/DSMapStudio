using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbShapeSetup
    {
        public enum Type
        {
            CAPSULE = 0,
            FILE = 1,
        }
        
        public float m_capsuleHeight;
        public float m_capsuleRadius;
        public string m_fileName;
        public Type m_type;
    }
}
