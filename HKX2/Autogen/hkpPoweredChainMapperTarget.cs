using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPoweredChainMapperTarget : IHavokObject
    {
        public hkpPoweredChainData m_chain;
        public int m_infoIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_chain = des.ReadClassPointer<hkpPoweredChainData>(br);
            m_infoIndex = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteInt32(m_infoIndex);
            bw.WriteUInt32(0);
        }
    }
}
