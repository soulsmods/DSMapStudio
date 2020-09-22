using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbTransformVectorModifier : hkbModifier
    {
        public Quaternion m_rotation;
        public Vector4 m_translation;
        public Vector4 m_vectorIn;
        public Vector4 m_vectorOut;
        public bool m_rotateOnly;
        public bool m_inverse;
        public bool m_computeOnActivate;
        public bool m_computeOnModify;
    }
}
