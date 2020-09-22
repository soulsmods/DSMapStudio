using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMaskedShape : hknpDecoratorShape
    {
        public hknpShapeKeyMask m_mask;
        public hknpMaskedShapeMaskWrapper m_maskWrapper;
        public int m_maskSize;
    }
}
