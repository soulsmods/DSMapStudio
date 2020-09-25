using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAiControlControlData : IHavokObject
    {
        public hkbAiControlControlDataBlendable m_blendable;
        public hkbAiControlControlDataNonBlendable m_nonBlendable;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_blendable = new hkbAiControlControlDataBlendable();
            m_blendable.Read(des, br);
            m_nonBlendable = new hkbAiControlControlDataNonBlendable();
            m_nonBlendable.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_blendable.Write(bw);
            m_nonBlendable.Write(bw);
        }
    }
}
