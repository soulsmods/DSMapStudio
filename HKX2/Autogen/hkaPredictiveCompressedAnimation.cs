using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_compressedData = des.ReadByteArray(br);
            m_intData = des.ReadUInt16Array(br);
            m_intArrayOffsets = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_floatData = des.ReadSingleArray(br);
            m_floatArrayOffsets = br.ReadInt32();
            br.AssertUInt64(0);
            m_numBones = br.ReadInt32();
            m_numFloatSlots = br.ReadInt32();
            m_numFrames = br.ReadInt32();
            m_firstFloatBlockScaleAndOffsetIndex = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_intArrayOffsets);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_floatArrayOffsets);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_numBones);
            bw.WriteInt32(m_numFloatSlots);
            bw.WriteInt32(m_numFrames);
            bw.WriteInt32(m_firstFloatBlockScaleAndOffsetIndex);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
