using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SensingMode
    {
        SENSE_IN_NEARBY_RIGID_BODIES = 0,
        SENSE_IN_RIGID_BODIES_OUTSIDE_THIS_CHARACTER = 1,
        SENSE_IN_OTHER_CHARACTER_RIGID_BODIES = 2,
        SENSE_IN_THIS_CHARACTER_RIGID_BODIES = 3,
        SENSE_IN_GIVEN_CHARACTER_RIGID_BODIES = 4,
        SENSE_IN_GIVEN_RIGID_BODY = 5,
        SENSE_IN_OTHER_CHARACTER_SKELETON = 6,
        SENSE_IN_THIS_CHARACTER_SKELETON = 7,
        SENSE_IN_GIVEN_CHARACTER_SKELETON = 8,
        SENSE_IN_GIVEN_LOCAL_FRAME_GROUP = 9,
    }
    
    public class hkbSenseHandleModifier : hkbModifier
    {
        public Vector4 m_sensorLocalOffset;
        public List<hkbSenseHandleModifierRange> m_ranges;
        public hkbHandle m_handleOut;
        public hkbHandle m_handleIn;
        public string m_localFrameName;
        public string m_sensorLocalFrameName;
        public float m_minDistance;
        public float m_maxDistance;
        public float m_distanceOut;
        public uint m_collisionFilterInfo;
        public short m_sensorRagdollBoneIndex;
        public short m_sensorAnimationBoneIndex;
        public SensingMode m_sensingMode;
        public bool m_extrapolateSensorPosition;
        public bool m_keepFirstSensedHandle;
        public bool m_foundHandleOut;
    }
}
