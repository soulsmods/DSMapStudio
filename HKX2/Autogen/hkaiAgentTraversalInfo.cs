using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiAgentTraversalInfo : IHavokObject
    {
        public float m_diameter;
        public uint m_filterInfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_diameter = br.ReadSingle();
            m_filterInfo = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_diameter);
            bw.WriteUInt32(m_filterInfo);
        }
    }
}
