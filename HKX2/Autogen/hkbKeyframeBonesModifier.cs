using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbKeyframeBonesModifier : hkbModifier
    {
        public List<hkbKeyframeBonesModifierKeyframeInfo> m_keyframeInfo;
        public hkbBoneIndexArray m_keyframedBonesList;
    }
}
