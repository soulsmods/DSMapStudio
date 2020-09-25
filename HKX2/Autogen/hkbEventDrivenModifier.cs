using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventDrivenModifier : hkbModifierWrapper
    {
        public int m_activateEventId;
        public int m_deactivateEventId;
        public bool m_activeByDefault;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_activateEventId = br.ReadInt32();
            m_deactivateEventId = br.ReadInt32();
            m_activeByDefault = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_activateEventId);
            bw.WriteInt32(m_deactivateEventId);
            bw.WriteBoolean(m_activeByDefault);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
