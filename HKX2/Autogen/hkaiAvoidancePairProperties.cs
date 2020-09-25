using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiAvoidancePairProperties : hkReferencedObject
    {
        public List<hkaiAvoidancePairPropertiesPairData> m_avoidancePairDataMap;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_avoidancePairDataMap = des.ReadClassArray<hkaiAvoidancePairPropertiesPairData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
