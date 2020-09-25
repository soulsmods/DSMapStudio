using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinPNTBOperator : hclObjectSpaceSkinOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNTBs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockPNTB>(br);
            m_localUnpackedPNTBs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockUnpackedPNTB>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
