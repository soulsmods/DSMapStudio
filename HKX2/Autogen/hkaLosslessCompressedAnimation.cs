using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum LosslessTrackType
    {
        TRACK_TYPE_CLEAR = 0,
        TRACK_TYPE_STATIC = 1,
        TRACK_TYPE_DYNAMIC = 2,
    }
    
    public class hkaLosslessCompressedAnimation : hkaAnimation
    {
        public List<float> m_dynamicTranslations;
        public List<float> m_staticTranslations;
        public List<ulong> m_translationTypeAndOffsets;
        public List<Quaternion> m_dynamicRotations;
        public List<Quaternion> m_staticRotations;
        public List<ushort> m_rotationTypeAndOffsets;
        public List<float> m_dynamicScales;
        public List<float> m_staticScales;
        public List<ulong> m_scaleTypeAndOffsets;
        public List<float> m_floats;
        public int m_numFrames;
    }
}
