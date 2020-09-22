using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum StorageClass
    {
        STORAGE_STATIC = 0,
        STORAGE_REFERENCE = 1,
        STORAGE_DYNAMIC_RANGE = 2,
        STORAGE_DYNAMIC_FIXED = 3,
    }
    
    public enum IntArrayID
    {
        BLOCK_OFFSETS = 0,
        FIRST_FLOAT_BLOCK_OFFSETS = 1,
        IS_ANIMATED_BITMAP = 2,
        IS_FIXED_RANGE_BITMAP = 3,
        DYNAMIC_BONE_TRACK_INDEX = 4,
        DYNAMIC_FLOAT_TRACK_INDEX = 5,
        STATIC_BONE_TRACK_INDEX = 6,
        STATIC_FLOAT_TRACK_INDEX = 7,
        RENORM_QUATERNION_INDEX = 8,
        NUM_INT_ARRAYS = 9,
    }
    
    public enum FloatArrayID
    {
        STATIC_VALUES = 0,
        DYNAMIC_SCALES = 1,
        DYNAMIC_OFFSETS = 2,
        NUM_FLOAT_ARRAYS = 3,
    }
    
    public class hkaPredictiveCompressedAnimation : hkaAnimation
    {
        public List<byte> m_compressedData;
        public List<ushort> m_intData;
        public int m_intArrayOffsets;
        public List<float> m_floatData;
        public int m_floatArrayOffsets;
        public int m_numBones;
        public int m_numFloatSlots;
        public int m_numFrames;
        public int m_firstFloatBlockScaleAndOffsetIndex;
    }
}
