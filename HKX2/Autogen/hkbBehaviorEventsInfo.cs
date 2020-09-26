using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorEventsInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public List<short> m_externalEventIds;
        public int m_padding;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_externalEventIds = des.ReadInt16Array(br);
            m_padding = br.ReadInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            bw.WriteInt32(m_padding);
            bw.WriteUInt32(0);
        }
    }
}
