using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGeneratorNestedTypesBase : hkbCustomTestGeneratorComplexTypes
    {
        public hkbGenerator m_nestedTypeHkbGeneratorPtr;
        public hkbGenerator m_nestedTypeHkbGeneratorRefPtr;
        public hkbModifier m_nestedTypeHkbModifierPtr;
        public hkbModifier m_nestedTypeHkbModifierRefPtr;
        public hkbCustomIdSelector m_nestedTypeHkbCustomIdSelectorPtr;
        public hkbCustomIdSelector m_nestedTypeHkbCustomIdSelectorRefPtr;
        public List<bool> m_nestedTypeArrayBool;
        public List<bool> m_nestedTypeArrayHkBool;
        public List<string> m_nestedTypeArrayCString;
        public List<string> m_nestedTypeArrayHkStringPtr;
        public List<char> m_nestedTypeArrayHkInt8;
        public List<short> m_nestedTypeArrayHkInt16;
        public List<int> m_nestedTypeArrayHkInt32;
        public List<byte> m_nestedTypeArrayHkUint8;
        public List<ushort> m_nestedTypeArrayHkUint16;
        public List<uint> m_nestedTypeArrayHkUint32;
        public List<float> m_nestedTypeArrayHkReal;
        public List<hkbGenerator> m_nestedTypeArrayHkbGeneratorPtr;
        public List<hkbGenerator> m_nestedTypeArrayHkbGeneratorRefPtr;
        public List<hkbModifier> m_nestedTypeArrayHkbModifierPtr;
        public List<hkbModifier> m_nestedTypeArrayHkbModifierRefPtr;
        public List<hkbCustomIdSelector> m_nestedTypeArrayHkbCustomIdSelectorPtr;
        public List<hkbCustomIdSelector> m_nestedTypeArrayHkbCustomIdSelectorRefPtr;
    }
}
