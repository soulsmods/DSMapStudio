using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSequence : hkbModifier
    {
        public List<hkbEventSequencedData> m_eventSequencedData;
        public List<hkbRealVariableSequencedData> m_realVariableSequencedData;
        public List<hkbBoolVariableSequencedData> m_boolVariableSequencedData;
        public List<hkbIntVariableSequencedData> m_intVariableSequencedData;
        public int m_enableEventId;
        public int m_disableEventId;
        public hkbSequenceStringData m_stringData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventSequencedData = des.ReadClassPointerArray<hkbEventSequencedData>(br);
            m_realVariableSequencedData = des.ReadClassPointerArray<hkbRealVariableSequencedData>(br);
            m_boolVariableSequencedData = des.ReadClassPointerArray<hkbBoolVariableSequencedData>(br);
            m_intVariableSequencedData = des.ReadClassPointerArray<hkbIntVariableSequencedData>(br);
            m_enableEventId = br.ReadInt32();
            m_disableEventId = br.ReadInt32();
            m_stringData = des.ReadClassPointer<hkbSequenceStringData>(br);
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_enableEventId);
            bw.WriteInt32(m_disableEventId);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
