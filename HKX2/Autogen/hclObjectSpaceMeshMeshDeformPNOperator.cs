using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceMeshMeshDeformPNOperator : hclObjectSpaceMeshMeshDeformOperator
    {
        public List<hclObjectSpaceDeformerLocalBlockPN> m_localPNs;
        public List<hclObjectSpaceDeformerLocalBlockUnpackedPN> m_localUnpackedPNs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localPNs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockPN>(br);
            m_localUnpackedPNs = des.ReadClassArray<hclObjectSpaceDeformerLocalBlockUnpackedPN>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
