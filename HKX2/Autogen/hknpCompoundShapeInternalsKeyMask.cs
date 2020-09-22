using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompoundShapeInternalsKeyMask : hknpCompoundShapeKeyMask
    {
        public hknpCompoundShape m_shape;
        public List<hknpShapeKeyMask> m_instanceMasks;
        public List<uint> m_filter;
    }
}
