using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum AlignMode
    {
        ALIGN_MODE_FORWARD_RIGHT = 0,
        ALIGN_MODE_FORWARD = 1,
    }
    
    public class hkbFootIkModifier : hkbModifier
    {
        public hkbFootIkGains m_gains;
        public List<hkbFootIkModifierLeg> m_legs;
        public float m_raycastDistanceUp;
        public float m_raycastDistanceDown;
        public float m_originalGroundHeightMS;
        public float m_errorOut;
        public float m_verticalOffset;
        public uint m_collisionFilterInfo;
        public float m_forwardAlignFraction;
        public float m_sidewaysAlignFraction;
        public float m_sidewaysSampleWidth;
        public bool m_useTrackData;
        public bool m_lockFeetWhenPlanted;
        public bool m_useCharacterUpVector;
        public bool m_keepSourceFootEndAboveGround;
        public AlignMode m_alignMode;
    }
}
