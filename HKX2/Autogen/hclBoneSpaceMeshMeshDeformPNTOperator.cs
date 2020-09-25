using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPNTOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPNT> m_localPNTs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNT> m_localUnpackedPNTs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNTs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockPNT>(br);
            m_localUnpackedPNTs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockUnpackedPNT>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
