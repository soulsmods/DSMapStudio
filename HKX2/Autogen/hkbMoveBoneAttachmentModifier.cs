using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbMoveBoneAttachmentModifier : hkbModifier
    {
        public int m_activateEventId;
        public string m_attachment;
        public string m_localFrame;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_activateEventId = br.ReadInt32();
            br.AssertUInt32(0);
            m_attachment = des.ReadStringPointer(br);
            m_localFrame = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_activateEventId);
            bw.WriteUInt32(0);
        }
    }
}
