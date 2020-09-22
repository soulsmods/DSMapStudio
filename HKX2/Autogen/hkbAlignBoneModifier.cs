using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum AlignModeABAM
    {
        ALIGN_MODE_CHARACTER_WORLD_FROM_MODEL = 0,
        ALIGN_MODE_ANIMATION_SKELETON_BONE = 1,
    }
    
    public enum AlignTargetMode
    {
        ALIGN_TARGET_MODE_CHARACTER_WORLD_FROM_MODEL = 0,
        ALIGN_TARGET_MODE_RAGDOLL_SKELETON_BONE = 1,
        ALIGN_TARGET_MODE_ANIMATION_SKELETON_BONE = 2,
        ALIGN_TARGET_MODE_USER_SPECIFIED_FRAME_OF_REFERENCE = 3,
    }
    
    public class hkbAlignBoneModifier : hkbModifier
    {
        public AlignModeABAM m_alignMode;
        public AlignTargetMode m_alignTargetMode;
        public bool m_alignSingleAxis;
        public Vector4 m_alignAxis;
        public Vector4 m_alignTargetAxis;
        public Quaternion m_frameOfReference;
        public float m_duration;
        public int m_alignModeIndex;
        public int m_alignTargetModeIndex;
    }
}
