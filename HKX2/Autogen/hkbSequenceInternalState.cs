using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSequenceInternalState : hkReferencedObject
    {
        public List<int> m_nextSampleEvents;
        public List<int> m_nextSampleReals;
        public List<int> m_nextSampleBools;
        public List<int> m_nextSampleInts;
        public float m_time;
        public bool m_isEnabled;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nextSampleEvents = des.ReadInt32Array(br);
            m_nextSampleReals = des.ReadInt32Array(br);
            m_nextSampleBools = des.ReadInt32Array(br);
            m_nextSampleInts = des.ReadInt32Array(br);
            m_time = br.ReadSingle();
            m_isEnabled = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_time);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
