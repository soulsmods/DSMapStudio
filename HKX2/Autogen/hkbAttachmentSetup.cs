using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum AttachmentType
    {
        ATTACHMENT_TYPE_KEYFRAME_RIGID_BODY = 0,
        ATTACHMENT_TYPE_BALL_SOCKET_CONSTRAINT = 1,
        ATTACHMENT_TYPE_RAGDOLL_CONSTRAINT = 2,
        ATTACHMENT_TYPE_SET_WORLD_FROM_MODEL = 3,
        ATTACHMENT_TYPE_NONE = 4,
    }
    
    public class hkbAttachmentSetup : hkReferencedObject
    {
        public float m_blendInTime;
        public float m_moveAttacherFraction;
        public float m_gain;
        public float m_extrapolationTimeStep;
        public float m_fixUpGain;
        public float m_maxLinearDistance;
        public float m_maxAngularDistance;
        public AttachmentType m_attachmentType;
    }
}
