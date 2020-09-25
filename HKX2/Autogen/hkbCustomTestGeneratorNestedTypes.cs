using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGeneratorNestedTypes : hkbCustomTestGeneratorNestedTypesBase
    {
        public hkbCustomTestGeneratorNestedTypesBase m_nestedTypeStruct;
        public List<hkbCustomTestGeneratorNestedTypesBase> m_nestedTypeArrayStruct;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nestedTypeStruct = new hkbCustomTestGeneratorNestedTypesBase();
            m_nestedTypeStruct.Read(des, br);
            m_nestedTypeArrayStruct = des.ReadClassArray<hkbCustomTestGeneratorNestedTypesBase>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_nestedTypeStruct.Write(bw);
        }
    }
}
