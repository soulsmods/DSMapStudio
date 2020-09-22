using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum WorldFromModelMode
    {
        WORLD_FROM_MODEL_MODE_USE_OLD = 0,
        WORLD_FROM_MODEL_MODE_USE_INPUT = 1,
        WORLD_FROM_MODEL_MODE_COMPUTE = 2,
        WORLD_FROM_MODEL_MODE_NONE = 3,
        WORLD_FROM_MODEL_MODE_RAGDOLL = 4,
    }
    
    public class hkbWorldFromModelModeData
    {
        public short m_poseMatchingBone0;
        public short m_poseMatchingBone1;
        public short m_poseMatchingBone2;
        public WorldFromModelMode m_mode;
    }
}
