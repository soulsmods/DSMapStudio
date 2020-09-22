using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDetectCloseToGroundModifier : hkbModifier
    {
        public hkbEventProperty m_closeToGroundEvent;
        public float m_closeToGroundHeight;
        public float m_raycastDistanceDown;
        public uint m_collisionFilterInfo;
        public short m_boneIndex;
        public short m_animBoneIndex;
    }
}
