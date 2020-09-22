using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum GizmoType
    {
        POINT = 0,
        SPHERE = 1,
        PLANE = 2,
        ARROW = 3,
    }
    
    public class hkGizmoAttribute
    {
        public bool m_visible;
        public string m_label;
        public GizmoType m_type;
    }
}
