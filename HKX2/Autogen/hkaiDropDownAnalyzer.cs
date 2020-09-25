using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiDropDownAnalyzer : hkaiTraversalAnalyzer
    {
        public float m_minDropDistance;
        public float m_maxDropDistance;
        public float m_maxUnderhang;
        public float m_verticalLipHeight;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_minDropDistance = br.ReadSingle();
            m_maxDropDistance = br.ReadSingle();
            m_maxUnderhang = br.ReadSingle();
            m_verticalLipHeight = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_minDropDistance);
            bw.WriteSingle(m_maxDropDistance);
            bw.WriteSingle(m_maxUnderhang);
            bw.WriteSingle(m_verticalLipHeight);
        }
    }
}
