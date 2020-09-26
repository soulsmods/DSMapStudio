using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SystemEventIds
    {
        EVENT_ID_NULL = -1,
    }
    
    public class hkbEventBase : IHavokObject
    {
        public int m_id;
        public hkbEventPayload m_payload;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadInt32();
            br.ReadUInt32();
            m_payload = des.ReadClassPointer<hkbEventPayload>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_id);
            bw.WriteUInt32(0);
            // Implement Write
        }
    }
}
