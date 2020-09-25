using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BindingType
    {
        BINDING_TYPE_VARIABLE = 0,
        BINDING_TYPE_CHARACTER_PROPERTY = 1,
    }
    
    public enum InternalBindingFlags
    {
        FLAG_NONE = 0,
        FLAG_OUTPUT = 1,
    }
    
    public class hkbVariableBindingSetBinding : IHavokObject
    {
        public string m_memberPath;
        public int m_variableIndex;
        public sbyte m_bitIndex;
        public BindingType m_bindingType;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_memberPath = des.ReadStringPointer(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_variableIndex = br.ReadInt32();
            m_bitIndex = br.ReadSByte();
            m_bindingType = (BindingType)br.ReadSByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_variableIndex);
            bw.WriteSByte(m_bitIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
