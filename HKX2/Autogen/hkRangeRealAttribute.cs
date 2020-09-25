using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkRangeRealAttribute : IHavokObject
    {
        public float m_absmin;
        public float m_absmax;
        public float m_softmin;
        public float m_softmax;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_absmin = br.ReadSingle();
            m_absmax = br.ReadSingle();
            m_softmin = br.ReadSingle();
            m_softmax = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_absmin);
            bw.WriteSingle(m_absmax);
            bw.WriteSingle(m_softmin);
            bw.WriteSingle(m_softmax);
        }
    }
}
