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
    }
}
