using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAiControlControlDataNonBlendable : IHavokObject
    {
        public bool m_canControl;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_canControl = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_canControl);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
