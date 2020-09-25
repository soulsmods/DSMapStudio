using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBoolVariableSequencedDataSample : IHavokObject
    {
        public float m_time;
        public bool m_value;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            m_value = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteBoolean(m_value);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
