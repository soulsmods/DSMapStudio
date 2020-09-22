using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetWorldFromModelModifier : hkbModifier
    {
        public Vector4 m_translation;
        public Quaternion m_rotation;
        public bool m_setTranslation;
        public bool m_setRotation;
    }
}
