using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetNodePropertyCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public string m_nodeName;
        public string m_propertyName;
        public hkbVariableValue m_propertyValue;
        public int m_padding;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_nodeName = des.ReadStringPointer(br);
            m_propertyName = des.ReadStringPointer(br);
            m_propertyValue = new hkbVariableValue();
            m_propertyValue.Read(des, br);
            m_padding = br.ReadInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            m_propertyValue.Write(bw);
            bw.WriteInt32(m_padding);
        }
    }
}
