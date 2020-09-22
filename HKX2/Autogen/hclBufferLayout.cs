using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SlotFlags
    {
        SF_NO_ALIGNED_START = 0,
        SF_16BYTE_ALIGNED_START = 1,
        SF_64BYTE_ALIGNED_START = 3,
    }
    
    public enum TriangleFormat
    {
        TF_THREE_INT32S = 0,
        TF_THREE_INT16S = 1,
        TF_OTHER = 2,
    }
    
    public class hclBufferLayout
    {
        public hclBufferLayoutBufferElement m_elementsLayout;
        public hclBufferLayoutSlot m_slots;
        public byte m_numSlots;
        public TriangleFormat m_triangleFormat;
    }
}
