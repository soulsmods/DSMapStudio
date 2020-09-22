using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaDefaultAnimatedReferenceFrame : hkaAnimatedReferenceFrame
    {
        public Vector4 m_up;
        public Vector4 m_forward;
        public float m_duration;
        public List<Vector4> m_referenceFrameSamples;
    }
}
