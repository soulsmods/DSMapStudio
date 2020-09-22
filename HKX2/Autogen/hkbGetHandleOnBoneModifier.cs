using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGetHandleOnBoneModifier : hkbModifier
    {
        public hkbHandle m_handleOut;
        public string m_localFrameName;
        public short m_ragdollBoneIndex;
        public short m_animationBoneIndex;
    }
}
