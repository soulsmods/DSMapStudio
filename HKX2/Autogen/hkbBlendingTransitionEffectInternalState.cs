using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBlendingTransitionEffectInternalState : hkReferencedObject
    {
        public Vector4 m_fromPos;
        public Quaternion m_fromRot;
        public Vector4 m_toPos;
        public Quaternion m_toRot;
        public Vector4 m_lastPos;
        public Quaternion m_lastRot;
        public List<hkQTransform> m_characterPoseAtBeginningOfTransition;
        public float m_timeRemaining;
        public float m_timeInTransition;
        public SelfTransitionMode m_toGeneratorSelfTranstitionMode;
        public bool m_initializeCharacterPose;
        public bool m_alignThisFrame;
        public bool m_alignmentFinished;
    }
}
