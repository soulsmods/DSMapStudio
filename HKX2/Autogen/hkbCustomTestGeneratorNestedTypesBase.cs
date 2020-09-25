using SoulsFormats;
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
        public List<sbyte> m_nestedTypeArrayHkInt8;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nestedTypeHkbGeneratorPtr = des.ReadClassPointer<hkbGenerator>(br);
            m_nestedTypeHkbGeneratorRefPtr = des.ReadClassPointer<hkbGenerator>(br);
            m_nestedTypeHkbModifierPtr = des.ReadClassPointer<hkbModifier>(br);
            m_nestedTypeHkbModifierRefPtr = des.ReadClassPointer<hkbModifier>(br);
            m_nestedTypeHkbCustomIdSelectorPtr = des.ReadClassPointer<hkbCustomIdSelector>(br);
            m_nestedTypeHkbCustomIdSelectorRefPtr = des.ReadClassPointer<hkbCustomIdSelector>(br);
            m_nestedTypeArrayBool = des.ReadBooleanArray(br);
            m_nestedTypeArrayHkBool = des.ReadBooleanArray(br);
            m_nestedTypeArrayCString = des.ReadStringPointerArray(br);
            m_nestedTypeArrayHkStringPtr = des.ReadStringPointerArray(br);
            m_nestedTypeArrayHkInt8 = des.ReadSByteArray(br);
            m_nestedTypeArrayHkInt16 = des.ReadInt16Array(br);
            m_nestedTypeArrayHkInt32 = des.ReadInt32Array(br);
            m_nestedTypeArrayHkUint8 = des.ReadByteArray(br);
            m_nestedTypeArrayHkUint16 = des.ReadUInt16Array(br);
            m_nestedTypeArrayHkUint32 = des.ReadUInt32Array(br);
            m_nestedTypeArrayHkReal = des.ReadSingleArray(br);
            m_nestedTypeArrayHkbGeneratorPtr = des.ReadClassPointerArray<hkbGenerator>(br);
            m_nestedTypeArrayHkbGeneratorRefPtr = des.ReadClassPointerArray<hkbGenerator>(br);
            m_nestedTypeArrayHkbModifierPtr = des.ReadClassPointerArray<hkbModifier>(br);
            m_nestedTypeArrayHkbModifierRefPtr = des.ReadClassPointerArray<hkbModifier>(br);
            m_nestedTypeArrayHkbCustomIdSelectorPtr = des.ReadClassPointerArray<hkbCustomIdSelector>(br);
            m_nestedTypeArrayHkbCustomIdSelectorRefPtr = des.ReadClassPointerArray<hkbCustomIdSelector>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
        }
    }
}
