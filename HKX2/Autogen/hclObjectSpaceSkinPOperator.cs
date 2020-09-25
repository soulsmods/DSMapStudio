using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinPOperator : hclObjectSpaceSkinOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockP> m_localPs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedP> m_localUnpackedPs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockP>(br);
            m_localUnpackedPs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockUnpackedP>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
