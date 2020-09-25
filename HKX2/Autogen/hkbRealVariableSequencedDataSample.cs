using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRealVariableSequencedDataSample : IHavokObject
    {
        public float m_time;
        public float m_value;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            m_value = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteSingle(m_value);
        }
    }
}
