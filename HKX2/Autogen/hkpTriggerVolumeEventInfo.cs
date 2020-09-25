using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpTriggerVolumeEventInfo : IHavokObject
    {
        public ulong m_sortValue;
        public hkpRigidBody m_body;
        public Operation m_operation;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_sortValue = br.ReadUInt64();
            m_body = des.ReadClassPointer<hkpRigidBody>(br);
            m_operation = (Operation)br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_sortValue);
            // Implement Write
            bw.WriteUInt32(0);
        }
    }
}
