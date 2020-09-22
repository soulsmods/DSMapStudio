using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum PlaybackMode
    {
        MODE_SINGLE_PLAY = 0,
        MODE_LOOPING = 1,
        MODE_USER_CONTROLLED = 2,
        MODE_PING_PONG = 3,
        MODE_COUNT = 4,
    }
    
    public enum ClipFlags
    {
        FLAG_CONTINUE_MOTION_AT_END = 1,
        FLAG_SYNC_HALF_CYCLE_IN_PING_PONG_MODE = 2,
        FLAG_MIRROR = 4,
        FLAG_FORCE_DENSE_POSE = 8,
        FLAG_DONT_CONVERT_ANNOTATIONS_TO_TRIGGERS = 16,
        FLAG_IGNORE_MOTION = 32,
    }
    
    public class hkbClipGenerator : hkbGenerator
    {
        public string m_animationBundleName;
        public string m_animationName;
        public hkbClipTriggerArray m_triggers;
        public uint m_userPartitionMask;
        public float m_cropStartAmountLocalTime;
        public float m_cropEndAmountLocalTime;
        public float m_startTime;
        public float m_playbackSpeed;
        public float m_enforcedDuration;
        public float m_userControlledTimeFraction;
        public short m_animationBindingIndex;
        public PlaybackMode m_mode;
        public char m_flags;
    }
}
