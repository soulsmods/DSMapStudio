using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceMeshMeshDeformPOperator : hclBoneSpaceMeshMeshDeformOperator
    {
        public List<hclBoneSpaceDeformerLocalBlockP> m_localPs;
        public List<hclBoneSpaceDeformerLocalBlockUnpackedP> m_localUnpackedPs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockP>(br);
            m_localUnpackedPs = des.ReadClassArray<hclBoneSpaceDeformerLocalBlockUnpackedP>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
