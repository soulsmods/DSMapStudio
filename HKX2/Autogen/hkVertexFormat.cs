using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ComponentType
    {
        TYPE_NONE = 0,
        TYPE_INT8 = 1,
        TYPE_UINT8 = 2,
        TYPE_INT16 = 3,
        TYPE_UINT16 = 4,
        TYPE_INT32 = 5,
        TYPE_UINT32 = 6,
        TYPE_UINT8_DWORD = 7,
        TYPE_ARGB32 = 8,
        TYPE_FLOAT16 = 9,
        TYPE_FLOAT32 = 10,
        TYPE_VECTOR4 = 11,
        TYPE_LAST = 12,
    }
    
    public enum ComponentUsage
    {
        USAGE_NONE = 0,
        USAGE_POSITION = 1,
        USAGE_NORMAL = 2,
        USAGE_COLOR = 3,
        USAGE_TANGENT = 4,
        USAGE_BINORMAL = 5,
        USAGE_BLEND_MATRIX_INDEX = 6,
        USAGE_BLEND_WEIGHTS = 7,
        USAGE_BLEND_WEIGHTS_LAST_IMPLIED = 8,
        USAGE_TEX_COORD = 9,
        USAGE_POINT_SIZE = 10,
        USAGE_USER = 11,
        USAGE_LAST = 12,
    }
    
    public enum HintFlags
    {
        FLAG_READ = 1,
        FLAG_WRITE = 2,
        FLAG_DYNAMIC = 4,
        FLAG_NOT_SHARED = 8,
    }
    
    public enum SharingType
    {
        SHARING_ALL_SHARED = 0,
        SHARING_ALL_NOT_SHARED = 1,
        SHARING_MIXTURE = 2,
    }
    
    public class hkVertexFormat
    {
        public hkVertexFormatElement m_elements;
        public int m_numElements;
    }
}
