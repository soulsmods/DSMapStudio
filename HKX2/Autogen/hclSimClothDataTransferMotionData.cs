using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothDataTransferMotionData
    {
        public uint m_transformSetIndex;
        public uint m_transformIndex;
        public bool m_transferTranslationMotion;
        public float m_minTranslationSpeed;
        public float m_maxTranslationSpeed;
        public float m_minTranslationBlend;
        public float m_maxTranslationBlend;
        public bool m_transferRotationMotion;
        public float m_minRotationSpeed;
        public float m_maxRotationSpeed;
        public float m_minRotationBlend;
        public float m_maxRotationBlend;
    }
}
