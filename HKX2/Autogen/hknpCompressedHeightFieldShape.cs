using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompressedHeightFieldShape : hknpHeightFieldShape
    {
        public List<ushort> m_storage;
        public List<ushort> m_shapeTags;
        public bool m_triangleFlip;
        public float m_offset;
        public float m_scale;
    }
}
