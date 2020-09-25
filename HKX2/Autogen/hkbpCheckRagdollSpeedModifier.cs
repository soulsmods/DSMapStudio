using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbpCheckRagdollSpeedModifier : hkbModifier
    {
        public hkbEventProperty m_eventToSend;
        public float m_minSpeedThreshold;
        public float m_maxSpeedThreshold;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventToSend = new hkbEventProperty();
            m_eventToSend.Read(des, br);
            m_minSpeedThreshold = br.ReadSingle();
            m_maxSpeedThreshold = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_eventToSend.Write(bw);
            bw.WriteSingle(m_minSpeedThreshold);
            bw.WriteSingle(m_maxSpeedThreshold);
        }
    }
}
