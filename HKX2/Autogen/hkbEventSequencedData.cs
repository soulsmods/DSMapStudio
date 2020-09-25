using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventSequencedData : hkbSequencedData
    {
        public List<hkbEventSequencedDataSequencedEvent> m_events;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_events = des.ReadClassArray<hkbEventSequencedDataSequencedEvent>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
