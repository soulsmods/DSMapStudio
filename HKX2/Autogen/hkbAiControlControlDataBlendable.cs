using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAiControlControlDataBlendable : IHavokObject
    {
        public float m_desiredSpeed;
        public float m_maximumSpeed;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_desiredSpeed = br.ReadSingle();
            m_maximumSpeed = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_desiredSpeed);
            bw.WriteSingle(m_maximumSpeed);
        }
    }
}
