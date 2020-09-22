using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompressedMeshShape : hknpCompositeShape
    {
        public hknpCompressedMeshShapeData m_data;
        public hkBitField m_quadIsFlat;
        public hkBitField m_triangleIsInterior;
    }
}
