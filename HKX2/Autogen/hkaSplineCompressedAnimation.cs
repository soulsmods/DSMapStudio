using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSplineCompressedAnimation : hkaAnimation
    {
        public int m_numFrames;
        public int m_numBlocks;
        public int m_maxFramesPerBlock;
        public int m_maskAndQuantizationSize;
        public float m_blockDuration;
        public float m_blockInverseDuration;
        public float m_frameDuration;
        public List<uint> m_blockOffsets;
        public List<uint> m_floatBlockOffsets;
        public List<uint> m_transformOffsets;
        public List<uint> m_floatOffsets;
        public List<byte> m_data;
        public int m_endian;
    }
}
