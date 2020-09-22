using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbFootIkDriverInfo : hkReferencedObject
    {
        public List<hkbFootIkDriverInfoLeg> m_legs;
        public float m_raycastDistanceUp;
        public float m_raycastDistanceDown;
        public float m_originalGroundHeightMS;
        public float m_verticalOffset;
        public uint m_collisionFilterInfo;
        public float m_forwardAlignFraction;
        public float m_sidewaysAlignFraction;
        public float m_sidewaysSampleWidth;
        public bool m_lockFeetWhenPlanted;
        public bool m_useCharacterUpVector;
        public bool m_isQuadrupedNarrow;
        public bool m_keepSourceFootEndAboveGround;
    }
}
