using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCombineTransformsModifier : hkbModifier
    {
        public Vector4 m_translationOut;
        public Quaternion m_rotationOut;
        public Vector4 m_leftTranslation;
        public Quaternion m_leftRotation;
        public Vector4 m_rightTranslation;
        public Quaternion m_rightRotation;
        public bool m_invertLeftTransform;
        public bool m_invertRightTransform;
        public bool m_invertResult;
    }
}
