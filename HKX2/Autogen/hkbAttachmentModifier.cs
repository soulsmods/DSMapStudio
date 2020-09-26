using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAttachmentModifier : hkbModifier
    {
        public hkbEventProperty m_sendToAttacherOnAttach;
        public hkbEventProperty m_sendToAttacheeOnAttach;
        public hkbEventProperty m_sendToAttacherOnDetach;
        public hkbEventProperty m_sendToAttacheeOnDetach;
        public hkbAttachmentSetup m_attachmentSetup;
        public hkbHandle m_attacherHandle;
        public hkbHandle m_attacheeHandle;
        public int m_attacheeLayer;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sendToAttacherOnAttach = new hkbEventProperty();
            m_sendToAttacherOnAttach.Read(des, br);
            m_sendToAttacheeOnAttach = new hkbEventProperty();
            m_sendToAttacheeOnAttach.Read(des, br);
            m_sendToAttacherOnDetach = new hkbEventProperty();
            m_sendToAttacherOnDetach.Read(des, br);
            m_sendToAttacheeOnDetach = new hkbEventProperty();
            m_sendToAttacheeOnDetach.Read(des, br);
            m_attachmentSetup = des.ReadClassPointer<hkbAttachmentSetup>(br);
            m_attacherHandle = des.ReadClassPointer<hkbHandle>(br);
            m_attacheeHandle = des.ReadClassPointer<hkbHandle>(br);
            m_attacheeLayer = br.ReadInt32();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_sendToAttacherOnAttach.Write(bw);
            m_sendToAttacheeOnAttach.Write(bw);
            m_sendToAttacherOnDetach.Write(bw);
            m_sendToAttacheeOnDetach.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteInt32(m_attacheeLayer);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
