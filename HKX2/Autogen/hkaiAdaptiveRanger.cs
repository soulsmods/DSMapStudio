using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiAdaptiveRanger : IHavokObject
    {
        public float m_curRange;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_curRange = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_curRange);
        }
    }
}
