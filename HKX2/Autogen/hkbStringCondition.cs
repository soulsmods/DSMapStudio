using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStringCondition : hkbCondition
    {
        public string m_conditionString;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_conditionString = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
