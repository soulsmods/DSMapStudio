using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventsFromRangeModifier : hkbModifier
    {
        public float m_inputValue;
        public float m_lowerBound;
        public hkbEventRangeDataArray m_eventRanges;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_inputValue = br.ReadSingle();
            m_lowerBound = br.ReadSingle();
            m_eventRanges = des.ReadClassPointer<hkbEventRangeDataArray>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_inputValue);
            bw.WriteSingle(m_lowerBound);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
