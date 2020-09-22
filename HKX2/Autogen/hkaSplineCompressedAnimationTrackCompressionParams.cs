using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum RotationQuantization
    {
        POLAR32 = 0,
        THREECOMP40 = 1,
        THREECOMP48 = 2,
        THREECOMP24 = 3,
        STRAIGHT16 = 4,
        UNCOMPRESSED = 5,
    }
    
    public enum ScalarQuantization
    {
        BITS8 = 0,
        BITS16 = 1,
    }
    
    public class hkaSplineCompressedAnimationTrackCompressionParams
    {
        public float m_rotationTolerance;
        public float m_translationTolerance;
        public float m_scaleTolerance;
        public float m_floatingTolerance;
        public ushort m_rotationDegree;
        public ushort m_translationDegree;
        public ushort m_scaleDegree;
        public ushort m_floatingDegree;
        public RotationQuantization m_rotationQuantizationType;
        public ScalarQuantization m_translationQuantizationType;
        public ScalarQuantization m_scaleQuantizationType;
        public ScalarQuantization m_floatQuantizationType;
    }
}
