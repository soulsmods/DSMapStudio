using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbTwistModifier : hkbModifier
    {
        public enum SetAngleMethod
        {
            LINEAR = 0,
            RAMPED = 1,
        }
        
        public enum RotationAxisCoordinates
        {
            ROTATION_AXIS_IN_MODEL_COORDINATES = 0,
            ROTATION_AXIS_IN_PARENT_COORDINATES = 1,
            ROTATION_AXIS_IN_LOCAL_COORDINATES = 2,
        }
        
        public Vector4 m_axisOfRotation;
        public float m_twistAngle;
        public short m_startBoneIndex;
        public short m_endBoneIndex;
        public SetAngleMethod m_setAngleMethod;
        public RotationAxisCoordinates m_rotationAxisCoordinates;
        public bool m_isAdditive;
    }
}
