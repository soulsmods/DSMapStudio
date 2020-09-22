using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Mode
    {
        MODE_MATCH = 0,
        MODE_PLAY = 1,
    }
    
    public class hkbPoseMatchingGenerator : hkbBlenderGenerator
    {
        public Quaternion m_worldFromModelRotation;
        public float m_blendSpeed;
        public float m_minSpeedToSwitch;
        public float m_minSwitchTimeNoError;
        public float m_minSwitchTimeFullError;
        public int m_startPlayingEventId;
        public int m_startMatchingEventId;
        public short m_rootBoneIndex;
        public short m_otherBoneIndex;
        public short m_anotherBoneIndex;
        public short m_pelvisIndex;
        public Mode m_mode;
    }
}
