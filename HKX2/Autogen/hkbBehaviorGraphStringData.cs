using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorGraphStringData : hkReferencedObject
    {
        public List<string> m_eventNames;
        public List<string> m_attributeNames;
        public List<string> m_variableNames;
        public List<string> m_characterPropertyNames;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventNames = des.ReadStringPointerArray(br);
            m_attributeNames = des.ReadStringPointerArray(br);
            m_variableNames = des.ReadStringPointerArray(br);
            m_characterPropertyNames = des.ReadStringPointerArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
