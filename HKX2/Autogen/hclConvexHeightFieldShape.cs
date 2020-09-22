using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclConvexHeightFieldShape : hclShape
    {
        public ushort m_res;
        public ushort m_resIncBorder;
        public Vector4 m_floatCorrectionOffset;
        public List<byte> m_heights;
        public int m_faces;
        public Matrix4x4 m_localToMapTransform;
        public Vector4 m_localToMapScale;
    }
}
