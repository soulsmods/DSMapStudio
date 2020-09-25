using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAttributeModifierAssignment : IHavokObject
    {
        public int m_attributeIndex;
        public float m_attributeValue;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_attributeIndex = br.ReadInt32();
            m_attributeValue = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_attributeIndex);
            bw.WriteSingle(m_attributeValue);
        }
    }
}
