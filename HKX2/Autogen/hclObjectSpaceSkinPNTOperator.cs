using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinPNTOperator : hclObjectSpaceSkinOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPNT> m_localPNTs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPNT> m_localUnpackedPNTs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNTs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockPNT>(br);
            m_localUnpackedPNTs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockUnpackedPNT>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
