using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BlendType
    {
        BLEND_TYPE_BLEND_IN = 0,
        BLEND_TYPE_FULL_ON = 1,
    }
    
    public enum DockingFlagBits
    {
        FLAG_NONE = 0,
        FLAG_DOCK_TO_FUTURE_POSITION = 1,
        FLAG_OVERRIDE_MOTION = 2,
    }
    
    public class hkbDockingGenerator : hkbGenerator
    {
        public short m_dockingBone;
        public Vector4 m_translationOffset;
        public Quaternion m_rotationOffset;
        public BlendType m_blendType;
        public uint m_flags;
        public hkbGenerator m_child;
        public int m_intervalStart;
        public int m_intervalEnd;
    }
}
