using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiJumpAnalyzer : hkaiTraversalAnalyzer
    {
        public float m_maxHorizontalDistance;
        public float m_maxUpHeight;
        public float m_maxDownHeight;
        public float m_verticalApex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_maxHorizontalDistance = br.ReadSingle();
            m_maxUpHeight = br.ReadSingle();
            m_maxDownHeight = br.ReadSingle();
            m_verticalApex = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_maxHorizontalDistance);
            bw.WriteSingle(m_maxUpHeight);
            bw.WriteSingle(m_maxDownHeight);
            bw.WriteSingle(m_verticalApex);
        }
    }
}
