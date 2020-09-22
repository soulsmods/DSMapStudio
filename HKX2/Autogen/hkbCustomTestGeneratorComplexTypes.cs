using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CustomEnum
    {
        CUSTOM_ENUM_ALA = 0,
        CUSTOM_ENUM_DEPECHE = 1,
        CUSTOM_ENUM_FURIOUS = 5,
    }
    
    public enum CustomFlag
    {
        CUSTOM_FLAG_NONE = 0,
        CUSTOM_FLAG_UNO = 1,
        CUSTOM_FLAG_ZWEI = 2,
        CUSTOM_FLAG_SHI_OR_YON = 4,
        CUSTOM_FLAG_LOTS_O_BITS = 240,
    }
    
    public class hkbCustomTestGeneratorComplexTypes : hkbCustomTestGeneratorSimpleTypes
    {
        public hkReferencedObject m_complexTypeHkObjectPtr;
        public bool m_complexHiddenTypeCopyStart;
        public Quaternion m_complexTypeHkQuaternion;
        public Vector4 m_complexTypeHkVector4;
        public CustomEnum m_complexTypeEnumHkInt8;
        public CustomEnum m_complexTypeEnumHkInt16;
        public CustomEnum m_complexTypeEnumHkInt32;
        public CustomEnum m_complexTypeEnumHkUint8;
        public CustomEnum m_complexTypeEnumHkUint16;
        public CustomEnum m_complexTypeEnumHkUint32;
        public CustomEnum m_complexTypeEnumHkInt8InvalidCheck;
        public CustomEnum m_complexTypeEnumHkInt16InvalidCheck;
        public CustomEnum m_complexTypeEnumHkInt32InvalidCheck;
        public CustomEnum m_complexTypeEnumHkUint8InvalidCheck;
        public CustomEnum m_complexTypeEnumHkUint16InvalidCheck;
        public CustomEnum m_complexTypeEnumHkUint32InvalidCheck;
        public uint m_complexTypeFlagsHkInt8;
        public uint m_complexTypeFlagsHkInt16;
        public uint m_complexTypeFlagsHkInt32;
        public uint m_complexTypeFlagsHkUint8;
        public uint m_complexTypeFlagsHkUint16;
        public uint m_complexTypeFlagsHkUint32;
        public uint m_complexTypeFlagsHkInt8InvalidCheck;
        public uint m_complexTypeFlagsHkInt16InvalidCheck;
        public uint m_complexTypeFlagsHkInt32InvalidCheck;
        public uint m_complexTypeFlagsHkUint8InvalidCheck;
        public uint m_complexTypeFlagsHkUint16InvalidCheck;
        public uint m_complexTypeFlagsHkUint32InvalidCheck;
        public bool m_complexHiddenTypeCopyEnd;
    }
}
