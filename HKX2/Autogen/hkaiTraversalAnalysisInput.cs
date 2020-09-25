using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiTraversalAnalysisInput : hkReferencedObject
    {
        public List<hkaiTraversalAnalysisInputSection> m_sections;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sections = des.ReadClassArray<hkaiTraversalAnalysisInputSection>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
