using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGetUpModifier : hkbModifier
    {
        public Vector4 m_groundNormal;
        public float m_duration;
        public float m_alignWithGroundDuration;
        public short m_rootBoneIndex;
        public short m_otherBoneIndex;
        public short m_anotherBoneIndex;
    }
}
