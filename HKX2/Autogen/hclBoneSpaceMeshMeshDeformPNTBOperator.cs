using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPNTBOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockPNTB> m_localPNTBs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedPNTB> m_localUnpackedPNTBs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNTBs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockPNTB>(br);
            m_localUnpackedPNTBs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockUnpackedPNTB>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
